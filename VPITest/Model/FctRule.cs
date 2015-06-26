using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Protocol;
using VPITest.Common;

namespace VPITest.Model
{
    public class FctRule
    {
        protected Cabinet cabinet;
        protected RxMsgQueue rxMsgQueue;
        Dictionary<Component, int> componentErrorPackageTimes = new Dictionary<Component, int>();
        Dictionary<Component, int> componentLostPackageTimes = new Dictionary<Component, int>();
        Dictionary<Component, int> componentInterruptTimes = new Dictionary<Component, int>();

        public FctRule(Cabinet cabinet, RxMsgQueue rxMsgQueue)
        {
            this.cabinet = cabinet;
            this.rxMsgQueue = rxMsgQueue;

            //错误计数全部置零
            foreach (var r in cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var ct in b.ComponentTypes)
                    {
                        foreach (var c in ct.Components)
                        {
                            componentErrorPackageTimes.Add(c, 0);
                            componentLostPackageTimes.Add(c, 0);
                            componentInterruptTimes.Add(c, 0);
                            c.AllTestTimes = 0;                            
                            c.ErrorPackageTimes = 0;
                            c.LostPackageTimes = 0;
                            c.InterruptTimes = 0;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 当误码、丢包、中断三者有其一发生变化时就生成一条过滤后组件消息
        /// </summary>
        /// <param name="cr"></param>
        public void ProcessFilter(ComponentTestResponse cr)
        {
            Component c = cr.Component;
            if (componentErrorPackageTimes.ContainsKey(c) 
                && componentLostPackageTimes.ContainsKey(c) 
                && componentInterruptTimes.ContainsKey(c))
            {
                if (componentErrorPackageTimes[c] < c.ErrorPackageTimes
                    || componentLostPackageTimes[c] < c.LostPackageTimes
                    || componentInterruptTimes[c] <　c.InterruptTimes)
                {
                    FiltedComponentTestResponse fr = FiltedComponentTestResponse.CreateNew(c);
                    rxMsgQueue.Push(fr);
                }
                componentErrorPackageTimes[c] = c.ErrorPackageTimes;
                componentLostPackageTimes[c] = c.LostPackageTimes;
                componentInterruptTimes[c] = c.InterruptTimes;
            }
        }

        /// <summary>
        /// 根据消息判断是否组件测试通过，如果通过，则返回null，否则返回测试失败的组件
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        public Component JudgeComponentPassStatus(FiltedComponentTestResponse cr)
        {
            if (cr.Component.AllTestTimes < 10000 && cr.Component.ErrorPackageTimes >=1)
            {
                cr.Component.IsFctTestPassed = false;
                return cr.Component;
            }
            if (cr.Component.AllTestTimes < 10000 && cr.Component.LostPackageTimes >= 1)
            {
                cr.Component.IsFctTestPassed = false;
                return cr.Component;
            }
            if (cr.Component.AllTestTimes < 10000 && cr.Component.InterruptTimes >= 1)
            {
                cr.Component.IsFctTestPassed = false;
                return cr.Component;
            }
            if (cr.Component.AllTestTimes >= 10000 && (float)cr.Component.ErrorPackageTimes / (float)cr.Component.AllTestTimes > 1.0 / 10000)
            {
                cr.Component.IsFctTestPassed = false;
                return cr.Component;
            }
            return null;
        }
    }
}
