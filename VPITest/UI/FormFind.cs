using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using VPITest.DB;
using VPITest.Model;
using DevComponents.DotNetBar.Controls;
using VPITest.Common;
using System.Collections.Specialized;
using System.IO;

namespace VPITest.UI
{
    public partial class FormFind : Office2007RibbonForm
    {
        public static string FIND_TYPE_FCT = DbADO.TEST_TYPE_FCT;
        public static string FIND_TYPE_GENERAL = DbADO.TEST_TYPE_GENERAL;

        string findType;
        string[] boardTypeList;
        DbADO dbADO;
        MessageLogFile fctMessageLogFile;
        MessageLogFile generalMessageLogFile;
        string pdfViewExe;

        public FormFind()
        {
            InitializeComponent();
        }

        private void FormFind_Load(object sender, EventArgs e)
        {
            //只初始化一次
            if (boardTypeList != null && cbBoardType.Items.Count == 0)
            {
                cbBoardType.Items.Add("全部类型");
                cbBoardType.Items.AddRange(boardTypeList);
                cbBoardType.SelectedIndex = 0;

                cbFinishResult.Items.Add("全部");
                cbFinishResult.Items.Add(DbADO.TEST_FINISH_RESULT_NORMAL);
                cbFinishResult.Items.Add(DbADO.TEST_FINISH_RESULT_MANUAL);
                cbFinishResult.SelectedIndex = 1;

                DataGridViewButtonXColumn btnColumn = dataGridView.Columns[11] as DataGridViewButtonXColumn;
                btnColumn.Click += new EventHandler<EventArgs>(openUsrRpt_Click);
            }
            if (dtiBegin != null && dtiEnd != null)
            {
                dtiBegin.Value = DateTime.Now;
                dtiEnd.Value = DateTime.Now;
            }
            //综合测试不显示待测试项目一列
            if (findType == FIND_TYPE_GENERAL)
            {
                clnTestedItems.Visible = false;
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {            
            DateTime dtBegin = dtiBegin.Value;
            DateTime dtEnd = dtiEnd.Value.AddDays(1);
            if (cbNoLimitDate.Checked)
            {
                dtBegin = DateTime.MinValue;
                dtEnd = DateTime.MaxValue;
            }
            else if (cbLastMonth.Checked)
            {
                dtBegin = DateTime.Now.AddMonths(-1);
                dtEnd = DateTime.Now.AddDays(1);
            }
            string boardType = "";
            if (cbBoardType.SelectedIndex > 0)
            {
                boardType = cbBoardType.Items[cbBoardType.SelectedIndex].ToString();
            }
            string finishResult = "";
            if (cbFinishResult.SelectedIndex > 0)
            {
                finishResult = cbFinishResult.Items[cbFinishResult.SelectedIndex].ToString();
            }
            string key = tbTestKey.Text.ToUpper();
            string sn = tbBoardSn.Text;
            btnQuery.Enabled = false;
            try
            {
                IList<VDetail> list = dbADO.Find(dtBegin, dtEnd, boardType, key, sn, finishResult, findType);
                DataGridViewRow[] rows = new DataGridViewRow[list.Count];
                int i = 0;
                foreach (var d in list)
                {
                    DataGridViewRow aRow = new DataGridViewRow();
                    aRow.CreateCells(dataGridView);
                    aRow.Cells[0].Value = false;
                    aRow.Cells[1].Value = d.Key;
                    aRow.Cells[2].Value = Util.FormateDateTime3(d.Start);
                    aRow.Cells[3].Value = Util.FormateDurationSecondsMaxHour(d.RunningTime);
                    aRow.Cells[4].Value = d.Tester;
                    aRow.Cells[5].Value = d.BoardType;
                    aRow.Cells[6].Value = d.BoardName;
                    aRow.Cells[7].Value = d.BoardSn;
                    aRow.Cells[8].Value = d.Component;
                    aRow.Cells[9].Value = d.IsTestedItemPass;
                    aRow.Cells[10].Value = d.TestNote;     
                    if (findType == FIND_TYPE_FCT)
                    {

                        aRow.Cells[11].Value= Report.GetUsrFctPdfName(d.TestNote,d.Key, d.BoardSn, d.IsTestedItemPass == "PASS");
                        aRow.Cells[12].Value = Report.GetDiagnoseFctPdfName(d.Key);
                        aRow.Cells[13].Value = fctMessageLogFile.GetFileName(d.Key);
                    }
                    else
                    {
                        aRow.Cells[11].Value = Report.GetUsrGeneralPdfName(d.TestNote, d.Key, d.BoardSn, d.IsTestedItemPass == "PASS");
                        aRow.Cells[12].Value = Report.GetDiagnoseGeneralPdfName(d.Key);
                        aRow.Cells[13].Value = generalMessageLogFile.GetFileName(d.Key);
                    }
                    rows[i] = aRow;
                    i++;
                }
                dataGridView.Rows.Clear();
                dataGridView.Rows.AddRange(rows);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            btnQuery.Enabled = true;
        }

        private void openUsrRpt_Click(object sender, EventArgs e)
        {
            ButtonItem item = sender as ButtonItem;
            if (item.Text != null)
            {
                //this.Enabled = false;
                try
                {
                    System.Diagnostics.Process.Start(Util.GetBasePath() + pdfViewExe, item.Text);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("文件已删除。");
                }
                //this.Enabled = true;
            }
        }
        
        private void btnCopySelected_Click(object sender, EventArgs e)
        {
            exportSelectedItems();
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].Cells[0].Value = true;
            }
            exportSelectedItems();
        }

        private void exportSelectedItems()
        {
            HashSet<string> files = new HashSet<string>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if ((bool)(dataGridView.Rows[i].Cells[0].Value) == true)
                {
                    TryAdd(files, (dataGridView.Rows[i].Cells[11].Value as string));
                    if (cbCopyDiagnose.Checked)
                    {
                        TryAdd(files, (dataGridView.Rows[i].Cells[12].Value as string));
                        TryAdd(files, (dataGridView.Rows[i].Cells[13].Value as string));
                    }
                }
            }
            copyFiles2Clipboard(files);
        }

        private static void TryAdd(HashSet<string> aSet,string msg)
        {
            if (!aSet.Contains(msg))
                aSet.Add(msg);
        }

        private void copyFiles2Clipboard(HashSet<string> pathFiles)
        {
            if (pathFiles.Count == 0)
            {
                MessageBox.Show("请选择至少一个文件，然后再导出数据。");
                return;
            }
            StringCollection paths = new StringCollection();
            foreach (var f in pathFiles)
            {
                try
                {
                    FileInfo fi = new FileInfo(f);
                    paths.Add(f);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(string.Format("文件{0}不存在，无法导出数据。",f));
                    return;
                }                
            }
            Clipboard.SetFileDropList(paths);
            MessageBox.Show("文件已经复制到剪贴板，请打开目标文件夹，然后单击鼠标右键，点击“粘贴”，开始复制文件。");
        }

        private void cbNoLimitDate_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxX select = sender as CheckBoxX;
            if(select.Checked == true)
            {
                if (cbLastMonth == select)
                    cbNoLimitDate.Checked = false;
                else
                    cbLastMonth.Checked = false;
            }
        }

        private void cbLastMonth_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxX select = sender as CheckBoxX;
            if (select.Checked == true)
            {
                if (cbNoLimitDate == select)
                    cbLastMonth.Checked = false;
                else
                    cbNoLimitDate.Checked = false;
            }
        }
    }
}
