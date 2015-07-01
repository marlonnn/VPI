using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Common;
using VPITest.Model;
using VPITest.Net;
using Summer.System.Core;

namespace VPITest.Protocol
{
    /// <summary>
    /// 基本的接收消息对象
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// 消息收到的时间
        /// </summary>
        public DateTime DtTime;
        /// <summary>
        /// 消息来自的板卡
        /// </summary>
        public Board CommunicatinBoard;

        /// <summary>
        /// 原始数据
        /// </summary>
        public OriginalBytes OriginalBytes;

        public uint CycleNo;

        public virtual List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            CycleNo = bp.CycleNo;
            return null;
        }

        protected List<BaseResponse> CreateOneList(BaseResponse br)
        {
            List<BaseResponse> list = new List<BaseResponse>();
            list.Add(br);
            return list;
        }
    }

    public class ShakeResponse : BaseResponse
    {
        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            Board b = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint);
            if (b != null)
            {
                ShakeResponse sr = new ShakeResponse();
                sr.CommunicatinBoard = b;
                sr.DtTime = DateTime.Now;
                sr.OriginalBytes = obytes;
                return CreateOneList(sr);
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("收到来自{0}板卡的握手反馈消息。", CommunicatinBoard.EqName);
        }
    }

    public class HeartMsg : BaseResponse
    {
        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            base.Decode(bp, obytes);
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            Board b = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint);
            if (b != null)
            {
                HeartMsg sr = new HeartMsg();
                sr.CommunicatinBoard = b;
                sr.DtTime = DateTime.Now;
                sr.OriginalBytes = obytes;
                return CreateOneList(sr);
            }
            return null;
        }
    }

    public class HeartTimeoutMsg : BaseResponse
    {
        /// <summary>
        /// 自己生成的，下位机不会送达，所以没有解码方法
        /// </summary>
        /// <param name="bp"></param>
        /// <returns></returns>
        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}板卡的心跳超时。", CommunicatinBoard.EqName);
        }
    }

    public class ComponentTestResponse : BaseResponse
    {
        public Component Component;

        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            base.Decode(bp, obytes);
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            List<BaseResponse> list = new List<BaseResponse>();
            //应用区数据格式：[EquipId（COMPONENT）(5)+testtimes（4）+误码次数（4）+丢包次数（4）+通信终端次数（4）]*
            if (bp.AppData.Length % 21 == 0)
            {
                for (int i = 0; i < bp.AppData.Length / 21; i++)
                {
                    byte[] eqid = new byte[] { bp.AppData[i * 21 + 0], bp.AppData[i * 21 + 1], bp.AppData[i * 21 + 2], bp.AppData[i * 21 + 3], bp.AppData[i * 21 + 4] };
                    int allTimes = (int)Util.B2LInt32(new byte[] { bp.AppData[i * 21 + 5], bp.AppData[i * 21 + 6], bp.AppData[i * 21 + 7], bp.AppData[i * 21 + 8] });
                    int errorPackageTimes = (int)Util.B2LInt32(new byte[] { bp.AppData[i * 21 + 9], bp.AppData[i * 21 + 10], bp.AppData[i * 21 + 11], bp.AppData[i * 21 + 12] });
                    int lostPackageTimes = (int)Util.B2LInt32(new byte[] { bp.AppData[i * 21 + 13], bp.AppData[i * 21 + 14], bp.AppData[i * 21 + 15], bp.AppData[i * 21 + 16] });
                    int interruptTimes = (int)Util.B2LInt32(new byte[] { bp.AppData[i * 21 + 17], bp.AppData[i * 21 + 18], bp.AppData[i * 21 + 19], bp.AppData[i * 21 + 20] });
                    AbstractEq eq = cabinet.FindEq(eqid);
                    if (eq != null && eq is Component)
                    {
                        ComponentTestResponse cr = new ComponentTestResponse();
                        cr.CommunicatinBoard = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint);
                        cr.DtTime = DateTime.Now;
                        cr.OriginalBytes = obytes;
                        cr.Component = eq as Component;
                        cr.Component.AllTestTimes = allTimes;
                        cr.Component.ErrorPackageTimes = errorPackageTimes;
                        cr.Component.LostPackageTimes = lostPackageTimes;
                        cr.Component.InterruptTimes = interruptTimes;
                        list.Add(cr);
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return string.Format("{0}板卡{1}组件的测试次数为{2}，误码次数为{3}，丢包次数为{4}，中断次数为{5}。",
                CommunicatinBoard.EqName, Component.EqName, Component.AllTestTimes,
                Component.ErrorPackageTimes, Component.LostPackageTimes, Component.InterruptTimes);
        }
    }

    /// <summary>
    /// 将ComponentTestResponse消息进行过滤后的组包消息，只有错误计数增加时才生成此消息
    /// </summary>
    public class FiltedComponentTestResponse : BaseResponse
    {
        private FiltedComponentTestResponse(Component component)
        {
            this.CommunicatinBoard = component.ParentComponentType.ParentBoard;
            this.Component = component;
            this.DtTime = DateTime.Now;
        }

        public static FiltedComponentTestResponse CreateNew(Component component)
        {
            return new FiltedComponentTestResponse(component);
        }

        public Component Component;
        /// <summary>
        /// 系统内部生成的，非解码出来的的消息
        /// </summary>
        /// <param name="bp"></param>
        /// <returns></returns>
        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}板卡{1}组件的测试次数为{2}，误码次数为{3}，丢包次数为{4}，中断次数为{5}。",
                CommunicatinBoard.EqName, Component.EqName, Component.AllTestTimes,
                Component.ErrorPackageTimes, Component.LostPackageTimes, Component.InterruptTimes);
        }
    }

    public class StopFctTestResponse : BaseResponse
    {
        public List<Component> Components;

        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            //应用区数据格式：[EquipId（COMPONENT）(5)]*
            StopFctTestResponse sr = new StopFctTestResponse();
            sr.Components = new List<Component>();
            if (bp.AppData.Length % 5 == 0)
            {
                for (int i = 0; i < bp.AppData.Length / 5; i++)
                {
                    byte[] eqid = new byte[] { bp.AppData[i * 5 + 0], bp.AppData[i * 5 + 1], bp.AppData[i * 5 + 2], bp.AppData[i * 5 + 3], bp.AppData[i * 5 + 4] };
                    AbstractEq eq = cabinet.FindEq(eqid);
                    if (eq is Component)
                    {
                        sr.CommunicatinBoard = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint);
                        sr.DtTime = DateTime.Now;
                        sr.OriginalBytes = obytes;
                        sr.Components.Add(eq as Component);
                    }
                }
            }
            return CreateOneList(sr);
        }

        public override string ToString()
        {
            return string.Format("收到来自{0}板卡的测试停止响应消息。", CommunicatinBoard.EqName);
        }
    }

    public class VPSErrorInfoResponse : BaseResponse
    {
        public Board ErrorBoard;
        public int ErrorCode;

        /// <summary>
        /// [EquipId(BOARD)(5)+errorCode(1)]
        /// </summary>
        /// <param name="bp"></param>
        /// <param name="obytes"></param>
        /// <returns></returns>
        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            if (bp.AppData.Length >= 6)
            {
                VPSErrorInfoResponse tr = new VPSErrorInfoResponse();
                tr.CommunicatinBoard = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint); 
                tr.DtTime = DateTime.Now;
                tr.OriginalBytes = obytes;
                byte[] eqid = new byte[] { bp.AppData[0], bp.AppData[1], bp.AppData[2], bp.AppData[3], bp.AppData[4] };
                tr.ErrorBoard = cabinet.FindEq(eqid) as Board;
                tr.ErrorCode = bp.AppData[5];
                return CreateOneList(tr);
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}板卡上报状态信息，状态码：0x{1:X2}。", ErrorBoard.EqName, ErrorCode);
        }
    }

    public class FiltedVPSErrorInfoResponse : VPSErrorInfoResponse
    {
        private FiltedVPSErrorInfoResponse()
        {
        }
        public static FiltedVPSErrorInfoResponse CreateNew(VPSErrorInfoResponse vpsRes)
        {
            FiltedVPSErrorInfoResponse fr = new FiltedVPSErrorInfoResponse();
            fr.CommunicatinBoard = vpsRes.CommunicatinBoard;
            fr.DtTime = vpsRes.DtTime;
            fr.ErrorBoard = vpsRes.ErrorBoard;
            fr.ErrorCode = vpsRes.ErrorCode;
            fr.OriginalBytes = vpsRes.OriginalBytes;
            return fr;
        }
    }

    public class BoardStatusResponse : BaseResponse
    {
        public Board Board;
        public int ErrorCode;

        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            if (bp.AppData.Length == 48)//应用数据(48)
            {
                List<BaseResponse> list = new List<BaseResponse>();
                for (byte rackNo = 2; rackNo <= 5; rackNo++)//除系统机笼
                {
                    for (byte slotNo = 3; slotNo <= 14; slotNo++)
                    {
                        BoardStatusResponse boardStatusResponse = new BoardStatusResponse();
                        boardStatusResponse.CommunicatinBoard = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint);
                        boardStatusResponse.DtTime = DateTime.Now;
                        boardStatusResponse.OriginalBytes = obytes;
                        byte[] eqid = new byte[] { 0x05, rackNo, slotNo, 0xFF, 0xFF };
                        Board b = cabinet.FindEq(eqid) as Board;
                        if (b != null)
                        {
                            boardStatusResponse.Board = b;
                            boardStatusResponse.ErrorCode = bp.AppData[(rackNo - 2) * 12 + (slotNo - 3)];
                            list.Add(boardStatusResponse);
                        }
                    }
                }
                return list;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}板卡状态信息0x{2:X2}。", Board.ParentRack.EqName, Board.EqName, ErrorCode);
        }
    }

    public class FiltedBoardStatusResponse : BoardStatusResponse
    {
        private FiltedBoardStatusResponse()
        {
        }

        public static FiltedBoardStatusResponse CreateNew(BoardStatusResponse br)
        {
            FiltedBoardStatusResponse fr = new FiltedBoardStatusResponse();
            fr.Board = br.Board;
            fr.CommunicatinBoard = br.CommunicatinBoard;
            fr.DtTime = br.DtTime;
            fr.ErrorCode = br.ErrorCode;
            fr.OriginalBytes = br.OriginalBytes;
            return fr;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}板卡被拔出，错误码为：0x{2:X2}。", Board.ParentRack.EqName, Board.EqName, ErrorCode);
        }
    }

    /// <summary>
    /// 此类解码出来的数据变成VIBTestResponse或者VOBTestResponse，不会在消息队列中出现VIBVOBTestResponse
    /// </summary>
    public class VIBVOBTestResponse : BaseResponse
    {
        public Board Board;

        public int LightPos;

        public UInt32 ExpectedCode;

        public UInt32 RealCode;

        public int ErrorTimes;

        public int smallCycleBoard;//小周期号

        public bool OCK;//OCK：true OCK错误

        public bool OutPut;//是否有输出

        public bool ErrorCode;//ErrorCode:true 测试字错误

        public override List<BaseResponse> Decode(BasePackage bp, OriginalBytes obytes)
        {
            base.Decode(bp, obytes);
            Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            List<BaseResponse> list = new List<BaseResponse>();
            Board communicatinBoard = cabinet.GetCommunicationBoard(bp.RemoteIpEndPoint);
            //应用区数据格式：rack(1)+slot(1)+boardtype(1)+smallcycle(1)+lightport(2) +[采集到的码字（4）*16] +[错误次数（4）*16]
            if (bp.AppData.Length == 6 + 4 * 16 + 4 * 16)
            {
                byte rackNo = bp.AppData[0];
                byte slotNo = bp.AppData[1];
                byte boardType = bp.AppData[2];
                byte smallCycle = bp.AppData[3];
                UInt32[] realCodes = new UInt32[16];
                Int32[] errorTimes = new Int32[16];
                for (int i = 0; i < 16; i++)
                {
                    realCodes[i] = System.BitConverter.ToUInt32(bp.AppData, 6 + i * 4);
                    errorTimes[i] = System.BitConverter.ToInt32(bp.AppData, 6 + 64 + i * 4);
                }
                
                int a = (int)(((bp.AppData[4] << 8) & 0xFF00) | (bp.AppData[5] & 0x00FF));
                byte[] eqId = new byte[] { 0x05, rackNo, slotNo, 0xFF, 0xFF };
                //byte[] eqId = GetEqId(rackNo, slotNo);
                for (int i = 0; i < 16; ++i)
                {
                    if (((a >> i) & 0x01) == 0x01)//error
                    {
                        //例如 02 04 02 05 01 00 
                        //表示 1 机笼 4槽道 VOB16板卡 01 00 ->0000 0001 0000 0000 : 第9个灯位有问题
                        int posk = (rackNo - 2) * 12 * 16 + (slotNo - 2) * 16 + i;
                        if (boardType == 0x01)//VIB
                        {
                            VIBTestResponse vibR = new VIBTestResponse();
                            vibR.CommunicatinBoard = communicatinBoard;
                            vibR.DtTime = DateTime.Now;
                            vibR.OriginalBytes = obytes;
                            vibR.Board = cabinet.FindEq(eqId) as Board;
                            vibR.Board.BoardType = "VIB";
                            vibR.LightPos = i + 1;
                            vibR.ExpectedCode = Util.vib_true[posk];
                            vibR.RealCode = realCodes[i];
                            vibR.ErrorTimes = errorTimes[i];
                            int readableSmallCycle = (int)smallCycle + 1;
                            vibR.smallCycleBoard = readableSmallCycle;
                            list.Add(vibR);
                        }
                        else if (boardType == 0x02)//VOB
                        {
                            VOBTestResponse vobR = new VOBTestResponse();
                            vobR.CommunicatinBoard = communicatinBoard;
                            vobR.DtTime = DateTime.Now;
                            vobR.OriginalBytes = obytes;
                            vobR.Board = cabinet.FindEq(eqId) as Board;
                            vobR.Board.BoardType = "VOB";
                            vobR.LightPos = i + 1;
                            vobR.RealCode = realCodes[i];
                            vobR.ErrorTimes = errorTimes[i];

                            byte[] cycleNo = new byte[2];
                            Array.Copy(obytes.Data, 3, cycleNo, 0, 2);//帧协议中周期号四个字节中的低两位作为是否有输出
                            uint value = (uint)(((cycleNo[0] << 8) & 0xFF00) | (cycleNo[1] & 0x00FF));
                            int readableSmallCycle = (int)smallCycle;
                            vobR.smallCycleBoard = readableSmallCycle;
                            if (((value >> i) & 0x01) == 0x01)//有输出
                            {
                                //VOB板卡第i个灯位错误，且当前灯位亮的状态(有输出)
                                vobR.OutPut = true;
                                vobR.ErrorCode = false;
                                if (readableSmallCycle == 2)
                                {
                                    vobR.ExpectedCode = Util.vob_ock_even[posk];
                                    vobR.OCK = true;
                                }
                                else if (readableSmallCycle == 0 || readableSmallCycle == 4 ||
                                    readableSmallCycle == 6 || readableSmallCycle == 8)
                                {
                                    vobR.ExpectedCode = Util.vob_true_even;
                                    vobR.OCK = false;
                                }
                                else if (readableSmallCycle == 1)
                                {
                                    vobR.ExpectedCode = Util.vob_ock_odd[posk];
                                    vobR.OCK = true;
                                }
                                else if (readableSmallCycle == 3 || readableSmallCycle == 5 ||
                                    readableSmallCycle == 7 || readableSmallCycle == 9)
                                {
                                    vobR.ExpectedCode = Util.vob_true_odd;
                                    vobR.OCK = false;
                                }
                            }
                            else
                            {
                                //当前的灯位灭的状态（无输出）
                                vobR.OutPut = false;
                                vobR.ErrorCode = true;
                                if (readableSmallCycle == 0 || readableSmallCycle == 2 ||
                                    readableSmallCycle == 4 || readableSmallCycle == 6 ||
                                    readableSmallCycle == 8)
                                {
                                    vobR.ExpectedCode = Util.vob_ckWord_even[posk];
                                }
                                else if (readableSmallCycle == 1 || readableSmallCycle == 3 ||
                                    readableSmallCycle == 5 || readableSmallCycle == 7 ||
                                    readableSmallCycle == 9)
                                {
                                    vobR.ExpectedCode = Util.vob_ckWord_odd[posk];
                                }
                            }
                            list.Add(vobR);
                        }
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            if (Board.BoardType.Contains("VOB"))
            {
                string outstring;
                string ockstring;
                outstring = OutPut ? "有输出" : "无输出";
                if(ErrorCode)
                {
                    ockstring = "测试字错误";
                }
                else 
                {
                    ockstring = OCK ? "OCK错误" : "未驱动错";
                }
                return string.Format("{0}板卡端口{1}报错，周期号为{2}，输出状态为{3}，错误次数为{4}，错误类型为{5}，期望码字为0x{6:X8}，实际码字为0x{7:X8}",
                    Board.EqName, LightPos, smallCycleBoard, outstring, ErrorTimes, ockstring, ExpectedCode, RealCode);
            }
            else
            {
                return string.Format("{0}板卡端口{1}报错，错误次数为{2}，期望码字为0x{3:X8}，实际码字为0x{4:X8}",
                    Board.EqName, LightPos, ErrorTimes, ExpectedCode, RealCode);
            }

        }
    }

    public class VIBTestResponse : VIBVOBTestResponse
    {
    }

    public class VOBTestResponse : VIBVOBTestResponse
    {
    }

    public class FiltedVIBTestResponse : VIBTestResponse
    {
        public FiltedVIBTestResponse(VIBTestResponse vib)
            : base()
        {
            this.CommunicatinBoard = vib.CommunicatinBoard;
            this.DtTime = vib.DtTime;
            this.OriginalBytes = vib.OriginalBytes;
            this.Board = vib.Board;
            this.smallCycleBoard = vib.smallCycleBoard;
            this.LightPos = vib.LightPos;
            this.ExpectedCode = vib.ExpectedCode;
            this.RealCode = vib.RealCode;
            this.ErrorTimes = vib.ErrorTimes;
        }
    }

    public class FiltedVOBTestResponse : VOBTestResponse
    {
        public FiltedVOBTestResponse(VOBTestResponse vob)
            : base()
        {
            this.CommunicatinBoard = vob.CommunicatinBoard;
            this.DtTime = vob.DtTime;
            this.OriginalBytes = vob.OriginalBytes;
            this.Board = vob.Board;
            this.smallCycleBoard = vob.smallCycleBoard;
            this.OCK = vob.OCK;
            this.OutPut = vob.OutPut;
            this.ErrorCode = vob.ErrorCode;
            this.LightPos = vob.LightPos;
            this.ExpectedCode = vob.ExpectedCode;
            this.RealCode = vob.RealCode;
            this.ErrorTimes = vob.ErrorTimes;
        }
    }
}
