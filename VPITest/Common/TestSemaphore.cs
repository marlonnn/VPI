using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Threading;

namespace VPITest.Common
{
    public class TestSemaphore
    {
        Semaphore semaphore;
        public string RunningTestName
        {
            get;
            set;
        }

        public static string GENERAL_RUNNING = "综合测试";
        public static string SELF_RUNNING = "系统自检";
        public static string FCT_RUNNING = "单项测试";

        public TestSemaphore()
        {
            semaphore = new Semaphore(1);
            RunningTestName = "";
        }

        /// <summary>
        /// 同一时刻只允许一种测试进行。
        /// 如果超时还没有拿到锁，系统返回正在进行的测试名称，否则返回空字符串。
        /// </summary>
        /// <param name="msecs"></param>
        /// <param name="prepareTestName"></param>
        /// <returns></returns>
        public string Attempt(long msecs,string prepareTestName)
        {
            try
            {
                bool res = semaphore.Attempt(msecs);
                if (res)
                {
                    RunningTestName = prepareTestName;                    
                }
                else
                {
                    return string.Format("系统正在进行{0}，请先停止{0}，或者等待{0}结束。",RunningTestName);
                }
            }
            catch (Exception ee)
            { 
            }
            return "";
        }

        public void Release()
        {
            semaphore.Release();
            RunningTestName = "";
        }
    }
}
