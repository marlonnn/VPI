using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VPITest.Common;
using VPITest.UI;
using Summer.System.Log;
using Spring.Context.Support;
using Summer.System.Core;
using VPITest.Model;
using System.IO;
using VPITest.UI;

namespace VPITest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                FormMain formMain = SpringHelper.GetObject<FormMain>("formMain");
                //FrmMain frmMain = SpringHelper.GetObject<FrmMain>("frmMain");
                //重新载入上次的配置文件
                //ReloadLastData();
                Application.Run(formMain);
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger("Main").Error(ee.InnerException.Message);
                LogHelper.GetLogger("Main").Error(ee.InnerException.StackTrace);
            }
        }

        static void ReloadLastData()
        {
            try
            {
                //只是复制测试人、测试时长和板卡的SN配置信息（包括综合和单项）
                //只在用户设置SN号的时候存储信息
                FctTest fctTest = SpringHelper.GetObject<FctTest>("fctTest");
                GeneralTest generalTest = SpringHelper.GetObject<GeneralTest>("generalTest");
                Cabinet cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
                GlobalConfig generalGlobalConfig = SpringHelper.GetObject<GlobalConfig>("generalGlobalConfig");
                GlobalConfig fctGlobalConfig = SpringHelper.GetObject<GlobalConfig>("fctGlobalConfig");
                generalGlobalConfig.ReloadCurrent();
                fctGlobalConfig.ReloadCurrent();
                fctTest.Tester = fctGlobalConfig.TryGetString("fctTest.Tester");
                fctTest.PlanRunningTime = fctGlobalConfig.TryGetInt("fctTest.PlanRunningTime");
                generalTest.Tester = generalGlobalConfig.TryGetString("generalTest.Tester");
                generalTest.PlanRunningTime = generalGlobalConfig.TryGetInt("generalTest.PlanRunningTime");
                foreach (var r in cabinet.Racks)
                {
                    foreach (var b in r.Boards)
                    {
                        b.IsGeneralTestTested = generalGlobalConfig.TryGetBool(b.EqName + "IsGeneralTestTested");
                        b.GeneralTestSN = generalGlobalConfig.TryGetString(b.EqName + "GeneralTestSN");
                        b.FctTestSN = fctGlobalConfig.TryGetString(b.EqName + "FctTestSN");
                        foreach (var c in b.ComponentTypes)
                        {
                            c.IsFctTestTested = fctGlobalConfig.TryGetBool(b.EqName + c.EqName + "IsFctTestTested");
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger("Main").Error(ee.InnerException.Message);
            }
        }
    }
}
