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
using System.Net;

namespace VPITest.Model
{
    /// <summary>
    /// VPI单项测试，测试对象是系统机笼CPUPD1和VCOM板卡上所有组件
    /// </summary>
    [Serializable]
    public class GeneralTest
    {
        //Key测试序号，唯一标识本次测试
        public string Key;
        //上一次的key
        public string LastKey;
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
        [field:NonSerialized]
        public event EventHandler OnTestStatusChange;
        [field: NonSerialized]
        public event EventHandler OnBoardStatusChange;
        [field: NonSerialized]
        public event EventHandler OnNewMessage;

        [NonSerialized]
        protected RxMsgQueue rxGeneralMsgQueue;                   //spring初始化完成后后，重载也不覆盖，单态唯一
        [NonSerialized]
        protected TxMsgQueue txMsgQueue;                   //spring初始化完成后后，重载也不覆盖，单态唯一      
        [NonSerialized]
        protected int preTimeout;                     //spring初始化完成后后，重载也不覆盖，单态唯一
        [NonSerialized]
        protected int runTimeout;                    //spring初始化完成后后，重载也不覆盖，单态唯一

        [field: NonSerialized]
        protected DateTime preTestTime;
        [field: NonSerialized]
        protected Dictionary<IPEndPoint, bool> preTimeoutDic; //开始测试时自动初始化
        [field: NonSerialized]
        protected Dictionary<IPEndPoint, DateTime> runTimeoutDic;//开始测试时自动初始化
        [field: NonSerialized]
        protected MessageLogFile generalMessageLogFile;
        [field: NonSerialized]
        protected DbADO dbAdo;
        [field: NonSerialized]
        protected Report report;
        [field: NonSerialized]
        protected TestSemaphore testSemaphore;

        /// <summary>
        /// 检查是否有组件被选中测试
        /// </summary>
        public void CheckTstConfig()
        {
            if (this.Tester == "")
            {
                throw new Exception("请输入测试人员姓名。");
            }
            if (PlanRunningTime <= 0)
            {
                throw new Exception("测试预设时间应该大于0 ！");
            }
            List<Board> boards = Cabinet.GetGeneralTestBoardsList();
            if (boards.Count == 0)
            {
                throw new Exception("没有一块待测板卡被选中，无法进行测试。");
            }
            foreach (var b in boards)
            {
                if (b.IsGeneralTestTested && b.GeneralTestSN.Length == 0)
                {
                    throw new Exception(string.Format("{0}板卡未设置SN号，请设置后再开始测试。",b.EqName));
                }
            }
        }
        //开始测试(点击开始测试按钮，执行测试检查，检查通过后才执行真正的测试)
        public virtual void StartTest()
        {
            FinishReason = "";
            string tryStartMsg = testSemaphore.Attempt(10, TestSemaphore.GENERAL_RUNNING);
            if (tryStartMsg.Length > 0)
            {
                throw new Exception(tryStartMsg);
            }
            //测试准备
            preTestTime = DateTime.Now;
            preTimeoutDic = new Dictionary<IPEndPoint, bool>();
            runTimeoutDic = new Dictionary<IPEndPoint, DateTime>();
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    b.IsGeneralTestPassed = false;//默认是false，检查通过后才全部置为true，开始真正测试
                    foreach (var ct in b.ComponentTypes)  //清空组件计数
                    {
                        foreach (var c in ct.Components)
                        {
                            c.AllTestTimes = 0;
                            c.ErrorPackageTimes = 0;
                            c.LostPackageTimes = 0;
                            c.InterruptTimes = 0;
                        }
                    }
                    if (b.IsGeneralTestTested)
                    { 
                        if( !preTimeoutDic.ContainsKey(b.CommunicationIP))
                            preTimeoutDic.Add(b.CommunicationIP, false); 
                        if( !runTimeoutDic.ContainsKey(b.CommunicationIP))
                            runTimeoutDic.Add(b.CommunicationIP, DateTime.Now);
                    }
                }
            }
            //进入临界状态
            GenTestStatusChangeEvent(TestStatus, TestStatus.THRESHOLD,"开始或者重新开始一次新的综合测试。");
            TestStatus = TestStatus.THRESHOLD;
            try
            {
                CheckTstConfig();
                //发送握手指令
                txMsgQueue.Push(ShakeRequest.CreateNew(Cabinet.Racks[0].Boards[0])); //pd1
                txMsgQueue.Push(ShakeRequest.CreateNew(Cabinet.Racks[0].Boards[3])); //vcom
            }
            catch (Exception ee)
            {
                //执行异常处理，并转入异常状态
                FinishUnExpectedTest(TestStatus.THRESHOLD,string.Format("开始一次新的测试时，系统报告了一个内部异常，原因如下：{0}", ee.Message));
                throw;
            }
        }
        //开始一次正常测试
        [field: NonSerialized]
        protected Dictionary<Board, Board[]> judgeMatrix;
        [field: NonSerialized]
        protected GeneralRule rule;
        protected void StartNomalTest()
        {
            Key = Util.GenrateKey();
            StartTime = DateTime.Now;
            RunningTime = 0;
            generalMessageLogFile.Open(Key);
            //所有组件初始状态为通过
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    b.IsGeneralTestPassed = true;
                }
            }
            //初始化检查规则
            rule = new GeneralRule(Cabinet, rxGeneralMsgQueue, judgeMatrix);
            //发送正式测试指令
            txMsgQueue.Push(StartGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[0]));
            txMsgQueue.Push(StartGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[3]));
            TestStatus = TestStatus.RUNNING;
            GenTestStatusChangeEvent(TestStatus.THRESHOLD, TestStatus,"系统自动从临界状态转入正式测试状态。");
        }
        //结束一次正常测试
        protected virtual void FinishExpectedTest()
        {
            FinishReason = DbADO.TEST_FINISH_RESULT_NORMAL;
            //先切换状态，然后执行耗时操作
            if (TestStatus != TestStatus.EXPECTED_FINNISH)
            {
                //发送正常结束指令
                txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[0]));
                txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[3]));
                TestStatus = TestStatus.EXPECTED_FINNISH;
                testSemaphore.Release();
                try
                {
                    LastKey = Key;
                    RunningTime = (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
                    generalMessageLogFile.Close();
                    Save(Key);
                    dbAdo.Save2DB(this);
                    report.GenerateUserPdf(this);
                    foreach (var r in Cabinet.Racks)
                    {
                        foreach (var b in r.Boards)
                        {
                            b.GeneralTestSN = "";
                        }
                    }
                }
                catch (Exception ee)
                {
                    LogHelper.GetLogger<FctTest>().Error(ee.ToString());
                }
                GenTestStatusChangeEvent(TestStatus.RUNNING, TestStatus, FinishReason);
            }
        }
        //获得已测时间
        public long GetRuningDuration()
        {
            return (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
        }
        //结束一次非正常测试
        public void FinishUnExpectedTest(TestStatus preStatus, string reason)
        {
            RunningTime = 0;
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    if (b.IsGeneralTestTested)
                    {
                        b.IsGeneralTestPassed = false;
                    }
                }
            }
            //确保消息只发送一次
            if (TestStatus != TestStatus.UNEXPECTED_FINNISH)
            {
                //发送正常结束指令
                txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[0]));
                txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[3]));
                TestStatus = TestStatus.UNEXPECTED_FINNISH;
                testSemaphore.Release();
                GenTestStatusChangeEvent(preStatus, TestStatus, reason);
            }
        }

        public void FinishManualTest(TestStatus preStatus, string reason)
        {
            FinishReason = reason;
            RunningTime = 0;
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    if (b.IsGeneralTestTested)
                    {
                        b.IsGeneralTestPassed = false;
                    }
                }
            }
            //确保消息只发送一次
            if (TestStatus != TestStatus.FORCE_FINISH)
            {
                //发送正常结束指令
                txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[0]));
                txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[3]));
                TestStatus = TestStatus.FORCE_FINISH;
                testSemaphore.Release();
                try
                {
                    LastKey = Key;
                    RunningTime = (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
                    generalMessageLogFile.Close();
                    Save(Key);
                    dbAdo.Save2DB(this);
                    report.GenerateUserPdf(this);
                    foreach (var r in Cabinet.Racks)
                    {
                        foreach (var b in r.Boards)
                        {
                            b.GeneralTestSN = "";
                        }
                    }
                }
                catch (Exception ee)
                {
                    LogHelper.GetLogger<FctTest>().Error(ee.ToString());
                }
                GenTestStatusChangeEvent(preStatus, TestStatus, reason);
            }
        }
        
        //计划任务，周期性调用此方法
        public void ExecuteInternal()
        {
            List<BaseResponse> rxs = rxGeneralMsgQueue.PopAll();
            if (TestStatus == TestStatus.THRESHOLD)
            {
                //更新握手记录，检查是否在规定时间内完成了握手，如果有一块板卡有异常，系统自动转入异常结束状态
                CheckPreTstHeart(rxs);
            }
            if (TestStatus == TestStatus.RUNNING)
            {
                //判断是否已经达到了计划时长，如果达到了，系统自动转入正常结束
                if (DateTime.Now.Ticks - preTestTime.Ticks > (PlanRunningTime+2) * 10000000)
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
                            generalMessageLogFile.Append(br);
                        }
                        //如果是过滤后的真实组件错误计数增加消息
                        if (br is FiltedComponentTestResponse) //过滤后PD1和VCOM板卡消息
                        {
                            GenOnNewMessageEvent(br);
                            Component c = rule.JudgeComponentPassStatus(br as FiltedComponentTestResponse);
                            if (c != null)
                            {
                                GenBoardStatusChangeEvent(rule.JudgeBoardPassStatus(c.ParentComponentType.ParentBoard));
                            }
                        }
                        else if (br is ComponentTestResponse)//PD1和VCOM板卡消息
                        {
                            rule.ProcessFilter(br as ComponentTestResponse);
                        }
                        //过滤后VPS板卡消息，此条件要放在VPSErrorInfoResponse，否则会被VPSErrorInfoResponse过滤掉
                        else if (br is FiltedVPSErrorInfoResponse)
                        {
                            GenOnNewMessageEvent(br);
                            GenBoardStatusChangeEvent(rule.JudgeBoardPassStatus((br as VPSErrorInfoResponse).ErrorBoard));
                        }
                        else if (br is VPSErrorInfoResponse)  //VPS板卡消息
                        {
                            rule.ProcessFilter(br as VPSErrorInfoResponse);                            
                        }
                        //过滤后VIB板卡消息，此条件要放在VIBTestResponse，否则会被VIBTestResponse过滤掉
                        else if (br is FiltedVIBTestResponse) 
                        {
                            GenOnNewMessageEvent(br);
                            GenBoardStatusChangeEvent(rule.JudgeBoardPassStatus((br as FiltedVIBTestResponse).Board));
                        }
                        else if (br is VIBTestResponse)  //VIB板卡消息
                        {
                            rule.ProcessFilter(br as VIBTestResponse);
                        }
                        else if (br is FiltedVOBTestResponse) //过滤后VOB板卡消息
                        {
                            GenOnNewMessageEvent(br);
                            GenBoardStatusChangeEvent(rule.JudgeBoardPassStatus((br as FiltedVOBTestResponse).Board));
                        }
                        else if (br is VOBTestResponse)  //VOB板卡消息
                        {
                            rule.ProcessFilter(br as VOBTestResponse);
                        }
                        else if (br is FiltedBoardStatusResponse)
                        {
                            GenOnNewMessageEvent(br);
                            GenBoardStatusChangeEvent(rule.JudgeBoardPassStatus((br as FiltedBoardStatusResponse).Board));
                        }
                        else if(br is BoardStatusResponse)
                        {
                            rule.ProcessFilter(br as BoardStatusResponse);       
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
            bool result = true;
            foreach (var r in Cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    if (b.IsGeneralTestTested)
                    {
                        result &= b.IsGeneralTestPassed;
                    }
                }
            }
            return result;
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
        protected void GenBoardStatusChangeEvent(Dictionary<Board,bool> maybeErrorBoardDicts)
        {
            foreach(var b in maybeErrorBoardDicts.Keys)
            {
                BoardStatusEventArgs e = new BoardStatusEventArgs();
                e.Board = b;
                e.IsMessageSource = maybeErrorBoardDicts[b];
                EventHandler temp = OnBoardStatusChange;
                if (temp != null)
                {
                    temp(this, e);
                }
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
                    if (preTimeoutDic.ContainsKey(msg.CommunicatinBoard.CommunicationIP))
                    {
                        preTimeoutDic[msg.CommunicatinBoard.CommunicationIP] = true;
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
                string reason = "测试系统未在" + preTimeout + "s内收到板卡";
                foreach (var kv in preTimeoutDic)
                {
                    if (kv.Value == false)
                    {
                        Board b = Cabinet.GetCommunicationBoard(kv.Key);
                        reason += string.Format("{0} ",b.EqName);
                    }
                }
                reason += "的握手反馈，本次测试无法正常开始，系统自动转入异常结束。";
                //执行异常结束工作
                FinishUnExpectedTest(TestStatus.THRESHOLD,reason);
            }
        }
        //检查是否有心跳超时
        private void CheckRunningTstHeart(List<BaseResponse> msgList)
        {
            //更新心跳记录
            foreach (var msg in msgList)
            {
                if (msg !=null && msg.CommunicatinBoard != null)
                {
                    if (runTimeoutDic.ContainsKey(msg.CommunicatinBoard.CommunicationIP))
                    {
                        runTimeoutDic[msg.CommunicatinBoard.CommunicationIP] = DateTime.Now.AddSeconds(5);
                    }
                }
            }            
            //判断是否有心跳超时板卡,如果有，生成心跳超时事件并入栈
            HashSet<Board> boards = GetTimeoutBoard(runTimeoutDic, runTimeout);
            foreach (var b in boards)
            {
                HeartTimeoutMsg mymsg = new HeartTimeoutMsg();
                mymsg.CommunicatinBoard = b;
                mymsg.DtTime = DateTime.Now;
                if (rxGeneralMsgQueue != null)
                    rxGeneralMsgQueue.Push(mymsg);
            }
        }
        //获得运行过程中超时的板卡
        private HashSet<Board> GetTimeoutBoard(Dictionary<IPEndPoint, DateTime> dict, int timeout)
        {
            HashSet<Board> boards = new HashSet<Board>();
            foreach (var ip in dict.Keys)
            {
                if (DateTime.Now.Ticks - dict[ip].Ticks > (uint)timeout * 10000000)
                {
                    Board b = Cabinet.GetCommunicationBoard(ip);
                    if(!boards.Contains(b))
                        boards.Add(b);
                }
            }
            return boards;
        }

        #region 序列化相关
        //保存当前对象到硬盘，用于程序下次启动时
        private void Save(string key)
        {
            string pathAndFilename = Util.GetBasePath() + "//Data//General//" + key + ".gnl";
            //序列化用户当前的配置
            System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(pathAndFilename, FileMode.OpenOrCreate, FileAccess.Write);
            using (stream)
            {
                formatter.Serialize(stream, this);
            }
        }

        public GeneralTest LoadByKey(string key)
        {
            string pathAndFilename = Util.GetBasePath() + "//Data//General//" + key + ".gnl";
            try
            {
                System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(pathAndFilename, FileMode.Open, FileAccess.Read);
                GeneralTest tr;
                using (stream)
                {
                    tr = (GeneralTest)formatter.Deserialize(stream);
                }
                return tr;
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<GeneralTest>().Error(ee.Message);
                LogHelper.GetLogger<GeneralTest>().Error(ee.StackTrace);
            }
            return null;
        }
        #endregion
    }
}
