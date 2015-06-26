using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Log;
using VPITest.Common;

namespace VPITest.Net
{
    public class FrameProtocol
    {
        /// <summary>
        /// 应用标志位
        /// </summary>
        private byte appMarkHead;//0X5A
        private byte appMarkTail;//0XA5
        public byte[] CycleNo;
        public byte[] Body;
        /// <summary>
        /// 将数据解包（第一个字节是标识符，最后第二个字节是奇偶校验，最后一个字节是结尾标识符）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public FrameProtocol DePackage(byte[] data)
        {
            FrameProtocol fp = new FrameProtocol();
            if (data == null || data.Length < 2)
            {
                LogHelper.GetLogger<FrameProtocol>().Error("通信层接收到数据包为空或者数据长度不足，丢弃。");
                return null;
            }
            if (data[0] != appMarkHead && data[data.Length - 1] != appMarkTail)
            {
                LogHelper.GetLogger<FrameProtocol>().Error("通信层接收到数据包不是本应用需要接受的数据包，丢弃。");
                return null;
            }

            //数据正常，去掉头尾返回，此处未考虑下位机发过来的数据分成N（N>1）包的情况，后续补充
            fp.appMarkHead = data[0];
            fp.appMarkTail = data[data.Length - 1];
            fp.CycleNo = new byte[4];
            Array.Copy(data, 1, fp.CycleNo, 0, 4);
            fp.Body = new byte[data.Length - 9];
            Array.Copy(data, 7, fp.Body, 0, data.Length - 9);
            return fp;
        }

        /// <summary>
        /// 将数据加包，包首增加应用标志，包尾增加奇偶校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] EnPackage(byte[] data, uint cycle)
        {
            if (data == null)
            {
                LogHelper.GetLogger<FrameProtocol>().Error("通信层待编码数据为空，丢弃。");
                return null;
            }
            byte[] enData = new byte[data.Length + 9];
            enData[0] = appMarkHead;//frame head
            Array.Copy(Util.L2BInt32(cycle), 0, enData, 1, 4);
            enData[5] = 1;//分包总数
            enData[6] = 1;//分包序列号
            Array.Copy(data, 0, enData, 7, data.Length);  //把需要发送的数据复制到帧数据体中
            //计算奇偶校验和，最后2位不参与奇偶校验
            byte oddCheck = enData[0];
            for (int i = 1; i < enData.Length - 2; i++)
            {
                oddCheck ^= enData[i];
            }
            enData[enData.Length - 2] = oddCheck;//奇偶校验位
            enData[enData.Length - 1] = appMarkTail;//frame tail
            return enData;
        }
    }
}
