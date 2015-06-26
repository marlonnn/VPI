using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Common;
using Summer.System.Log;
using VPITest.Net;
using VPITest.Net;

namespace VPITest.Protocol
{
    public class ProtocolFactory
    {
        FrameProtocol frameProtocol;

        RxQueue rxQueue;
        RxMsgQueue rxFctMsgQueue;
        RxMsgQueue rxGeneralMsgQueue;
        RxMsgQueue rxSelfMsgQueue;
        Dictionary<byte, BaseResponse> Decoders;
        //解码工厂
        public void DecodeInternal()
        {
            List<Original> list = rxQueue.PopAll();
            foreach (var o in list)
            {
                if (o is OriginalBytes)
                {
                    OriginalBytes obytes = o as OriginalBytes;
                    FrameProtocol fp = frameProtocol.DePackage(obytes.Data);
                    byte[] data = fp.Body;
                    if (data.Length > 10)
                    {                        
                        BasePackage bp = new BasePackage();
                        bp.RemoteIpEndPoint = obytes.RemoteIpEndPoint;
                        bp.ProtocolVersion = data[0];
                        bp.CycleNo = Util.B2LInt32(new byte[] { data[1], data[2], data[3], data[4] });
                        bp.Type = data[5];
                        bp.SubType = data[6];
                        bp.ErrorStatus = data[7];
                        bp.DataLen = Util.B2LInt16(new byte[]{data[8],data[9]});
                        bp.AppData = new byte[data.Length - 10];
                        Array.Copy(data,10,bp.AppData,0,data.Length - 10);
                        //if (data.Length == bp.DataLen + 10)
                        {
                            if (Decoders.ContainsKey(bp.Type))
                            {
                                List<BaseResponse> responseList = Decoders[bp.Type].Decode(bp, obytes);
                                if (responseList != null)
                                {
                                    rxFctMsgQueue.Push(responseList);
                                    rxGeneralMsgQueue.Push(responseList);
                                    rxSelfMsgQueue.Push(responseList);
                                }
                                else
                                {
                                    LogHelper.GetLogger<ProtocolFactory>().Error(string.Format("解码错误：{0}",
                                        Summer.System.Util.ByteHelper.Byte2ReadalbeXstring(obytes.Data)));
                                }
                            }
                            else
                            {
                                LogHelper.GetLogger<ProtocolFactory>().Error(string.Format("没有解码器可以解码：{0}",
                                        Summer.System.Util.ByteHelper.Byte2ReadalbeXstring(obytes.Data)));
                            }
                        }
                        //else
                        //{
                        //    LogHelper.GetLogger<ProtocolFactory>().Error(string.Format("数据不完整，丢弃：0x{0}",
                        //                Summer.System.Util.ByteHelper.Byte2Xstring(obytes.Data)));
                        //}
                    }
                }
            }
        }

        TxQueue txQueue;
        TxMsgQueue txMsgQueue;
        //编码工厂
        public void EncodeInternal()
        {
            List<BaseRequest> list = txMsgQueue.PopAll();
            foreach (var br in list)
            {
                BasePackage bp = br.Encode();
                byte[] data = br.GetBigBytes(bp);
                OriginalBytes ob = new OriginalBytes();
                ob.RemoteIpEndPoint = bp.RemoteIpEndPoint;
                ob.Data = frameProtocol.EnPackage(data, bp.CycleNo);
                txQueue.Push(ob);
            }
        }
    }
}