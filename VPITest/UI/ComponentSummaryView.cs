using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPITest.Model;
using Summer.System.Core;
using System.Drawing;
using VPITest.Common;

namespace VPITest.UI
{
    public class ComponentSummaryView : System.Windows.Forms.ListView
    {     
        public ComponentSummaryView()
            : base()
        {
            // 开启双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);

            this.FullRowSelect = true;
            this.GridLines = true;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.HideSelection = false;
            this.MultiSelect = false;
            this.ShowGroups = false;
            this.UseCompatibleStateImageBehavior = false;
            this.View = View.Details;

            this.Columns.Add("待测组件",200);
            this.Columns.Add("总次数", 100);
            this.Columns.Add("误码次数", 100);
            this.Columns.Add("丢包次数", 100);
            this.Columns.Add("中断次数", 100);
        }
        protected TestSemaphore testSemaphore;
        private Timer timer;
        public Timer Timer
        {
            get
            {
                return timer;
            }
            set
            {
                timer = value;
                if (timer != null)
                    value.Tick += new EventHandler(timer_Tick);
            }
        }

        protected Cabinet cabinet;

        public void Reset(string runningTest)
        {
            RunningTest = runningTest;
            this.Items.Clear();
            if (cabinet == null)
            {
                try
                {
                    cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
                }
                catch (Exception ee)
                {
                }
            }
            if (testSemaphore == null)
            {
                try
                {
                    testSemaphore = SpringHelper.GetObject<TestSemaphore>("testSemaphore");
                }
                catch (Exception ee)
                {
                }
            }            
            List<Component> list = new List<Component>();
            if (runningTest == TestSemaphore.FCT_RUNNING)
            {
                //list = cabinet.GetFctTestedComponentsList();
                foreach (var r in cabinet.Racks)
                {
                    foreach (var b in r.Boards)
                    {
                        foreach (var ct in b.ComponentTypes)
                        {
                            if (ct.Components != null && ct.Components.Count > 0)
                            {
                                foreach (var c in ct.Components)
                                {
                                    c.AllTestTimes = 0;
                                    c.ErrorPackageTimes = 0;
                                    c.LostPackageTimes = 0;
                                    c.InterruptTimes = 0;
                                }
                                list.AddRange(ct.Components);
                            }
                        }
                    }
                }
            }
            else if (runningTest == TestSemaphore.GENERAL_RUNNING)
            {
                foreach (var r in cabinet.Racks)
                {
                    foreach (var b in r.Boards)
                    {
                        foreach (var ct in b.ComponentTypes)
                        {
                            if (ct.Components != null && ct.Components.Count > 0)
                            {
                                foreach (var c in ct.Components)
                                {
                                    c.AllTestTimes = 0;
                                    c.ErrorPackageTimes = 0;
                                    c.LostPackageTimes = 0;
                                    c.InterruptTimes = 0;
                                }
                                list.AddRange(ct.Components);
                            }
                        }
                    }
                }
            }
            foreach (var c in list)
            {
                this.Items.Add(CreateItem(c));
            }
        }
        public string RunningTest;
        private void timer_Tick(object sender, EventArgs e)
        {
            if (testSemaphore == null || testSemaphore.RunningTestName.Length == 0 
                || testSemaphore.RunningTestName != RunningTest)
            {
                return;
            }
            this.BeginUpdate();
            for (int i = 0; i < this.Items.Count; i++)
            {
                ListViewItem lvi = this.Items[i];
                if (lvi.Tag != null && lvi.Tag is Component)
                {
                    Component c = lvi.Tag as Component;
                    lvi.SubItems[1].Text = string.Format("{0}",c.AllTestTimes);
                    if (c.AllTestTimes > 0)
                    {
                        //lvi.SubItems[1].BackColor = Color.AliceBlue;
                    }
                    if (c.ErrorPackageTimes > 0)
                    {
                        lvi.SubItems[2].BackColor = Color.OrangeRed;
                        lvi.SubItems[2].Text = string.Format("{0}", c.ErrorPackageTimes);
                    }
                    if (c.LostPackageTimes > 0)
                    {
                        lvi.SubItems[3].BackColor = Color.OrangeRed;
                        lvi.SubItems[3].Text = string.Format("{0}", c.LostPackageTimes);
                    }
                    if (c.InterruptTimes > 0)
                    {
                        lvi.SubItems[4].BackColor = Color.OrangeRed;
                        lvi.SubItems[4].Text = string.Format("{0}", c.InterruptTimes);
                    }
                }
            }
            this.EndUpdate();
        }

        protected ListViewItem CreateItem(Component c)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.UseItemStyleForSubItems = false;
            lvi.Text = string.Format("{0} - {1}",c.ParentComponentType.ParentBoard.EqName,c.EqName);
            lvi.SubItems.Add(string.Format("{0}",c.AllTestTimes));
            lvi.SubItems.Add(string.Format("{0}", c.ErrorPackageTimes));
            lvi.SubItems.Add(string.Format("{0}", c.LostPackageTimes));
            lvi.SubItems.Add(string.Format("{0}", c.InterruptTimes));
            lvi.Tag = c;
            return lvi;
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
