using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Common;
using Summer.System.Log;
using VPITest.Net;
using VPITest.Net;
using VPITest.Model;

namespace VPITest.Protocol
{
    public class ProtocolFactorySimFct
    {
        RxMsgQueue rxFctMsgQueue;
        RxMsgQueue rxGeneralMsgQueue;
        RxMsgQueue rxSelfMsgQueue;
        TxMsgQueue txMsgQueue;
        Cabinet cabinet;

        bool isFctRunning;
        bool isGeneralRunning;
        //编码工厂
        public void ExecuteInternal()
        {
            List<BaseRequest> list = txMsgQueue.PopAll();
            foreach (var br in list)
            {
                if (br is ShakeRequest)
                {
                    ShakeResponse sr = new ShakeResponse();
                    sr.CommunicatinBoard = (br as ShakeRequest).Board;
                    sr.DtTime = DateTime.Now;
                    isFctRunning = false;
                    rxFctMsgQueue.Push(sr);
                    rxGeneralMsgQueue.Push(sr);
                    rxSelfMsgQueue.Push(sr);
                }
                else if (br is StartFctRequest)
                {
                    isFctRunning = true;
                }
                else if (br is StopFctRequest)
                {
                    isFctRunning = true;
                    StopFctTestResponse sr = new StopFctTestResponse();
                    sr.CommunicatinBoard = (br as StopFctRequest).Board;
                    sr.DtTime = DateTime.Now;
                    isFctRunning = false;
                    rxFctMsgQueue.Push(sr);
                }
                else if (br is StartGeneralTestRequest)
                {
                    isGeneralRunning = true;
                }
                else if (br is StopFctRequest)
                {
                    isGeneralRunning = false;
                }             
            }

            if (isFctRunning || isGeneralRunning)
            {
                //周期性的送心跳
                HeartMsg msg1 = new HeartMsg();
                msg1.CommunicatinBoard = cabinet.Racks[0].Boards[0];
                msg1.DtTime = DateTime.Now;
                rxFctMsgQueue.Push(msg1);
                rxGeneralMsgQueue.Push(msg1);
                rxSelfMsgQueue.Push(msg1);
                HeartMsg msg2 = new HeartMsg();
                msg2.CommunicatinBoard = cabinet.Racks[0].Boards[3];
                msg2.DtTime = DateTime.Now;
                rxFctMsgQueue.Push(msg2);
                rxGeneralMsgQueue.Push(msg2);
                rxSelfMsgQueue.Push(msg2);
            }

            if (isFctRunning || isGeneralRunning)
            {
                //周期性送错误码子
                {
                    ComponentTestResponse cr1 = new ComponentTestResponse();
                    cr1.CommunicatinBoard = cabinet.Racks[0].Boards[0];
                    cr1.Component = cabinet.Racks[0].Boards[0].ComponentTypes[0].Components[2];
                    cr1.Component.AllTestTimes++;
                    cr1.Component.ErrorPackageTimes++;
                    cr1.DtTime = DateTime.Now;
                    rxFctMsgQueue.Push(cr1);
                    rxGeneralMsgQueue.Push(cr1);
                    rxSelfMsgQueue.Push(cr1);
                }
                {
                    ComponentTestResponse cr1 = new ComponentTestResponse();
                    cr1.CommunicatinBoard = cabinet.Racks[0].Boards[0];
                    cr1.Component = cabinet.Racks[0].Boards[0].ComponentTypes[1].Components[0];
                    cr1.Component.AllTestTimes++;
                    cr1.Component.LostPackageTimes++;
                    cr1.DtTime = DateTime.Now;
                    rxFctMsgQueue.Push(cr1);
                    rxGeneralMsgQueue.Push(cr1);
                    rxSelfMsgQueue.Push(cr1);
                }
                {
                    ComponentTestResponse cr1 = new ComponentTestResponse();
                    cr1.CommunicatinBoard = cabinet.Racks[0].Boards[0];
                    cr1.Component = cabinet.Racks[0].Boards[0].ComponentTypes[1].Components[1];
                    cr1.Component.AllTestTimes++;
                    cr1.DtTime = DateTime.Now;
                    rxFctMsgQueue.Push(cr1);
                    rxGeneralMsgQueue.Push(cr1);
                    rxSelfMsgQueue.Push(cr1);
                }
                {
                    ComponentTestResponse cr1 = new ComponentTestResponse();
                    cr1.CommunicatinBoard = cabinet.Racks[0].Boards[3];
                    cr1.Component = cabinet.Racks[0].Boards[3].ComponentTypes[0].Components[0];
                    cr1.Component.AllTestTimes++;
                    cr1.DtTime = DateTime.Now;
                    rxFctMsgQueue.Push(cr1);
                    rxGeneralMsgQueue.Push(cr1);
                    rxSelfMsgQueue.Push(cr1);
                }
            }

            if (isGeneralRunning)
            {
                VIBTestResponse vr1 = new VIBTestResponse();
                vr1.Board = cabinet.Racks[2].Boards[2];
                vr1.CommunicatinBoard = cabinet.Racks[0].Boards[3];
                vr1.DtTime = DateTime.Now;
                vr1.ErrorTimes = 0;
                vr1.LightPos = 10;
                vr1.ExpectedCode = 0xEAB10499;
                vr1.RealCode = 0xEAB10499;
                rxGeneralMsgQueue.Push(vr1);
                rxSelfMsgQueue.Push(vr1);

                VIBTestResponse vr2 = new VIBTestResponse();
                vr2.Board = cabinet.Racks[2].Boards[2];
                vr2.CommunicatinBoard = cabinet.Racks[0].Boards[3];
                vr2.DtTime = DateTime.Now;
                vr2.ErrorTimes++;
                vr2.LightPos = 11;
                vr2.ExpectedCode = 0xEAB10499;
                vr2.RealCode = 0xEAB10498;
                rxGeneralMsgQueue.Push(vr2);
                rxSelfMsgQueue.Push(vr2);
            }
        }
    }
}
