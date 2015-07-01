using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VPITest.Common;
using Summer.System.Log;
using VPITest.Protocol;

namespace VPITest.Model
{
    [Serializable]
    public class MessageLogFile
    {
        StreamWriter sw;
        string basePath;
        object lockFile = new object();

        public string GetFileName(string key)
        {
            return Util.GetBasePath() + basePath + key + ".log";
        }

        public void Open(string key)
        {
            try
            {
                lock (lockFile)
                {
                    sw = new StreamWriter(GetFileName(key));
                    sw.WriteLine("{0},{1},{2},{3}",
                        "时间","消息类型","消息","原始数据");
                }
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<MessageLogFile>().Error(ee.Message);
                LogHelper.GetLogger<MessageLogFile>().Error(ee.StackTrace);
            }
        }

        public void Close()
        {
            try
            {
                lock (lockFile)
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw = null;
                    }
                }
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<MessageLogFile>().Error(ee.Message);
                LogHelper.GetLogger<MessageLogFile>().Error(ee.StackTrace);
            }
        }

        public void Append(BaseResponse br)
        {
            try
            {
                lock (lockFile)
                {
                    if (sw != null)
                    {
                        if (br.OriginalBytes != null && br.OriginalBytes.Data != null)
                        {
                            sw.WriteLine("{0},{1},{2},{3}",
                                Util.FormateDateTime3(br.DtTime),
                                br.GetType().ToString(),
                                br.ToString(),
                                Summer.System.Util.ByteHelper.Byte2ReadalbeXstring(br.OriginalBytes.Data)
                            );
                        }
                        else
                        {
                            sw.WriteLine("{0},{1},{2},{3},{4}",
                                Util.FormateDateTime3(br.DtTime),
                                br.CycleNo,
                                br.GetType().ToString(),
                                br.ToString(),                                
                                ""
                            );
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<MessageLogFile>().Error(ee.Message);
                LogHelper.GetLogger<MessageLogFile>().Error(ee.StackTrace);
            }
        }

        public void Flush()
        {
            //LogHelper.GetLogger("job").Debug("Flush Job Start.");
            lock (lockFile)
            {
                if (sw != null)
                {
                    sw.Flush();
                }
            }
            //LogHelper.GetLogger("job").Debug("Flush Job Finish.");
        }
    }
}
