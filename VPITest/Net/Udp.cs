using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using VPITest.Common;
using VPITest.UI;
using Summer.System.NET;
using Summer.System.Log;
using VPITest.Model;
using Summer.System.Util;

namespace VPITest.Net
{
    public class Udp
    {
        public int ListenPort;
        RxQueue rxQueue;
        private UdpNetServer udpNetServer;

        TxQueue txQueue;
        private Dictionary<IPEndPoint, UdpNetClient> udpNetClients;

        public Udp()
        {
        }

        //启动侦听端口并接收数据
        public void UpdRxStart()
        {
            try
            {
	            udpNetServer.AsyncRxProcessCallBack += new NetAsyncRxDataCallBack(this.ReceiveBytes);
	            udpNetServer.Open();
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                udpNetServer.ReceiveAsync(remoteIpEndPoint);
            }
            catch (System.Exception e)
            {
                LogHelper.GetLogger<Udp>().Error(e.Message);
                LogHelper.GetLogger<Udp>().Error(e.StackTrace);
            }
        }

        bool isConnected = false;
        object aLock = new object();
        public void TxInternal()
        {
            if (isConnected == false)
            {
                lock (aLock)
                {
                    if (isConnected == false)
                    {
                        try
                        {
                            foreach (var c in udpNetClients.Values)
                            {
                                c.Connect();
                            }
                            isConnected = true;
                        }
                        catch (Exception ee)
                        {
                            isConnected = false;
                        }
                    }
                }
            }
            List<Original> list = txQueue.PopAll();
            foreach (var o in list)
            {
                if (o is OriginalBytes && udpNetClients.ContainsKey(o.RemoteIpEndPoint))
                {
                    try
                    {
                        udpNetClients[o.RemoteIpEndPoint].Send((o as OriginalBytes).Data);
                    }
                    catch (Exception ee)
                    {
                    }
                }
            }
        }

        //关闭Udp
        public void UdpClose()
        {
            try
            {
                //关闭server
	            udpNetServer.AsyncRxProcessCallBack -= new NetAsyncRxDataCallBack(this.ReceiveBytes);
                //关闭客户端
                foreach (var c in udpNetClients.Values)
                {
                    c.Close();
                }
            }
            catch (System.Exception e)
            {
                LogHelper.GetLogger<Udp>().Error(e.Message);
                LogHelper.GetLogger<Udp>().Error(e.StackTrace);
            }
            finally
            {
                udpNetServer.Close();
            }
        }

        private void ReceiveBytes(byte[] receiveBytes, IPEndPoint remoteIpEndPoint)
        {
            try
            {
                if (rxQueue != null)
                {
                    rxQueue.Push(new OriginalBytes(DateTime.Now, remoteIpEndPoint, receiveBytes));
                }
                IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                udpNetServer.ReceiveAsync(remoteIp);
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<Udp>().Error(ee.Message);
                LogHelper.GetLogger<Udp>().Error(ee.StackTrace);
            }
        }
    }
}
