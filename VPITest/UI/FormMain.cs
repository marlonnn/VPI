using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.AdvTree;
using Summer.System.Core;
using Summer.System.Log;
using System.Threading;
using VPITest.Model;
using VPITest.Common;
using VPITest.Net;
using VPITest.DB;
namespace VPITest.UI
{
    public partial class FormMain : Office2007RibbonForm
    {

        Udp udp;
        SelfTest selfTest;
        GeneralTest generalTest;
        FctTest fctTest;
        bool isUIStart = false;
        TestSemaphore testSemaphore;

        FormGeneralSN formGeneralSN;
        FormFCTSN formFCTSN;
        FormFind formGeneralFind;
        FormFind formFCTFind;
        FormAbout formAbout;

        private const int SELF_CHECK = 0;
        private const int FCT_TEST = 1;
        private const int GENERAL_TEST = 2;
        private int testType;

        private delegate void UIFinishDelegate();
        private delegate void UpdateTreeAndMatrixViewDelegate(BoardStatusEventArgs ea);
        private void UpdateTreeAndMatrixView(BoardStatusEventArgs ea)
        {
            UpdateGeneralTreeNodeColor();
            judgeMatrixView.UpdateErrorBoard(ea);
        }

        public FormMain()
        {
            InitializeComponent();
        }

        #region 系统自检
        //初始化左边板卡选择树
        public void DisplaySelfTree(Cabinet cabinet)
        {
            cabinetTreeView.Nodes.Clear();
            TreeNode baseroot = new TreeNode();
            baseroot.Tag = cabinet;
            baseroot.Text = cabinet.EqName;

            foreach (var r in cabinet.Racks)
            {
                TreeNode rackNode = new TreeNode();
                rackNode.Tag = r;
                rackNode.Text = r.EqName;
                baseroot.Nodes.Add(rackNode);
                foreach (var b in r.Boards)
                {
                    TreeNode boardNode = new TreeNode();
                    boardNode.Tag = b;
                    boardNode.Text = b.EqName;
                    rackNode.Nodes.Add(boardNode);
                }
            }
            baseroot.ExpandAll();
            cabinetTreeView.Nodes.Add(baseroot);
            cabinetTreeView.Refresh();
        }
        private void UpdateSelfTreeNodeColor()
        {
            Color passColor, warningColor, failColor, normalColor;
            if (selfTest.TestStatus == TestStatus.RUNNING)
            {
                passColor = Color.Transparent;
                warningColor = Color.Yellow;
                failColor = Color.Yellow;
                normalColor = Color.Transparent;
            }
            else
            {
                passColor = Color.Green;
                warningColor = Color.Yellow;
                failColor = Color.Red;
                normalColor = Color.Transparent;
            }
            foreach (TreeNode c in cabinetTreeView.Nodes)
            {
                foreach (TreeNode r in c.Nodes)
                {
                    foreach (TreeNode bn in r.Nodes)
                    {
                        Board b = bn.Tag as Board;
                        if (b.IsGeneralTestPassed == true)//可测组件没有报错
                        {
                            bn.BackColor = passColor;
                        }
                        else if (b.IsGeneralTestPassed == false)//可测组件报错
                        {
                            bn.BackColor = failColor;
                        }
                    }//end b.Nodes;
                }//end r.Nodes
            }//end cabinetTree.Nodes 
        }

        private void UISelfInitial()
        {
            btnSelfTest.Enabled = true;
            cabinetTreeView.Enabled = true;
        }

        private void UISelfStart()
        {
            btnSelfTest.Enabled = false;
            cabinetTreeView.Enabled = false;
            lblRunningTime.Text = "计时：000:00:00";
            lblRemainTime.Text = "剩余：000:00:00";
            lblStatus.Text = "Preparing...";
            lblStatus.ForeColor = Color.OrangeRed;
            selfLogList.Items.Clear();
            foreach (TreeNode c in cabinetTreeView.Nodes)
            {
                foreach (TreeNode r in c.Nodes)
                {
                    foreach (TreeNode b in r.Nodes)
                    {
                        b.BackColor = Color.Transparent;
                    }
                }
            }
        }

        private void UISelfFinish()
        {
            btnSelfTest.Enabled = true;
            cabinetTreeView.Enabled = true;
            if (selfTest.IsPass())
            {
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "PASS";
            }
            else
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "FAIL";
            }
        }

        private void LoadSelfTestView()
        {
            //只加载一次
            if (selfLogList.Timer == null)
            {
                selfTest.OnTestStatusChange += new EventHandler(selfTest_OnTestStatusChange);
                selfTest.OnBoardStatusChange += new EventHandler(selfTest_OnBoardStatusChange);
                selfTest.OnNewMessage += new EventHandler(selfTest_OnNewMessage);
                UISelfInitial();
                DisplaySelfTree(this.selfTest.Cabinet);
                selfLogList.Timer = mainTimer;
                udp.UpdRxStart();
            }
        }

        #region 系统自检按钮
        //开始自检
        private void btnSelfTest_Click(object sender, EventArgs e)
        {
            try
            {
                selfTest.StartTest();
                UISelfStart();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnSelfDialog_Click(object sender, EventArgs e)
        {
            Report.ExploreSelfDiagReportFolder();
        }
        #endregion

        #region 系统自检事件
        private void selfTest_OnTestStatusChange(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(SelfTest)))
                return;
            TestStatusEventArgs ea = e as TestStatusEventArgs;
            selfLogList.AppendLog(new string[] { DateTime.Now.ToString("HH:mm:ss:fff"), ea.Reason });
            if (ea.CurStatus == TestStatus.EXPECTED_FINNISH || ea.CurStatus == TestStatus.UNEXPECTED_FINNISH)
            {
                UpdateSelfTreeNodeColor();
                isUIStart = false;
                this.Invoke(new UIFinishDelegate(UISelfFinish));
            }
            else if (ea.CurStatus == TestStatus.RUNNING)
            {
                isUIStart = true;
                testType = SELF_CHECK;
            }
        }

        private void selfTest_OnBoardStatusChange(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(SelfTest)))
                return;
            BoardStatusEventArgs ea = e as BoardStatusEventArgs;
            UpdateSelfTreeNodeColor();
        }

        private void selfTest_OnNewMessage(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(SelfTest)))
                return;
            NewMessageEventArgs ea = e as NewMessageEventArgs;
            selfLogList.AppendLog(new string[] { ea.rxMessage.DtTime.ToString("HH:mm:ss:fff"), ea.rxMessage.ToString() });
        }
        #endregion


        #endregion

        #region 组件测试(FCT)
        public void DisplayFCTTree(Cabinet cabinet)
        {
            Rack sysRack = cabinet.Racks[0];
            Board cpuBoard = sysRack.Boards[0];
            Board vcomBoard = sysRack.Boards[3];

            fctCheckBoxTree.Nodes.Clear();
            TreeNode baseroot = new TreeNode();
            baseroot.Tag = sysRack;
            baseroot.Text = sysRack.EqName;


            TreeNode cpuBoardNode = new TreeNode();
            cpuBoardNode.Tag = cpuBoard;
            cpuBoardNode.Text = cpuBoard.EqName;
            baseroot.Nodes.Add(cpuBoardNode);
            foreach (var ct in cpuBoard.ComponentTypes)
            {
                TreeNode node = new TreeNode();
                node.Tag = ct;
                node.Text = ct.EqName;
                if (ct.IsFctTestTested)
                {
                    node.Checked = true;
                }
                cpuBoardNode.Nodes.Add(node);
            }

            TreeNode vcomBoardNode = new TreeNode();
            vcomBoardNode.Tag = vcomBoard;
            vcomBoardNode.Text = vcomBoard.EqName;

            baseroot.Nodes.Add(vcomBoardNode);
            foreach (var ct in vcomBoard.ComponentTypes)
            {
                TreeNode node = new TreeNode();
                node.Tag = ct;
                node.Text = ct.EqName;
                if (ct.IsFctTestTested)
                {
                    node.Checked = true;
                }
                vcomBoardNode.Nodes.Add(node);
            }

            baseroot.ExpandAll();
            fctCheckBoxTree.Nodes.Add(baseroot);
            fctCheckBoxTree.HideCheckBox(baseroot);
            fctCheckBoxTree.HideCheckBox(cpuBoardNode);
            fctCheckBoxTree.HideCheckBox(vcomBoardNode);
            fctCheckBoxTree.Refresh();
        }

        private void UpdateFCTTreeNodeColor()
        {
            Color passColor, warningColor, failColor, normalColor;
            if (fctTest.TestStatus == TestStatus.RUNNING)
            {
                passColor = Color.Transparent;
                warningColor = Color.Yellow;
                failColor = Color.Yellow;
                normalColor = Color.Transparent;
            }
            else
            {
                passColor = Color.Green;
                warningColor = Color.Yellow;
                failColor = Color.Red;
                normalColor = Color.Transparent;
            }
            foreach (TreeNode r in fctCheckBoxTree.Nodes)
            {
                foreach (TreeNode b in r.Nodes)
                {
                    foreach (TreeNode cn in b.Nodes)
                    {
                        ComponentType ct = cn.Tag as ComponentType;
                        if (ct.IsFctTestTested == true && ct.IsFctTestPassed() == true)//待测组件没有报错
                        {
                            cn.BackColor = passColor;
                        }
                        else if (ct.IsFctTestTested == true && ct.IsFctTestPassed() == false)//待测组件报错
                        {
                            cn.BackColor = failColor;
                        }
                        else if (ct.IsFctTestTested == false && ct.IsFctTestPassed() == true)//非待测组件没有报错
                        {
                            cn.BackColor = normalColor;
                        }
                        else if (ct.IsFctTestTested == false && ct.IsFctTestPassed() == false)//非待测组件报错
                        {
                            cn.BackColor = warningColor;
                        }
                    }//end b.Nodes;
                }//end r.Nodes
            }//end cabinetTree.Nodes 
        }

        private void UpdateTestedComponentTypes()
        {
            foreach (TreeNode bNode in fctCheckBoxTree.Nodes[0].Nodes)
            {
                foreach (TreeNode cNode in bNode.Nodes)
                {
                    (cNode.Tag as ComponentType).IsFctTestTested = cNode.Checked;
                }
            }
        }

        private void UIFCTInitial()
        {
            btnFCTStart.Enabled = true;
            btnFCTFinish.Enabled = false;
            btnFCTSN.Enabled = true;
            btnFCTSearch.Enabled = true;
            fctCheckBoxTree.Enabled = true;
            if (componentSummaryView.Timer == null)
            {
                componentSummaryView.Timer = mainTimer;
            }
        }

        private void UIFCTClear()
        {
            lblRunningTime.Text = "计时:000:00:00";
            lblRemainTime.Text = "剩余:000:00:00";
            lblStatus.Text = "Preparing...";
            lblStatus.ForeColor = Color.OrangeRed;
            fctLogList.Items.Clear();
            componentSummaryView.Reset(TestSemaphore.FCT_RUNNING);
            foreach (TreeNode r in fctCheckBoxTree.Nodes)
            {
                foreach (TreeNode b in r.Nodes)
                {
                    foreach (TreeNode cn in b.Nodes)
                    {
                        cn.BackColor = Color.Transparent;
                    }
                }
            }
        }

        private void UIFCTStart()
        {
            btnFCTStart.Enabled = false;
            btnFCTFinish.Enabled = true;
            btnFCTSN.Enabled = false;
            btnFCTSearch.Enabled = false;
            fctCheckBoxTree.Enabled = false;
            UIFCTClear();
        }

        private void UIFCTFinish()
        {
            btnFCTStart.Enabled = true;
            btnFCTFinish.Enabled = false;
            btnFCTSN.Enabled = true;
            btnFCTSearch.Enabled = true;
            fctCheckBoxTree.Enabled = true;
            if (fctTest.IsPass())
            {
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "PASS";
            }
            else
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "FAIL";
            }

        }

        private void fctCheckBoxTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    //不可能的情况，复选框在叶子节点
                }
                else
                {
                    if (e.Node.Tag is ComponentType)
                    {
                        //设定同时选中
                        fctCheckBoxTree.Nodes[0].Nodes[0].Nodes[e.Node.Index].Checked = e.Node.Checked;
                        fctCheckBoxTree.Nodes[0].Nodes[1].Nodes[e.Node.Index].Checked = e.Node.Checked;
                        //更新待测组件
                        UpdateTestedComponentTypes();
                    }
                }
                //testedRacks.SaveThis();
            }
        }

        private void LoadFCTTestView()
        {
            //只加载一次
            if (fctLogList.Timer == null)
            {
                fctTest.OnTestStatusChange += new EventHandler(fctTest_OnTestStatusChange);
                fctTest.OnComponentStatusChange += new EventHandler(fctTest_OnComponentStatusChange);
                fctTest.OnNewMessage += new EventHandler(fctTest_OnNewMessage);
                UIFCTInitial();
                DisplayFCTTree(this.fctTest.Cabinet);
                fctLogList.Timer = mainTimer;
                //componentSummaryView.Timer = mainTimer;
            }
        }

        #region 组件测试按钮（FCT）
        private void btnFCTSN_Click(object sender, EventArgs e)
        {
            if (formFCTSN.ShowDialog() == DialogResult.OK)
            {
                UIFCTClear();
            }
        }

        private void btnFCTStart_Click(object sender, EventArgs e)
        {
            try
            {
                fctTest.StartTest();
                UIFCTStart();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnFCTFinish_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要停止测试吗？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                fctTest.FinishManualTest();
                UIFCTFinish();

            }
        }

        private void btnFCTSearch_Click(object sender, EventArgs e)
        {
            formFCTFind.ShowDialog();
        }

        private void btnFCTReport_Click(object sender, EventArgs e)
        {
            Report.ExploreFctReportFolder();
        }

        private void btnFCTDialog_Click(object sender, EventArgs e)
        {
            Report.ExploreFctDiagReportFolder();
        }
        #endregion

        #region 组件测试事件（FCT）
        private void fctTest_OnTestStatusChange(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(FctTest)))
                return;
            TestStatusEventArgs ea = e as TestStatusEventArgs;
            fctLogList.AppendLog(new string[] { DateTime.Now.ToString("HH:mm:ss:fff"), ea.Reason });
            if (ea.CurStatus == Common.TestStatus.EXPECTED_FINNISH || ea.CurStatus == Common.TestStatus.UNEXPECTED_FINNISH)
            {
                UpdateFCTTreeNodeColor();
                isUIStart = false;
                this.Invoke(new UIFinishDelegate(UIFCTFinish));
            }
            else if (ea.CurStatus == TestStatus.RUNNING)
            {
                isUIStart = true;
                testType = FCT_TEST;
            }
        }

        private void fctTest_OnComponentStatusChange(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(FctTest)))
                return;
            ComponentStatusEventArgs ea = e as ComponentStatusEventArgs;
            UpdateFCTTreeNodeColor();
        }

        private void fctTest_OnNewMessage(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(FctTest)))
                return;
            NewMessageEventArgs ea = e as NewMessageEventArgs;
            fctLogList.AppendLog(new string[] { ea.rxMessage.DtTime.ToString("HH:mm:ss:fff"), ea.rxMessage.ToString() });
        }
        #endregion

        #endregion

        #region 单板测试（综合）
        public void DisplayGenralTree(Cabinet cabinet)
        {
            generalCheckBoxTree.Nodes.Clear();
            TreeNode baseroot = new TreeNode();
            baseroot.Tag = cabinet;
            baseroot.Text = cabinet.EqName;

            foreach (var r in cabinet.Racks)
            {
                TreeNode rackNode = new TreeNode();
                rackNode.Tag = r;
                rackNode.Text = r.EqName;
                baseroot.Nodes.Add(rackNode);
                foreach (var b in r.Boards)
                {
                    TreeNode boardNode = new TreeNode();
                    boardNode.Tag = b;
                    boardNode.Text = b.EqName;
                    if (b.IsGeneralTestTested)
                    {
                        boardNode.Checked = true;
                    }
                    rackNode.Nodes.Add(boardNode);
                }
            }
            baseroot.ExpandAll();
            generalCheckBoxTree.Nodes.Add(baseroot);
            foreach (TreeNode cNode in generalCheckBoxTree.Nodes)
            {
                generalCheckBoxTree.HideCheckBox(cNode);
                foreach (TreeNode rNode in cNode.Nodes)
                {
                    generalCheckBoxTree.HideCheckBox(rNode);
                    foreach (TreeNode bNode in rNode.Nodes)
                    {
                        Board b = bNode.Tag as Board;
                        if (!b.CanGeneralTest)
                        {
                            generalCheckBoxTree.HideCheckBox(bNode);
                        }
                    }
                }
            }
            generalCheckBoxTree.Refresh();
        }

        private void UpdateGeneralTreeNodeColor()
        {
            Color passColor, warningColor, failColor, normalColor;
            if (generalTest.TestStatus == TestStatus.RUNNING)
            {
                passColor = Color.Transparent;
                warningColor = Color.Yellow;
                failColor = Color.Yellow;
                normalColor = Color.Transparent;
            }
            else
            {
                passColor = Color.Green;
                warningColor = Color.Yellow;
                failColor = Color.Red;
                normalColor = Color.Transparent;
            }
            foreach (TreeNode c in generalCheckBoxTree.Nodes)
            {
                foreach (TreeNode r in c.Nodes)
                {
                    foreach (TreeNode bn in r.Nodes)
                    {
                        Board b = bn.Tag as Board;
                        if (b.IsGeneralTestTested == true && b.IsGeneralTestPassed == true)//待测组件没有报错
                        {
                            bn.BackColor = passColor;
                        }
                        else if (b.IsGeneralTestTested == true && b.IsGeneralTestPassed == false)//待测组件报错
                        {
                            bn.BackColor = failColor;
                        }
                        else if (b.IsGeneralTestTested == false && b.IsGeneralTestPassed == true)//非待测组件没有报错
                        {
                            bn.BackColor = normalColor;
                        }
                        else if (b.IsGeneralTestTested == false && b.IsGeneralTestPassed == false)//非待测组件报错
                        {
                            bn.BackColor = warningColor;
                        }
                    }//end b.Nodes;
                }//end r.Nodes
            }//end cabinetTree.Nodes 
        }        

        private void UIGeneralInitial()
        {
            btnGeneralStart.Enabled = true;
            btnGeneralFinish.Enabled = false;
            btnGeneralSN.Enabled = true;
            btnGeneralSearch.Enabled = true;
            generalCheckBoxTree.Enabled = true;
            judgeMatrixView.Init();
            if (judgeMatrixView.Timer == null)
            {
                judgeMatrixView.Timer = mainTimer;
            }
            if (componentSummaryViewGeneral.Timer == null)
            {
                componentSummaryViewGeneral.Timer = mainTimer;
            }
        }

        private void UIGeneralClear()
        {
            lblRunningTime.Text = "计时:000:00:00";
            lblRemainTime.Text = "剩余:000:00:00";
            lblStatus.Text = "Preparing...";
            lblStatus.ForeColor = Color.OrangeRed;
            generalLogList.Items.Clear();
            foreach (TreeNode c in generalCheckBoxTree.Nodes)
            {
                foreach (TreeNode r in c.Nodes)
                {
                    foreach (TreeNode b in r.Nodes)
                    {
                        b.BackColor = Color.Transparent;
                    }
                }
            }
            componentSummaryViewGeneral.Reset(TestSemaphore.GENERAL_RUNNING);
            judgeMatrixView.Reset();
        }

        private void UIGeneralStart()
        {
            btnGeneralStart.Enabled = false;
            btnGeneralFinish.Enabled = true;
            btnGeneralSN.Enabled = false;
            btnGeneralSearch.Enabled = false;
            generalCheckBoxTree.Enabled = false;
            UIGeneralClear();
        }

        private void UIGeneralFinish()
        {
            btnGeneralStart.Enabled = true;
            btnGeneralFinish.Enabled = false;
            btnGeneralSN.Enabled = true;
            btnGeneralSearch.Enabled = true;
            generalCheckBoxTree.Enabled = true;
            if (generalTest.IsPass())
            {
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "PASS";
            }
            else
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "FAIL";
            }
        }

        private void generalCheckBoxTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    //不可能的情况，复选框在叶子节点
                }
                else
                {
                    if (e.Node.Tag != null && e.Node.Tag is Board)
                    {
                        //更新待测板卡
                        UpdateGeneralTestedBoards();
                    }
                }
            }
        }

        private void UpdateGeneralTestedBoards()
        {
            foreach (TreeNode cNode in generalCheckBoxTree.Nodes)
            {
                foreach (TreeNode rNode in cNode.Nodes)
                {
                    foreach (TreeNode bNode in rNode.Nodes)
                    {
                        (bNode.Tag as Board).IsGeneralTestTested = bNode.Checked;
                    }
                }
            }
        }

        private void LoadGeneralTestView()
        {
            //只加载一次
            if (generalLogList.Timer == null)
            {
                generalTest.OnTestStatusChange += new EventHandler(generalTest_OnTestStatusChange);
                generalTest.OnBoardStatusChange += new EventHandler(generalTest_OnBoardStatusChange);
                generalTest.OnNewMessage += new EventHandler(generalTest_OnNewMessage);
                UIGeneralInitial();
                DisplayGenralTree(this.generalTest.Cabinet);
                generalLogList.Timer = mainTimer;
            }
        }


        #region 单板测试按钮（综合）
        private void btnGeneralSN_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == formGeneralSN.ShowDialog())
            {
                UIGeneralClear();
            }
        }

        private void btnGeneralStart_Click(object sender, EventArgs e)
        {
            try
            {
                generalTest.StartTest();
                UIGeneralStart();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnGeneralFinish_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要停止测试吗？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                UIGeneralFinish();
                generalTest.FinishManualTest(Common.TestStatus.FORCE_FINISH,DbADO.TEST_FINISH_RESULT_MANUAL);
            }
        }

        private void btnGeneralSearch_Click(object sender, EventArgs e)
        {
            formGeneralFind.ShowDialog();
        }

        private void btnGeneralReport_Click(object sender, EventArgs e)
        {
            Report.ExploreGeneralReportFolder();
        }

        private void btnGeneralDialog_Click(object sender, EventArgs e)
        {
            Report.ExploreGeneralDiagReportFolder();
        }
        #endregion

        #region 单板测试事件（综合）
        private void generalTest_OnTestStatusChange(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(GeneralTest)))
                return;
            TestStatusEventArgs ea = e as TestStatusEventArgs;
            generalLogList.AppendLog(new string[] { DateTime.Now.ToString("HH:mm:ss:fff"), ea.Reason });
            if (ea.CurStatus == Common.TestStatus.EXPECTED_FINNISH || ea.CurStatus == Common.TestStatus.UNEXPECTED_FINNISH || 
                ea.CurStatus == Common.TestStatus.FORCE_FINISH)
            {
                UpdateGeneralTreeNodeColor();
                isUIStart = false;
                this.Invoke(new UIFinishDelegate(UIGeneralFinish));
            }
            else if (ea.CurStatus == TestStatus.RUNNING)
            {
                isUIStart = true;
                testType = GENERAL_TEST;
            }
        }

        private void generalTest_OnBoardStatusChange(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(GeneralTest)))
                return;
            BoardStatusEventArgs ea = e as BoardStatusEventArgs;
            this.Invoke(new UpdateTreeAndMatrixViewDelegate(UpdateTreeAndMatrixView), ea);
            //UpdateGeneralTreeNodeColor();
            //judgeMatrixView.UpdateErrorBoard(ea);
        }

        private void generalTest_OnNewMessage(object sender, EventArgs e)
        {
            if (!sender.GetType().Equals(typeof(GeneralTest)))
                return;
            NewMessageEventArgs ea = e as NewMessageEventArgs;
            generalLogList.AppendLog(new string[] { ea.rxMessage.DtTime.ToString("HH:mm:ss:fff"), ea.rxMessage.ToString() });
        }
        #endregion


        #endregion

        #region 主窗体部分
        private void SetAdvTreeNodeColor(Color color, Node node)
        {
            ElementStyle style = new ElementStyle();
            style.BackColor = color;
            node.Style = style;
        }

        //更新日志区列表
        private void mainTimer_Tick(object sender, EventArgs e)
        {
        } 

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadSelfTestView();
            LoadFCTTestView();
            LoadGeneralTestView();            
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            formAbout.ShowDialog();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {

        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(isUIStart)
            {
                DialogResult result = MessageBox.Show("正在测试中，请先停止测试！\n如果系统自检，则等待自检自动结束。", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }
            else
            {
                string tryCloseMsg = testSemaphore.Attempt(10, "关闭窗体");
                if (tryCloseMsg.Length > 0)
                {
                    MessageBox.Show(tryCloseMsg);
                    e.Cancel = true;
                    return;
                }
                try
                {
                    Quartz.Impl.StdScheduler scheduler = (Quartz.Impl.StdScheduler)SpringHelper.GetContext().GetObject("scheduler");
                    scheduler.Shutdown();
                    udp.UdpClose();
                }
                catch (Exception ee)
                {
                }
            }
        }

        private void selfRibbonTabItem_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectedPanel = selfTabControlPanel;
            this.tabControl.SelectedTab = selfTabItem;
        }

        private void fctRibbonTabItem_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectedPanel = fctTabControlPanel;
            this.tabControl.SelectedTab = fctTabItem;
            //LoadFCTTestView();
        }

        private void genneralRibbonTabItem_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectedPanel = genneralTabControlPanel;
            this.tabControl.SelectedTab = genneralTabItem;
            //LoadGeneralTestView();
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            if (isUIStart)
            {
                lblRunningTime.Text = "计时:" + Util.FormateDurationSecondsMaxHour2(GetRuningDuration(testType));
                if (GetRemainDuration(testType)<0)
                    lblRemainTime.Text = "剩余:000:00:00";
                else
                    lblRemainTime.Text = "剩余:" + Util.FormateDurationSecondsMaxHour2(GetRemainDuration(testType));
                lblStatus.ForeColor = Color.Blue;
                if (lblStatus.Text == "·")
                    lblStatus.Text = "··";
                else if (lblStatus.Text == "··")
                    lblStatus.Text = "···";
                else
                    lblStatus.Text = "·";
            }
        }

        private long GetRemainDuration(int type)
        {
            long time = 0;
            switch (type)
            {
                case SELF_CHECK:
                    time = selfTest.PlanRunningTime - selfTest.GetRuningDuration();
                    break;
                case FCT_TEST:
                    time = fctTest.PlanRunningTime - fctTest.GetRuningDuration();
                    break;
                case GENERAL_TEST:
                    time = generalTest.PlanRunningTime - generalTest.GetRuningDuration();
                    break;
            }
            return time;
        }

        private long GetRuningDuration(int type)
        {
            long time = 0;
            switch (type)
            {
                case SELF_CHECK:
                    time = selfTest.GetRuningDuration();
                    break;
                case FCT_TEST:
                    time = fctTest.GetRuningDuration();
                    break;
                case GENERAL_TEST:
                    time = generalTest.GetRuningDuration();
                    break;
            }
            return time;
        }
        #endregion
    }
}
