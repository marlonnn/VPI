using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using System.Reflection;
using iTextSharp.text;
using System.IO;
using System.Windows.Forms;
using Summer.System.Log;
using VPITest.Common;
using VPITest.DB;

namespace VPITest.Model
{
    //文件存储结构：
    //Report根目录放pdf报告
    //Report目录下的Data放TestRacks二进制文件和日志文件
    //这三种文件使用同一个文件名，后缀分别是pdf trs log
    public class Report
    {
        VPITest.Common.Version version;

        string fctTempletFile;
        string generalTempletFile;

        string fctUserReportTitle;
        string fctDiagReportTitle;
        string generalUserReportTitle;
        string generalDiagReportTitle;
        string fontFile;
        float fontSizeHead;
        float fontSizeBody;
        string endString;

        MessageLogFile fctMessageLogFile;
        MessageLogFile generalMessageLogFile;

        public static void ExploreReportFolder()
        {
            System.Diagnostics.Process.Start(Util.GetBasePath() + "//Report");
        }

        public static void ExploreFctReportFolder()
        {
            System.Diagnostics.Process.Start(Util.GetBasePath() + "//Report//Component");
        }

        public static void ExploreGeneralReportFolder()
        {
            System.Diagnostics.Process.Start(Util.GetBasePath() + "//Report//SingleBoard");
        }

        public static void ExploreSelfDiagReportFolder()
        {
            System.Diagnostics.Process.Start(Util.GetBasePath() + "//Data//Self");
        }

        public static string GetUsrFctPdfName(string finishReason,string key, string SN, bool isComponentPass)
        {
            string pdfFile;
            if (isComponentPass)
            {
                pdfFile = string.Format("{0}//Report//Component//Passboard//{1}_{2}.pdf",
                    Util.GetBasePath(), SN, key);
            }
            else
            {
                if (finishReason == DbADO.TEST_FINISH_RESULT_NORMAL) 
                {
                    pdfFile = string.Format("{0}//Report//Component//Failboard//{1}_{2}.pdf",
                        Util.GetBasePath(), SN, key);
                }
                else 
                {
                    pdfFile = string.Format("{0}//Report//Exception//{1}_{2}.pdf",
                        Util.GetBasePath(), SN, key);
                }
            }
            return pdfFile;
        }

        public static string GetDiagnoseFctPdfName(string key)
        {
            string pdfFile = pdfFile = string.Format("{0}//Data//Component//{1}.pdf", Util.GetBasePath(), key); 
            return pdfFile;
        }

        public static string GetUsrGeneralPdfName(string finishReason,string key, string SN, bool isBoardPass)
        {
            string pdfFile;
            if (isBoardPass)
            {
                pdfFile = string.Format("{0}//Report//SingleBoard//Passboard//{1}_{2}.pdf",
                    Util.GetBasePath(), SN, key);
            }
            else
            {
                if(finishReason == DbADO.TEST_FINISH_RESULT_NORMAL)
                {
                    pdfFile = string.Format("{0}//Report//SingleBoard//Failboard//{1}_{2}.pdf",
                        Util.GetBasePath(), SN, key);
                }
                else
                {
                    pdfFile = string.Format("{0}//Report//Exception//{1}_{2}.pdf",
                        Util.GetBasePath(), SN, key);
                }
            }
            return pdfFile;
        }

        public static string GetDiagnoseGeneralPdfName(string key)
        {
            string pdfFile = pdfFile = string.Format("{0}//Data//SingleBoard//{1}.pdf", Util.GetBasePath(), key);
            return pdfFile;
        }

        /// <summary>
        /// FctTest报告生成共包含三种文件：
        /// 1、Pdf报告文件，每个被测试组件生成一份，通过的放在Report//Fct//Passboard，未通过的放在Report//Fct//FailBoard下面，
        /// 命名格式为SN_key.pdf；
        /// 2、Pdf诊断文件，每次测试生成一份，通过的放在Data//Fct，命名格式为key.pdf；
        /// 3、fct二进制文件，保存在Data//Fct目录下，命名格式为 SN_key.fct；
        /// 4、中间的日志文件保存在Data//Fct目录下，命名格式为 SN_key.log；
        /// 本函数用于第一种文件和第二种文件的生成
        /// </summary>
        /// <param name="tr"></param>
        public void GenerateUserPdf(FctTest fctTest)
        {
            //生成用户报告
            Dictionary<Board, List<ComponentType>> dicts = fctTest.Cabinet.GetFctTestedComponentTypesDicts();
            List<Board> boards = new List<Board>();
            List<ComponentType> componentTypes = new List<ComponentType>();
            foreach (var b in dicts.Keys)
            {
                List<Board> tmpBoardList = new List<Board>();
                tmpBoardList.Add(b);
                boards.Add(b);
                componentTypes.AddRange(dicts[b]);
                GenerateFctPdf(fctTest, tmpBoardList, dicts[b], true);
            }
            //生成诊断报告(诊断报告不包含log文件，因为log文件比较大，包含后压缩时间太长)
            //GenerateFctPdf(fctTest, boards, componentTypes, fctMessageLogFile.GetFileName(fctTest.Key), false);
            GenerateFctPdf(fctTest, boards, componentTypes, false);
        }

        private void GenerateFctPdf(FctTest fctTest, List<Board> boards, List<ComponentType> componentTypes, bool isUserReport)
        {
            BaseFont baseFont;
            try
            {
                baseFont = BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                PdfReader rdr = new PdfReader(Util.GetBasePath() + fctTempletFile);
                PdfStamper stamper;
                string pdfFile;
                if (isUserReport)
                    pdfFile = GetUsrFctPdfName(fctTest.FinishReason,fctTest.Key, boards[0].FctTestSN, componentTypes[0].IsFctTestPassed());
                else
                    pdfFile = GetDiagnoseFctPdfName(fctTest.Key);

                stamper = new PdfStamper(rdr, new System.IO.FileStream(pdfFile, System.IO.FileMode.Create));
                stamper.AcroFields.AddSubstitutionFont(baseFont);

                if (isUserReport)
                {
                    SetHeadFieldValue(stamper, "pageHead", fctUserReportTitle);
                    SetFieldValue(stamper, "Head", fctUserReportTitle);
                }
                else
                {
                    if (fctMessageLogFile.GetFileName(fctTest.Key) != null)
                        SetFieldValue(stamper, "pageHead", Util.GetMD5(fctMessageLogFile.GetFileName(fctTest.Key)));
                    else
                        SetFieldValue(stamper, "pageHead", fctDiagReportTitle);
                    SetHeadFieldValue(stamper, "Head", fctDiagReportTitle);
                    
                }
                stamper.AcroFields.SetFieldProperty("Head", "textsize", 20.0f, null);
                stamper.AcroFields.SetFieldProperty("Head", "textsize", 20.0f, null);

                SetHeadFieldValue(stamper, "ver", string.Format("{0} Build:{1}", version.Ver, version.Build));
                SetHeadFieldValue(stamper, "data", fctTest.Key);
                SetHeadFieldValue(stamper, "gDate", Util.FormateDateTime2(DateTime.Now));

                SetFieldValue(stamper, "tester", fctTest.Tester);
                SetFieldValue(stamper, "startTime", Util.FormateDateTime(fctTest.StartTime));
                SetFieldValue(stamper, "planRunningTime",
                        string.Format("{0}/{1}", Util.FormateDurationSecondsMaxHour(fctTest.PlanRunningTime),
                        Util.FormateDurationSecondsMaxHour(fctTest.RunningTime)));
                SetFieldValue(stamper, "finishReason", fctTest.FinishReason);

                string names = "";
                string sns = "";
                string passes = "";
                bool isPass = true;

                string componentName = "";
                string allTestTimes = "";
                string errorPackageTimes = "";
                string lostPackageTimes = "";
                string interruptTimes = "";
                for (int i = 0; i < boards.Count; i++)
                {
                    names += boards[i].EqName + "\n";
                    if (boards[i].FctTestSN != "")
                        sns += boards[i].FctTestSN + "\n";
                    else
                        sns += "非待测" + "\n";
                    passes += boards[i].IsFctTestPassed() ? "PASS\n" : "FAIL\n";
                    isPass &= componentTypes[i].IsFctTestPassed();
                }
                names += endString;
                sns += endString;
                passes += endString;

                foreach (ComponentType ct in componentTypes)
                {
                    if (ct.IsFctTestTested)
                    {
                        foreach (Component cp in ct.Components)
                        {
                            if (isUserReport)
                            {
                                componentName += cp.EqName + "\n";
                            }
                            else
                            {
                                componentName += cp.ParentComponentType.ParentBoard.EqName + "-" + cp.EqName + "\n";
                            }
                            allTestTimes += cp.AllTestTimes.ToString() + "\n";
                            errorPackageTimes += cp.ErrorPackageTimes.ToString() + "\n";
                            lostPackageTimes += cp.LostPackageTimes.ToString() + "\n";
                            interruptTimes += cp.InterruptTimes.ToString() + "\n";
                        }
                    }
                }
                componentName += endString;
                allTestTimes += endString;
                errorPackageTimes += endString;
                lostPackageTimes += endString;
                interruptTimes += endString;

                SetFieldValue(stamper, "boardName", names);
                SetFieldValue(stamper, "boardSn", sns);
                SetFieldValue(stamper, "isItemPass", passes);

                SetFieldValue(stamper, "componentName", componentName);
                SetFieldValue(stamper, "allTestTimes", allTestTimes);
                SetFieldValue(stamper, "errorPackageTimes", errorPackageTimes);
                SetFieldValue(stamper, "lostPackageTimes", lostPackageTimes);
                SetFieldValue(stamper, "interruptTimes", interruptTimes);

                if (isPass)
                {
                    SetFieldValue(stamper, "IsPass", "PASS");
                    stamper.AcroFields.SetFieldProperty("IsPass", "textsize", 38.0f, null);
                    BaseColor bc = new BaseColor(00, 64, 00);//DarkGreen
                    stamper.AcroFields.SetFieldProperty("IsPass", "textcolor", bc, null);
                }
                else
                {
                    SetFieldValue(stamper, "IsPass", "FAIL");
                    stamper.AcroFields.SetFieldProperty("IsPass", "textsize", 38.0f, null);
                    stamper.AcroFields.SetFieldProperty("IsPass", "textcolor", BaseColor.RED, null);
                }
                stamper.FormFlattening = true;//不允许编辑   
                stamper.Close();
                rdr.Close();
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<Report>().Error(ee.Message);
                LogHelper.GetLogger<Report>().Error(ee.StackTrace);
                //MessageBox.Show(ee.Message);
            }
        }

        public void GenerateUserPdf(GeneralTest generalTest)
        {
            //生成用户报告
            List<Board> boards = generalTest.Cabinet.GetGeneralTestBoardsList();
            List<Board> allboards = generalTest.Cabinet.GetAllGeneralTestBoardsList();
            foreach (var b in boards)
            {
                List<Board> tmpBoards = new List<Board>();
                tmpBoards.Add(b);
                GenerateGeneralPdf(generalTest, tmpBoards,  true);
            }
            //生成诊断报告(诊断报告不包含log文件，因为log文件比较大，包含后压缩时间太长)
            //GenerateGeneralPdf(generalTest, boards, generalMessageLogFile.GetFileName(generalTest.Key), false);
            GenerateGeneralPdf(generalTest, allboards,  false);
        }

        private void GenerateGeneralPdf(GeneralTest generalTest, List<Board> boards, bool isUserReport)
        {
            BaseFont baseFont;
            try
            {
                baseFont = BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                PdfReader rdr = new PdfReader(Util.GetBasePath() + generalTempletFile);
                PdfStamper stamper;
                string pdfFile;
                if (isUserReport)
                {
                    pdfFile = GetUsrGeneralPdfName(generalTest.FinishReason,generalTest.Key, boards[0].GeneralTestSN, boards[0].IsGeneralTestPassed);
                }
                else
                    pdfFile = GetDiagnoseGeneralPdfName(generalTest.Key);

                stamper = new PdfStamper(rdr, new System.IO.FileStream(pdfFile, System.IO.FileMode.Create));
                stamper.AcroFields.AddSubstitutionFont(baseFont);

                if (isUserReport)
                {
                    SetHeadFieldValue(stamper, "pageHead", generalUserReportTitle);
                    SetFieldValue(stamper, "Head", generalUserReportTitle);
                }
                else
                {
                    if (generalMessageLogFile.GetFileName(generalTest.Key) != null)
                        SetFieldValue(stamper, "pageHead", Util.GetMD5(generalMessageLogFile.GetFileName(generalTest.Key)));
                    else
                        SetFieldValue(stamper, "pageHead", generalDiagReportTitle);
                    SetHeadFieldValue(stamper, "Head", generalDiagReportTitle);
                }
                stamper.AcroFields.SetFieldProperty("Head", "textsize", 20.0f, null);
                stamper.AcroFields.SetFieldProperty("Head", "textsize", 20.0f, null);

                SetHeadFieldValue(stamper, "ver", string.Format("{0} Build:{1}", version.Ver, version.Build));
                SetHeadFieldValue(stamper, "data", generalTest.Key);
                SetHeadFieldValue(stamper, "gDate", Util.FormateDateTime2(DateTime.Now));

                SetFieldValue(stamper, "tester", generalTest.Tester);
                SetFieldValue(stamper, "startTime", Util.FormateDateTime(generalTest.StartTime));
                SetFieldValue(stamper, "planRunningTime",
                    string.Format("{0}/{1}", Util.FormateDurationSecondsMaxHour(generalTest.PlanRunningTime), 
                    Util.FormateDurationSecondsMaxHour(generalTest.RunningTime)));
                SetFieldValue(stamper, "finishReason", generalTest.FinishReason);

                string names = "";
                string sns = "";
                string passes = "";
                bool isPass = true;
                for (int i = 0; i < boards.Count; i++)
                {
                    names += boards[i].EqName + "\n";
                    if (boards[i].GeneralTestSN != "")
                        sns += boards[i].GeneralTestSN + "\n";
                    else
                        sns += "非待测" + "\n";
                    passes += boards[i].IsGeneralTestPassed ? "PASS\n" : "FAIL\n";
                    isPass &= boards[i].IsGeneralTestPassed;
                }
                names += endString;
                sns += endString;
                passes += endString;
                SetFieldValue(stamper, "boardName", names);
                SetFieldValue(stamper, "boardSn", sns);
                SetFieldValue(stamper, "isBoardPass", passes);

                if (isPass)
                {
                    SetFieldValue(stamper, "IsPass", "PASS");
                    stamper.AcroFields.SetFieldProperty("IsPass", "textsize", 38.0f, null);
                    BaseColor bc = new BaseColor(00,64,00);//DarkGreen
                    stamper.AcroFields.SetFieldProperty("IsPass", "textcolor", bc, null);
                }
                else
                {
                    SetFieldValue(stamper, "IsPass", "FAIL");
                    stamper.AcroFields.SetFieldProperty("IsPass", "textsize", 38.0f, null);
                    stamper.AcroFields.SetFieldProperty("IsPass", "textcolor", BaseColor.RED, null);
                }
                stamper.FormFlattening = true;//不允许编辑   
                stamper.Close();
                rdr.Close();
            }
            catch (Exception ee)
            {
                LogHelper.GetLogger<Report>().Error(ee.Message);
                LogHelper.GetLogger<Report>().Error(ee.StackTrace);
                //MessageBox.Show(ee.Message);
            }
        }

        private void SetHeadFieldValue(PdfStamper stamper, string name, string value)
        {
            stamper.AcroFields.SetField(name, value);
            stamper.AcroFields.SetFieldProperty(name, "textsize", fontSizeHead, null);
            stamper.AcroFields.SetFieldProperty(name, "textcolor", BaseColor.GRAY, null);
        }

        private void SetFieldValue(PdfStamper stamper, string name, string value)
        {
            stamper.AcroFields.SetField(name, value);
            stamper.AcroFields.SetFieldProperty(name, "textsize", fontSizeBody, null);
        }
    }
}
