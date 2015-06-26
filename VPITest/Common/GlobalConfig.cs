using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Summer.System.Log;

namespace VPITest.Common
{
    [Serializable]
    public class GlobalConfig
    {
        private string basePath;

        public Dictionary<string, string> Configs = new Dictionary<string,string>();

        public void TryPut(string cfgName, string value)
        {
            if (Configs.ContainsKey(cfgName))
                Configs[cfgName] = value;
            else
                Configs.Add(cfgName, value);
        }

        public void TryPut(string cfgName, long value)
        {
            TryPut(cfgName,string.Format("{0}",value));
        }

        public void TryPut(string cfgName, bool value)
        {
            TryPut(cfgName, value ? "true" : "false");
        }

        public string TryGetString(string cfgName)
        {
            if (Configs.ContainsKey(cfgName))
                return Configs[cfgName];
            else
                return "";
        }

        public int TryGetInt(string cfgName)
        {
            string data = TryGetString(cfgName);
            if (data.Length>0)
            {
                try
                {
                    return int.Parse(data);
                }
                catch
                {
                }
            }
            return 0;
        }

        public bool TryGetBool(string cfgName)
        {
            string data = TryGetString(cfgName);
            return (data == "true");
        }

        public void SaveCurrent()
        {
            string pathAndFilename = Util.GetBasePath() + basePath + "current.global";
            //序列化用户当前的配置
            System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(pathAndFilename, FileMode.OpenOrCreate, FileAccess.Write);
            using (stream)
            {
                formatter.Serialize(stream, this);
            }
            //将当前的重命名为上次的数据
            SaveLast();
        }

        private void SaveLast()
        {
            System.IO.FileInfo file = new System.IO.FileInfo(Util.GetBasePath()  + basePath + "current.global");
            if (file.Exists)
            {
                file.CopyTo(Util.GetBasePath() + basePath + "last.global", true);
            }
        }

        public void ReloadCurrent()
        {
            string pathAndFilename = Util.GetBasePath() + basePath + "current.global";
            try
            {
                System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(pathAndFilename, FileMode.Open, FileAccess.Read);
                GlobalConfig gc;
                using (stream)
                {
                    gc = (GlobalConfig)formatter.Deserialize(stream);
                }
                if (gc != null)
                {
                    this.Configs.Clear();
                    this.Configs = new Dictionary<string, string>(gc.Configs);
                }
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<GlobalConfig>().Error(ee.Message);
                LogHelper.GetLogger<GlobalConfig>().Error(ee.StackTrace);
            }
        }

        public GlobalConfig ReloadLast()
        {
            string pathAndFilename = Util.GetBasePath() + basePath + "last.global";
            try
            {
                System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(pathAndFilename, FileMode.Open, FileAccess.Read);
                GlobalConfig gc;
                using (stream)
                {
                    gc = (GlobalConfig)formatter.Deserialize(stream);
                }
                if (gc != null)
                {
                    return gc;
                }
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<GlobalConfig>().Error(ee.Message);
                LogHelper.GetLogger<GlobalConfig>().Error(ee.StackTrace);
            }
            return null;
        }
    }
}
