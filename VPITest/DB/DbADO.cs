using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Data.DbMapping;
using Summer.System.Data;
using VPITest.Model;
using VPITest.Common;
using Summer.System.Log;

namespace VPITest.DB
{
    /// <summary>
    /// 单项测试和综合测试使用同样的数据表和数据视图，通过vc_testtype区分两种测试
    /// </summary>
    public class DbADO
    {
        public static string TEST_TYPE_FCT = "FCT";
        public static string TEST_TYPE_GENERAL = "GENERAL";

        public static string TEST_RESULT_PASS = "PASS";
        public static string TEST_RESULT_FAIL = "FAIL";

        public static string TEST_FINISH_RESULT_NORMAL = "正常结束";
        public static string TEST_FINISH_RESULT_MANUAL = "强制结束";

        MainADO mainADO;
        BoardADO boardADO;
        DetailADO detailADO;

        public void Save2DB(FctTest fctTest)
        {
            TMain main = new TMain();
            main.Key = fctTest.Key;
            main.Start = fctTest.StartTime;
            main.RunningTime = fctTest.RunningTime;
            main.Tester = fctTest.Tester;
            main.TestType = TEST_TYPE_FCT;
            main.IsPass = fctTest.IsPass() ? TEST_RESULT_PASS : TEST_RESULT_FAIL;
            main.Note = fctTest.FinishReason;
            mainADO.Insert(main);

            Dictionary<Board, List<ComponentType>> dicts = fctTest.Cabinet.GetFctTestedComponentTypesDicts();
            foreach (var b in dicts.Keys)
            {
                TBoard board = new TBoard();
                board.Id = Summer.System.Util.DbHelper.GenerateKey();
                board.Key = fctTest.Key;
                board.Name = b.EqName;
                board.Type = b.BoardType;
                board.Component = "";
                List<ComponentType> list = dicts[b];
                foreach (var ct in list)
                {
                    board.Component += ct.EqName + " ";
                }
                board.Sn = b.FctTestSN;
                board.IsPass = b.IsFctTestPassed() ? TEST_RESULT_PASS : TEST_RESULT_FAIL;
                boardADO.Insert(board);
            }
        }

        public void Save2DB(GeneralTest generalTest)
        {
            TMain main = new TMain();
            main.Key = generalTest.Key;
            main.Start = generalTest.StartTime;
            main.RunningTime = generalTest.RunningTime;
            main.Tester = generalTest.Tester;
            main.TestType = TEST_TYPE_GENERAL;
            main.IsPass = generalTest.IsPass() ? TEST_RESULT_PASS : TEST_RESULT_FAIL;
            main.Note = generalTest.FinishReason;
            mainADO.Insert(main);

            List<Board> boards = generalTest.Cabinet.GetGeneralTestBoardsList();
            foreach (var b in boards)
            {
                TBoard board = new TBoard();
                board.Id = Summer.System.Util.DbHelper.GenerateKey();
                board.Key = generalTest.Key;
                board.Name = b.EqName;
                board.Type = b.BoardType;
                board.Component = ""; //综合测试没有组件，所以设置为空
                board.Sn = b.GeneralTestSN;
                board.IsPass = b.IsGeneralTestPassed ? TEST_RESULT_PASS : TEST_RESULT_FAIL;
                boardADO.Insert(board);
            }
        }

        public IList<VDetail> Find(DateTime begin, DateTime end, string boardType, string key, string sn, string finishResult, string testType)
        {
            string sql = "select * from v_detail where 1=1 ";
            if (begin != null)
                sql += string.Format(" and dt_test >= '{0}'", Util.FormateDate(begin));
            if (end != null)
                sql += string.Format(" and dt_test <= '{0}'", Util.FormateDate(end));
            if (boardType != null && boardType.Length > 0)
                sql += string.Format(" and vc_boardtype = '{0}'", boardType);
            if (key != null && key.Length > 0)
                sql += string.Format(" and vc_key like '%{0}%'", key);
            if (sn != null && sn.Length > 0)
                sql += string.Format(" and vc_sn = '{0}'", sn);
            if (testType != null && testType.Length > 0)
                sql += string.Format(" and vc_testtype = '{0}'", testType);
            if (finishResult != null && finishResult.Length > 0)
                sql += string.Format(" and vc_note = '{0}'", finishResult);
            sql += " order by dt_test,vc_boardtype";
            LogHelper.GetLogger<DbADO>().Debug(sql);
            return detailADO.Find(sql);
        }
    }
}
