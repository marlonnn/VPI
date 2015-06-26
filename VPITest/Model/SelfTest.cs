using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using VPITest.Common;
using VPITest.Protocol;
using VPITest.DB;
using System.Net;
using Summer.System.Log;

namespace VPITest.Model
{
    /// <summary>
    /// 测试人和测试时长由配置文件配置产生
    /// </summary>
    public class SelfTest : GeneralTest
    {
        public override void StartTest()
        {
            string tryStartMsg = testSemaphore.Attempt(10, TestSemaphore.SELF_RUNNING);
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
                    if (b.CanGeneralTest)
                    {
                        b.IsGeneralTestTested = true;
                        foreach (var ct in b.ComponentTypes) //清空组件计数
                        {
                            foreach (var c in ct.Components)
                            {
                                c.AllTestTimes = 0;
                                c.ErrorPackageTimes = 0;
                                c.LostPackageTimes = 0;
                                c.InterruptTimes = 0;
                            }
                        }
                        if (!preTimeoutDic.ContainsKey(b.CommunicationIP))
                            preTimeoutDic.Add(b.CommunicationIP, false);
                        if (!runTimeoutDic.ContainsKey(b.CommunicationIP))
                            runTimeoutDic.Add(b.CommunicationIP, DateTime.Now);
                    }
                }
            }
            //进入临界状态
            GenTestStatusChangeEvent(TestStatus, TestStatus.THRESHOLD, "开始或者重新开始一次新的综合测试。");
            TestStatus = TestStatus.THRESHOLD;
            try
            {
                //发送握手指令
                txMsgQueue.Push(ShakeRequest.CreateNew(Cabinet.Racks[0].Boards[0])); //pd1
                txMsgQueue.Push(ShakeRequest.CreateNew(Cabinet.Racks[0].Boards[3])); //vcom
            }
            catch (Exception ee)
            {
                //执行异常处理，并转入异常状态
                FinishUnExpectedTest(TestStatus.THRESHOLD, string.Format("开始一次新的测试时，系统报告了一个内部异常，原因如下：{0}", ee.Message));
                throw;
            }
        }

        protected override void FinishExpectedTest()
        {
            //发送正常结束指令
            txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[0]));
            txMsgQueue.Push(StopGeneralTestRequest.CreateNew(Cabinet.Racks[0].Boards[3]));
            //先切换状态，然后执行耗时操作
            if (TestStatus != TestStatus.EXPECTED_FINNISH)
            {
                TestStatus = TestStatus.EXPECTED_FINNISH;
                try
                {
                    RunningTime = (DateTime.Now.Ticks - StartTime.Ticks) / 10000000;
                    generalMessageLogFile.Close();
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
                testSemaphore.Release();
                GenTestStatusChangeEvent(TestStatus.RUNNING, TestStatus, "系统自动从正式测试状态转入正常结束状态。");
            }
        }
    }
}
