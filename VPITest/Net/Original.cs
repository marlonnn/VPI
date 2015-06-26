using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace VPITest.Net
{
    public class Original
    {
        public DateTime RxTime { get; set; }

        public IPEndPoint RemoteIpEndPoint { get; set; }
    }

    public class OriginalBytes : Original
    {
        public byte[] Data { get; set; }

        public OriginalBytes()
        {
        }

        public OriginalBytes(DateTime dt, IPEndPoint ip, byte[] msg)
        {
            RxTime = dt;
            Data = msg;
            RemoteIpEndPoint = ip;
        }
    }

    public struct BasePackage
    {
        public IPEndPoint RemoteIpEndPoint;
        /// <summary>
        /// 协议版本
        /// </summary>
        public byte ProtocolVersion;

        /// <summary>
        /// 上位机发送的序列号
        /// </summary>
        public uint CycleNo;

        /// <summary>
        /// 类型
        /// </summary>
        public byte Type;

        /// <summary>
        /// 子类型
        /// </summary>
        public byte SubType;

        /// <summary>
        /// 错误标识符
        /// </summary>
        public byte ErrorStatus;

        /// <summary>
        /// 应用实际数据长度
        /// </summary>
        public ushort DataLen;

        /// <summary>
        /// 应用实际数据
        /// </summary>
        public byte[] AppData;
    }
}
