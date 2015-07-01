using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Summer.System.Log;
using System.Windows.Forms;
using VPITest.Common;
using VPITest.Protocol;
using VPITest.DB;

namespace VPITest.Model
{
    /// <summary>
    /// VPI单项测试，测试对象是系统机笼CPUPD1和VCOM板卡上所有组件
    /// </summary>
    [Serializable]
    public class FctTest
    {
        //Key测试序号，唯一标识本次测试
        public string Key;
        //测试人
        public string Tester = "";
        //待测试机柜
        public Cabinet Cabinet;
        //计划测试时长(单位：秒)
        public long PlanRunningTime;
        //测试结束原因
        public string FinishReason;

        //测试状态
        [field: NonSerialized]
        public TestStatus TestStatus = TestStatus.UNEXPECTED_FINNISH;
        //开始测试时间
        [field: NonSerialized]
        public DateTime StartTime;
        //实际测试时长(单位：秒)
        [field: NonSerialized]
        public long RunningTime;

        /// <summary>
        /// 测试阶段状态发生改变时，生成事件消息
        /// </summary>
        [field: NonSerialized]
        public event EventHandler OnTestStatusChange;
        [field: NonSerialized]
        public event EventHandler OnComponentStatusChange;
        [field: NonSerialized]
        public event EventHandler OnNewMessage;

        [NonSerialized]
        RxMsgQueue rxFctMsgQueue;                   //spring初始化完成后后，重载也不覆盖，单态唯一
        [NonSerialized]
        TxMsgQueue txMsgQueue;                   //spring初始化完成后后，重载也不覆盖，单态唯一
        //[NonSerialized]
        //ErrorCodeMsgFile errorCodeMsgFile; //spring初始化完成后后，重载也不覆盖，单态唯一        
        [NonSerialized]
        int preTimeout;                     //spring初始化完成后后，重载也不覆盖，单态唯一
        [NonSerialized]
        int runTimeout;                    //spring初始化完成后后，重载也不覆盖，单态唯一
        [NonSerialized]
        protected int delayTime;    //开始正式开始第一次心跳宽恕(单位：秒)

        [field: NonSerialized]
        DateTime preTestTime;
        [field: NonSerialized]
        Dictionary<Board, bool> preTimeoutDic; //开始测试时自动初始化
        [field: NonSerialized]
        Dictionary<Board, DateTime> runTimeoutDic;//开始测试时自动初始化
        [field: NonSerialized]
        MessageLogFile fctMessageLogFile;
        [field: NonSerialized]
        DbADO dbAdo;
        [field: NonSerialized]
        Report report;
        [field: NonSerialized]
        TestSemaphore testSemaphore;

        /// <summary>
        /// 检查是否有组件被选中测试
        /// </summary>
        public void CheckTstConfig()
        {
            int testCount = 0;
            if (this.Tester == "")
            {
                throw new Exception("请输入测试人员姓名。");
            }

            if (PlanRunningTime <= 0)
            {
                throw new Exception("测试预设时间应该大于0 ！");
            }
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        if (ct.IsFctTestTested)
                        {
                            testCount++;
                        }
                    }
                }
            }
            if (testCount == 0)
            {
                throw new Exception("未选中一个组件，无法进行测试！");
            }

            foreach (Rack r in Cabinet.Racks)
            {
                foreach (Board b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        if (ct.IsFctTestTested)
                        {
                            if (b.FctTestSN.Length == 0)
                            {
                                throw new Exception(string.Format("{0}机笼{1}板卡未设置SN号", b.ParentRack.EqName, b.EqName));
                            }
                        }
                    }
                }
            }
        }
        //开始测试(点击开始测试按钮，执行测试检查，检查通过后才执行真正的测试)
        public void StartTest()
        {
            string tryStartMsg = testSemaphore.Attempt(10, TestSemaphore.FCT_RUNNING);
            if (tryStartMsg.Length > 0)
            {
                throw new Exception(tryStartMsg);
            }
            //测试准备
            preTestTime = DateTime.Now;
            FinishReason = "";
            preTimeoutDic = new Dictionary<Board, bool>();
            runTimeoutDic = new Dictionary<Board, DateTime>();
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        foreach (var c in ct.Components)
                        {
                            c.IsFctTestPassed = false;//默认是false，检查通过后才全部置为true，开始真正测试
                            c.AllTestTimes = 0;
                            c.ErrorPackageTimes = 0;
                            c.LostPackageTimes = 0;
                            c.InterruptTimes = 0;
                        }
                        if (ct.IsFctTestTested)
                        {
                            if (!preTimeoutDic.ContainsKey(b))
                            {
                                preTimeoutDic.Add(b, false);         //针对板卡设定超时
                            }
                        }
                    }
                }
            }
            //进入临界状态
            GenTestStatusChangeEvent(TestStatus, TestStatus.THRESHOLD, "开始或者重新开始一次新的组件测试。");
            TestStatus = TestStatus.THRESHOLD;
            try
            {
                CheckTstConfig();
                //发送握手指令
                txMsgQueue.Push(ShakeRequest.CreateNew(Cabinet.Racks[0].Boards[0]));
                txMsgQueue.Push(ShakeRequest.CreateNew(Cabinet.Racks[0].Boards[3]));
            }
            catch (Exception ee)
            {
                //执行异常处理，并转入异常状态
                FinishUnExpectedTest(TestStatus.THRESHOLD, string.Format("开始一次新的测试时，系统报告了一个内部异常，原因如下：{0}", ee.Message));
                throw;
            }
        }
        //开始一次正常测试
        [field: NonSerialized]
        FctRule rule;
        protected void StartNomalTest()
        {
            Key = Util.GenrateKey();
            StartTime = DateTime.Now;
            RunningTime = 0;
            fctMessageLogFile.Open(Key);
            //所有组件初始状态为通过
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        foreach (var c in ct.Components)
                        {
                            c.IsFctTestPassed = true;//默认是false，检查通过后才全部置为true，开始真正测试
                        }
                        if (ct.IsFctTestTested)
                        {
                            if(!runTimeoutDic.ContainsKey(b))
                                runTimeoutDic.Add(b, DateTime.Now.AddSeconds(delayTime));
                        }
                    }
                }
            }
            //初始化检查规则
            rule = new FctRule(this.Cabinet, this.rxFctMsgQueue);
            //发送正式测试指令
            Dictionary<Board, List<ComponentType>> bcDict = Cabinet.GetFctTestedComponentTypesDicts();
            foreach (var b in bcDict.Keys)
            {
                StartFctRequest sr = StartFctRequest.CreateNew(b, bcDict[b]);
                txMsgQueue.Push(sr);
            }
            TestStatus = TestStatus.RUNNING;
            GenTestStatusChangeEvent(TestStatus.THRESHOLD, TestStatus, "系统自动从临界状态转入正式测试状态。");
        }
        //结束一次正常测试
        protected void FinishExpectedTest()
        {
            //发送正常结束指令
            Dictionary<Board, List<ComponentType>> bcDict = Cabinet.GetFctTestedComponentTypesDicts();
            foreach (var b in bcDict.Keys)
            {
                StopFctRequest sr = StopFctRequest.CreateNew(b);
                txMsgQueue.Push(sr);
            }
            //先切换状态，然后执行耗时操作
            if (TestStatus != TestStatus.EXPECTED_FINNISH)
            {
                TestStatus = TestStatus.EXPECTED_FINNISH;
                testSemaphore.Release();
                FinishReason = DbADO.TEST_FINISH_RESULT_NORMAL;
                try
                {
                    RunningTime = (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
                    fctMessageLogFile.Close();
                    Save(Key);
                    dbAdo.Save2DB(this);
                    report.GenerateUserPdf(this);
                    foreach (var r in Cabinet.Racks)
                    {
                        foreach (var b in r.Boards)
                        {
                            b.FctTestSN = "";
                        }
                    }
                }
                catch (Exception ee)
                {
                    LogHelper.GetLogger<FctTest>().Error(ee.ToString());
                }
                GenTestStatusChangeEvent(TestStatus.RUNNING, TestStatus, "系统自动从正式测试状态转入正常结束状态。");
            }
        }
        //获得已测时间
        public long GetRuningDuration()
        {
            return (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
        }
        //结束一次非正常测试
        private void FinishUnExpectedTest(TestStatus preStatus, string reason)
        {
            FinishReason = reason;
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        if (ct.IsFctTestTested)
                        {
                            foreach (var c in ct.Components)
                            {
                                c.IsFctTestPassed = false;
                            }
                        }
                    }
                }
            }
            //确保消息只发送一次
            if (TestStatus != TestStatus.UNEXPECTED_FINNISH)
            {
                //发送停止测试指令
                Dictionary<Board, List<ComponentType>> bcDict = Cabinet.GetFctTestedComponentTypesDicts();
                foreach (var b in bcDict.Keys)
                {
                    StopFctRequest sr = StopFctRequest.CreateNew(b);
                    txMsgQueue.Push(sr);
                }
                testSemaphore.Release();
                TestStatus = TestStatus.UNEXPECTED_FINNISH;
                GenTestStatusChangeEvent(preStatus, TestStatus, reason);
            }
        }
        //用户强制结束
        public void FinishManualTest(TestStatus preStatus, string reason)
        {
            FinishReason = reason;
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        if (ct.IsFctTestTested)
                        {
                            foreach (var c in ct.Components)
                            {
                                c.IsFctTestPassed = false;
                            }
                        }
                    }
                }
            }
            //确保消息只发送一次
            if (TestStatus != TestStatus.UNEXPECTED_FINNISH)
            {
                //发送停止测试指令
                Dictionary<Board, List<ComponentType>> bcDict = Cabinet.GetFctTestedComponentTypesDicts();
                foreach (var b in bcDict.Keys)
                {
                    StopFctRequest sr = StopFctRequest.CreateNew(b);
                    txMsgQueue.Push(sr);
                }
                testSemaphore.Release();
                TestStatus = TestStatus.UNEXPECTED_FINNISH;
                GenTestStatusChangeEvent(TestStatus.RUNNING, TestStatus, FinishReason);

                RunningTime = (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
                try
                {
                    fctMessageLogFile.Close();
                    Save(Key);
                    dbAdo.Save2DB(this);
                    report.GenerateUserPdf(this);
                }
                catch (Exception ee)
                {
                }
            }
        }
        //计划任务，周期性调用此方法
        public void ExecuteInternal()
        {
            List<BaseResponse> rxs = rxFctMsgQueue.PopAll();
            if (TestStatus == TestStatus.THRESHOLD)
            {
                //更新握手记录，检查是否在规定时间内完成了握手，如果有一块板卡有异常，系统自动转入异常结束状态
                CheckPreTstHeart(rxs);
            }
            if (TestStatus == TestStatus.RUNNING)
            {
                //判断是否已经达到了计划时长，如果达到了，系统自动转入正常结束(结束时间比计划时间多两秒，宽恕一下)
                if (DateTime.Now.Ticks - preTestTime.Ticks > (PlanRunningTime + 2) * 10000000)
                {
                    FinishExpectedTest();
                    return;
                }
                //首先进行心跳判断，如果有板卡心跳超时，系统自动生成心跳超时消息
                CheckRunningTstHeart(rxs);
                rxs.ForEach((br) =>
                {
                    if (!(br is HeartMsg))
                    {
                        //日志系统记录除了心跳以外的所有日志
                        fctMessageLogFile.Append(br);
                    }
                    //如果是板卡组件上报的消息，根据规则具体判断
                    if (br is ComponentTestResponse)
                    {
                        //GenOnNewMessageEvent(br);
                        rule.ProcessFilter(br as ComponentTestResponse);
                    }
                    //如果是过滤后的真实组件错误计数增加消息
                    else if (br is FiltedComponentTestResponse)
                    {
                        GenOnNewMessageEvent(br);
                        Component c = rule.JudgeComponentPassStatus(br as FiltedComponentTestResponse);
                        if (c != null)
                        {
                            GenComponentStatusChangeEvent(c);
                        }
                    }
                    //如果心跳超时，系统自动转入异常结束状态
                    else if (br is HeartTimeoutMsg)
                    {
                        GenOnNewMessageEvent(br);
                        FinishUnExpectedTest(TestStatus.RUNNING,
                            string.Format("板卡{0}心跳超时，系统自动转入异常结束状态。", (br as HeartTimeoutMsg).CommunicatinBoard.EqName));
                    }
                });
            }
        }
        //判断此次测试是否通过
        public bool IsPass()
        {
            return Cabinet.IsFctTestPassed();
        }
        //生成状态切换事件
        protected void GenTestStatusChangeEvent(TestStatus last, TestStatus cur, string reason)
        {
            TestStatusEventArgs e = new TestStatusEventArgs();
            e.LastStatus = last;
            e.CurStatus = cur;
            e.Reason = reason;
            EventHandler temp = OnTestStatusChange;
            if (temp != null)
            {
                temp(this, e);
            }
        }

        //生成板卡通过状态切换事件（在Running 状态下）
        protected void GenComponentStatusChangeEvent(Component component)
        {
            ComponentStatusEventArgs e = new ComponentStatusEventArgs();
            e.Component = component;
            EventHandler temp = OnComponentStatusChange;
            if (temp != null)
            {
                temp(this, e);
            }
        }
        //收到新消息事件（在Running 状态下）
        protected void GenOnNewMessageEvent(BaseResponse baseRxMessage)
        {
            NewMessageEventArgs e = new NewMessageEventArgs();
            e.rxMessage = baseRxMessage;
            EventHandler temp = OnNewMessage;
            if (temp != null)
            {
                temp(this, e);
            }
        }
        //检查是否所有的心跳都已经收到
        private void CheckPreTstHeart(List<BaseResponse> msgList)
        {
            //更新握手记录
            foreach (var msg in msgList)
            {
                if (msg is ShakeResponse && msg.CommunicatinBoard != null)
                {
                    if (preTimeoutDic.ContainsKey(msg.CommunicatinBoard))
                    {
                        preTimeoutDic[msg.CommunicatinBoard] = true;
                    }
                }
            }

            //如果所有握手都收到，开始正式测试
            bool isAllOk = true;
            foreach (var kv in preTimeoutDic)
            {
                isAllOk &= kv.Value;
            }
            if (isAllOk)
            {
                StartNomalTest();
                return;
            }
            //如果超时了，还有板卡没有收到消息，进入异常结束
            if (DateTime.Now.Ticks - preTestTime.Ticks > (uint)preTimeout * 10000000)
            {
                List<Board> boards = new List<Board>();
                string reason = "测试系统未在" + preTimeout + "s内收到板卡";
                foreach (var kv in preTimeoutDic)
                {
                    if (kv.Value == false)
                    {
                        boards.Add(kv.Key);
                        reason += string.Format("{0} ", kv.Key.EqName);
                    }
                }
                reason += "的握手反馈，本次测试无法正常开始，系统自动转入异常结束。";
                //执行异常结束工作
                FinishUnExpectedTest(TestStatus.THRESHOLD, reason);
            }
        }
        //检查是否有心跳超时
        private void CheckRunningTstHeart(List<BaseResponse> msgList)
        {
            //更新心跳记录
            foreach (var msg in msgList)
            {
                //if (msg is HeartMsg && msg.CommunicatinBoard != null)
                if (msg != null && !(msg is ShakeResponse) &&msg.CommunicatinBoard != null)
                {
                    if (runTimeoutDic.ContainsKey(msg.CommunicatinBoard))
                    {
                        runTimeoutDic[msg.CommunicatinBoard] = DateTime.Now;
                    }
                }
            }
            //判断是否有心跳超时板卡,如果有，生成心跳超时事件并入栈
            List<Board> boards = GetTimeoutBoard(runTimeoutDic, runTimeout);
            foreach (var b in boards)
            {
                HeartTimeoutMsg mymsg = new HeartTimeoutMsg();
                mymsg.CommunicatinBoard = b;
                mymsg.DtTime = DateTime.Now;
                if (rxFctMsgQueue != null)
                    rxFctMsgQueue.Push(mymsg);
            }
        }
        //获得运行过程中超时的板卡
        private List<Board> GetTimeoutBoard(Dictionary<Board, DateTime> dict, int timeout)
        {
            List<Board> boards = new List<Board>();
            foreach (var b in dict.Keys)
            {
                if (DateTime.Now.Ticks - dict[b].Ticks > (uint)timeout * 10000000)
                {
                    boards.Add(b);
                }
            }
            return boards;
        }

        #region 序列化相关
        //保存当前对象到硬盘，用于程序下次启动时
        public void SaveThis()
        {
            //序列化用户当前的配置
            Save("last");
        }
        public FctTest LoadLast()
        {
            return LoadByKey("last");
        }

        private void Save(string key)
        {
            string pathAndFilename = Util.GetBasePath() + "//Data//Component//" + key + ".fct";
            //序列化用户当前的配置
            System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(pathAndFilename, FileMode.OpenOrCreate, FileAccess.Write);
            using (stream)
            {
                formatter.Serialize(stream, this);
            }
        }

        public FctTest LoadByKey(string key)
        {
            string pathAndFilename = Util.GetBasePath() + "//Data//Component//" + key + ".fct";
            try
            {
                System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(pathAndFilename, FileMode.Open, FileAccess.Read);
                FctTest tr;
                using (stream)
                {
                    tr = (FctTest)formatter.Deserialize(stream);
                }
                return tr;
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<FctTest>().Error(ee.Message);
                LogHelper.GetLogger<FctTest>().Error(ee.StackTrace);
            }
            return null;
        }
        #endregion
    }
}
