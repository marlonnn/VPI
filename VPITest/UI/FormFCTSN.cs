using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using VPITest.Model;
using VPITest.Common;

namespace VPITest.UI
{
    public partial class FormFCTSN : Office2007RibbonForm
    {
        FctTest fctTest;
        GlobalConfig fctGlobalConfig;
        public FormFCTSN()
        {
            InitializeComponent();
        }
      
        private void ReloadFrm()
        {
            Dictionary<Board, List<VPITest.Model.ComponentType>> list = fctTest.Cabinet.GetFctTestedComponentTypesDicts();
            tbCPUSn.Tag = fctTest.Cabinet.Racks[0].Boards[0];
            tbCPUSn.Enabled = false;
            tbCPUSn.Text = "";
            tbVcomSn.Tag = fctTest.Cabinet.Racks[0].Boards[3];
            tbVcomSn.Enabled = false;
            tbVcomSn.Text = "";
            if (list.Count == 0)
            {
                btnOk.Enabled = false;
                btnReadBack.Enabled = false;
                btnClear.Enabled = false;
                MessageBox.Show("至少选择一个待测试项目才能设置SN号。");
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                foreach (var k in list.Keys)
                {
                    if (k == tbCPUSn.Tag)
                    {
                        tbCPUSn.Enabled = true;
                        tbCPUSn.Text = k.FctTestSN;
                        tbCPUSn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
                    }
                    else if (k == tbVcomSn.Tag)
                    {
                        tbVcomSn.Enabled = true;
                        tbVcomSn.Text = k.FctTestSN;
                        tbVcomSn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
                    }
                }
                if (fctTest.PlanRunningTime <= 0)
                {
                    fctTest.PlanRunningTime = 30 * 60;//系统默认测试时长30min
                }
                tbRunningPlan.Text = string.Format("{0}", fctTest.PlanRunningTime / 60);
                tbTester.Text = fctTest.Tester;
                tbTester.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
                btnOk.Enabled = true;
                btnReadBack.Enabled = true;
                btnClear.Enabled = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (tbCPUSn.Enabled && tbCPUSn.Text.Length == 0)
            {
                MessageBox.Show("请设定CPU/PD1板卡的SN号。");
                tbCPUSn.Focus();
                return;
            }
            else if (tbCPUSn.Enabled && tbCPUSn.Text.Length > 0)
            {
                (tbCPUSn.Tag as Board).FctTestSN = tbCPUSn.Text;
            }

            if (tbVcomSn.Enabled && tbVcomSn.Text.Length == 0)
            {
                MessageBox.Show("请设定VCOM板卡的SN号。");
                tbVcomSn.Focus();
                return;
            }
            else if (tbVcomSn.Text == tbCPUSn.Text)
            {
                MessageBox.Show("SN号不能重复。");
                tbVcomSn.Focus();
                return;
            }
            else if (tbVcomSn.Enabled && tbVcomSn.Text.Length > 0)
            {
                (tbVcomSn.Tag as Board).FctTestSN = tbVcomSn.Text;
            }

            try
            {
                fctTest.PlanRunningTime = 60 * int.Parse(tbRunningPlan.Text);
                if (fctTest.PlanRunningTime <= 0 )
                {
                    MessageBox.Show("测试预设时间应该大于0。");
                    return;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("请输入有效的测试时长（单位分钟，输入整数，最大65535）。");
                tbRunningPlan.Focus();
                return;
            }

            if (tbTester.Text.Length == 0)
            {
                MessageBox.Show("请输入测试人员姓名。");
                tbTester.Focus();
                return;
            }
            else
            {
                fctTest.Tester = tbTester.Text;
            }

            fctTest.TestStatus = TestStatus.EXPECTED_FINNISH;

            fctGlobalConfig.Configs.Clear();
            fctGlobalConfig.TryPut("fctTest.Tester", fctTest.Tester);
            fctGlobalConfig.TryPut("fctTest.PlanRunningTime", fctTest.PlanRunningTime);
            foreach (var c in this.boardPanel.Controls)
            {
                TextBox tb = c as TextBox;
                if (tb != null && tb.Tag is Board)
                {
                    Board b = tb.Tag as Board;
                    if (tb.Enabled)
                    {
                        fctGlobalConfig.TryPut(b.EqName + "FctTestSN", b.FctTestSN);
                    }
                    foreach (var v in b.ComponentTypes)
                    {
                        fctGlobalConfig.TryPut(b.EqName + v.EqName + "IsFctTestTested", v.IsFctTestTested);
                    }
                }
            }
            fctGlobalConfig.SaveCurrent();

            this.DialogResult = DialogResult.OK;
        }

        //回车后换到下一个文本框
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void btnReadBack_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalConfig last = fctGlobalConfig.ReloadLast();
                if (last != null)
                {
                    if (tbCPUSn.Enabled)
                    {
                        Board b = tbCPUSn.Tag as Board;
                        tbCPUSn.Text = last.TryGetString(b.EqName + "FctTestSN");
                    }
                    if (tbVcomSn.Enabled)
                    {
                        Board b = tbVcomSn.Tag as Board;
                        tbVcomSn.Text = last.TryGetString(b.EqName + "FctTestSN");
                    }
                }
            }
            catch (Exception ee)
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void formGeneralSN_Load(object sender, EventArgs e)
        {
            ReloadFrm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbCPUSn.Text = "";
            tbVcomSn.Text = "";
        }
    }
}
