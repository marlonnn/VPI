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
    public partial class FormGeneralSN : Office2007RibbonForm
    {
        GeneralTest generalTest;
        GlobalConfig generalGlobalConfig;
        public FormGeneralSN()
        {
            InitializeComponent();
        }

        private void ReloadFrm()
        {
            List<Board> list = generalTest.Cabinet.GetGeneralTestBoardsList();
            if (list.Count == 0)
            {
                btnOk.Enabled = false;
                btnReadBack.Enabled = false;
                btnClear.Enabled = false;
                MessageBox.Show("至少选择一个待测板卡才能设置SN号。");
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            else
            {
                this.panel.Controls.Clear();
                int ControlHeight = 25;
                int LblWidth = 90;
                int InputWidth = 128;
                int tabIndex = 0;
                tbRunningPlan.TabIndex = tabIndex++;
                tbRunningPlan.Text = string.Format("{0}", generalTest.PlanRunningTime / 60);
                tbTester.TabIndex = tabIndex++;
                tbTester.Text = generalTest.Tester;
                for (int i = 0; i < list.Count; i++)
                {
                    Board b = list[i];
                    if (b.IsGeneralTestTested)
                    {
                        Label lbl = new Label();
                        lbl.Text = string.Format("{0}", b.EqName);
                        lbl.Top = 5 + 30 * i;
                        lbl.Left = 20;
                        lbl.Width = LblWidth;
                        lbl.Height = ControlHeight;
                        this.panel.Controls.Add(lbl);

                        TextBox tb = new TextBox();
                        tb.Text = b.GeneralTestSN;
                        tb.Left = 121;
                        tb.Top = lbl.Top;
                        tb.Width = InputWidth;
                        tb.Height = ControlHeight;
                        tb.TabIndex = tabIndex++;
                        if (i == 0)
                            tb.Focus();
                        tb.Tag = b;
                        tb.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
                        this.panel.Controls.Add(tb);
                    }
                }
                //重置窗口大小和按钮位置
                this.panel.Height = (ControlHeight + 5) * list.Count + 5;
                btnOk.Top = this.panel.Bottom + 10;
                btnOk.TabIndex = tabIndex++;
                btnCancel.Top = btnOk.Top;
                btnCancel.TabIndex = tabIndex++;
                this.Height = btnOk.Bottom + 10;


                if (generalTest.PlanRunningTime <= 0)
                {
                    generalTest.PlanRunningTime = 30 * 60;//系统默认测试时长30min
                }
                tbRunningPlan.Text = string.Format("{0}", generalTest.PlanRunningTime / 60);
                btnOk.Enabled = true;
                btnReadBack.Enabled = true;
                btnClear.Enabled = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            List<Board> boards = new List<Board>();
            HashSet<String> snSet = new HashSet<string>();
            foreach (Control c in this.panel.Controls)
            {
                if (c.Tag != null && c.Tag is Board)
                {
                    TextBox tb = c as TextBox;
                    Board b = c.Tag as Board;
                    boards.Add(b);
                    if (tb.Text.Length == 0)
                    {
                        MessageBox.Show(string.Format("请输入{0}的SN号。", b.EqName));
                        tb.Focus();
                        return;
                    }
                    else
                    {
                        if(!snSet.Contains(tb.Text))
                        {
                            snSet.Add(tb.Text);
                        }
                        else 
                        {
                            MessageBox.Show(string.Format("SN号不能重复。"));
                            return;
                        }
                        b.GeneralTestSN = tb.Text;
                    }
                }
            }
            try
            {
                generalTest.PlanRunningTime = 60 * int.Parse(tbRunningPlan.Text);
                if (generalTest.PlanRunningTime <= 0)
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
                generalTest.Tester = tbTester.Text;
            }
            generalTest.TestStatus = TestStatus.EXPECTED_FINNISH;
            generalGlobalConfig.Configs.Clear();
            generalGlobalConfig.TryPut("generalTest.Tester", generalTest.Tester);
            generalGlobalConfig.TryPut("generalTest.PlanRunningTime", generalTest.PlanRunningTime);
            foreach (var c in this.panel.Controls)
            {
                TextBox tb = c as TextBox;
                if (tb != null && tb.Tag is Board)
                {
                    Board b = tb.Tag as Board;
                    generalGlobalConfig.TryPut(b.EqName + "IsGeneralTestTested", b.IsGeneralTestTested);
                    generalGlobalConfig.TryPut(b.EqName + "GeneralTestSN", b.GeneralTestSN);
                }
            }
            generalGlobalConfig.SaveCurrent();
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
                GlobalConfig last = generalGlobalConfig.ReloadLast();
                if (last != null)
                {
                    foreach (var c in this.panel.Controls)
                    {
                        TextBox tb = c as TextBox;
                        if (tb != null && tb.Tag is Board)
                        {
                            tb.Text = last.TryGetString((tb.Tag as Board).EqName + "GeneralTestSN");
                        }
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
            foreach (Control c in this.panel.Controls)
            {
                if (c.Tag != null && c.Tag is Board)
                {
                    (c as TextBox).Text = "";
                }
            }
        }
    }
}
