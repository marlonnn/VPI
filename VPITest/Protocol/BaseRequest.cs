using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPITest.Common;
using VPITest.Model;
using VPITest.Net;

namespace VPITest.Protocol
{
    /// <summary>
    /// 基本的发送消息对象
    /// </summary>
    public class BaseRequest
    {
        protected BaseRequest()
        {
        }
        /// <summary>
        /// 要发送的板卡
        /// </summary>
        public Board Board;

        public virtual BasePackage Encode()
        {
            BasePackage bp = new BasePackage();
            bp.RemoteIpEndPoint = Board.CommunicationIP;
            bp.ProtocolVersion = 0x01;
            bp.CycleNo = GetNextSequence(Board);
            bp.ErrorStatus = 0x00;
            return bp;
        }

        public byte[] GetBigBytes(BasePackage bp)
        {
            bp.DataLen = (ushort)bp.AppData.Length;

            byte[] data = new byte[bp.AppData.Length + 10];
            data[0] = bp.ProtocolVersion;
            Array.Copy(Util.L2BInt32(bp.CycleNo), 0, data, 1, 4);
            data[5] = bp.Type;
            data[6] = bp.SubType;
            data[7] = bp.ErrorStatus;
            Array.Copy(Util.L2BInt16(bp.DataLen), 0, data, 8, 2);
            Array.Copy(bp.AppData, 0, data, 10, bp.AppData.Length);

            return data;
        }

        static Dictionary<string, uint> sequences = new Dictionary<string,uint>();
        protected uint GetNextSequence(Board b)
        {
            string ip = b.CommunicationIP.Address.ToString();
            if (sequences.ContainsKey(ip))
            {
                sequences[ip]++;
            }
            else
            {
                sequences.Add(ip, 1);
            }
            return sequences[ip];
        }
    }

    public class ShakeRequest : BaseRequest
    {
        private ShakeRequest()
        {
        }
        public override BasePackage Encode()
        {
            BasePackage bp = base.Encode();            
            bp.Type = 0x01;
            bp.SubType = 0x01;
            bp.DataLen = 1;
            bp.AppData = new byte[] { 0x00 };
            return bp;
        }

        public static ShakeRequest CreateNew(Board board)
        {
            ShakeRequest sr = new ShakeRequest();
            sr.Board = board;
            return sr;
        }
    }

    public class StartFctRequest : BaseRequest
    {
        private StartFctRequest()
        {
        }
        /// <summary>
        /// 待测组件，这些组件应属于同一块板卡
        /// </summary>
        List<ComponentType> TestedComponentTypes;
        public override BasePackage Encode()
        {
            BasePackage bp = base.Encode();
            bp.Type = 0xC0;
            bp.SubType = 0x01;
            bp.AppData = new byte[5 * TestedComponentTypes.Count];
            for (int i = 0; i < TestedComponentTypes.Count; i++)
            {
                Array.Copy(TestedComponentTypes[i].EqId, 0, bp.AppData, 5 * i, 5);
            }
            bp.DataLen = (ushort)bp.AppData.Length;
            return bp;
        }

        public static StartFctRequest CreateNew(Board board, List<ComponentType> TestedComponentTypes)
        {
            StartFctRequest sr = new StartFctRequest();
            sr.Board = board;
            sr.TestedComponentTypes = TestedComponentTypes;
            return sr;
        }
    }

    public class StopFctRequest : BaseRequest
    {
        private StopFctRequest()
        {
        }

        public override BasePackage Encode()
        {
            BasePackage bp = base.Encode();
            bp.Type = 0xC1;
            bp.SubType = 0x01;
            bp.AppData = Board.EqId;
            bp.DataLen = (ushort)bp.AppData.Length;
            return bp;
        }

        public static StopFctRequest CreateNew(Board board)
        {
            StopFctRequest sr = new StopFctRequest();
            sr.Board = board;
            return sr;
        }
    }

    public class StartGeneralTestRequest : BaseRequest
    {
        private StartGeneralTestRequest()
        {
        }
        public override BasePackage Encode()
        {
            BasePackage bp = base.Encode();
            bp.Type = 0xC0;
            bp.SubType = 0x04;
            bp.AppData = new byte[5] { 0x5, 0xff, 0xff, 0xff, 0xff };
            bp.DataLen = (ushort)bp.AppData.Length;
            return bp;
        }

        public static StartGeneralTestRequest CreateNew(Board board)
        {
            StartGeneralTestRequest sr = new StartGeneralTestRequest();
            sr.Board = board;
            return sr;
        }
    }

    public class StopGeneralTestRequest : BaseRequest
    {
        private StopGeneralTestRequest()
        {
        }
        /// <summary>
        /// 待测组件，这些组件应属于同一块板卡
        /// </summary>
        public Board board;
        public override BasePackage Encode()
        {
            BasePackage bp = base.Encode();
            bp.Type = 0xC1;
            bp.SubType = 0x04;
            bp.AppData = new byte[5] { 0x5, 0xff, 0xff, 0xff, 0xff };
            bp.DataLen = (ushort)bp.AppData.Length;
            return bp;
        }

        public static StopGeneralTestRequest CreateNew(Board board)
        {
            StopGeneralTestRequest sr = new StopGeneralTestRequest();
            sr.Board = board;
            return sr;
        }
    }
}
