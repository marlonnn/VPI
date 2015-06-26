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
    public class JudgeMatrixView : System.Windows.Forms.ListView
    {
        protected Dictionary<Board, Board[]> judgeMatrix;
        protected Cabinet cabinet;
        protected TestSemaphore testSemaphore;
        private static string optionalCauseString = "o";

        public JudgeMatrixView()
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
        }

        public void Init()
        {
            if (this.Columns.Count > 0)
                return;

            cabinet = SpringHelper.GetObject<Cabinet>("cabinet");
            judgeMatrix = SpringHelper.GetObject<Dictionary<Board, Board[]>>("judgeMatrix");
            testSemaphore = SpringHelper.GetObject<TestSemaphore>("testSemaphore");
            this.Columns.Add("", 80);

            foreach (var r in cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    foreach (var bk in judgeMatrix.Keys)
                    {
                        if (bk.EqName == b.EqName)
                        {
                            ColumnHeader ch = new ColumnHeader();
                            ch.Text = bk.EqName;
                            ch.Width = 75;
                            ch.Tag = bk;
                            this.Columns.Add(ch);
                            break;
                        }
                    }
                }
            }
            foreach (var r in cabinet.Racks)
            {
                foreach (var b in r.Boards)
                {
                    Items.Add(CreateItem(b));
                }
            }
            Reset();
        }

        protected ListViewItem CreateItem(Board b)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = string.Format("{0}",b.EqName);
            lvi.Tag = b;
            lvi.UseItemStyleForSubItems = false;
            for (int i = 0; i < this.Columns.Count; i++)
            {
                ColumnHeader ch = this.Columns[i];
                if (ch.Tag != null && ch.Tag is Board)
                {
                    System.Windows.Forms.ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                    subItem.Text = "";
                    //判定矩阵中的columnBoard板卡是否会引起此板卡故障
                    foreach (var board in judgeMatrix[ch.Tag as Board])
                    {
                        if (board.EqName.Equals(b.EqName))
                        {
                            subItem.Text = optionalCauseString;
                            break;
                        }
                    }
                    lvi.SubItems.Add(subItem);
                }
            }
            lvi.Tag = b;
            return lvi;
        }

        public void Reset()
        {
            BeginUpdate();
            for (int i = 0; i < this.Items.Count; i++)
            {
                ListViewItem lvi = this.Items[i];
                for (int j = 1; j < lvi.SubItems.Count; j++)
                {
                    lvi.SubItems[j].BackColor = Color.White;
                }
            }
            EndUpdate();
        }

        public void UpdateErrorBoard(BoardStatusEventArgs e)
        {
            if (e.IsMessageSource)
            {
                UpdateErrorBoard(e.Board);
            }
        }

        protected void UpdateErrorBoard(Board errorBoard)
        {
            for (int i = 1; i < Columns.Count; i++)
            {
                Board b = Columns[i].Tag as Board;
                if (b != null && b.EqName == errorBoard.EqName)
                {
                    for (int j = 0; j < this.Items.Count; j++)
                    {
                        ListViewItem lvi = this.Items[j];
                        if (lvi.SubItems[i].Text == optionalCauseString)
                        {
                            lvi.SubItems[i].BackColor = Color.Red;
                        }
                        else
                        {
                            lvi.SubItems[i].BackColor = Color.PaleVioletRed;
                        }
                    }
                }
            }
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

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
        static int tick = 0;
        //static string[] roolingChar = new string[] { @"●", @"    ", @"    ", @"    ", @"●", @"   ", @"   ", @"   " };
        static string[] roolingChar = new string[] { @"->", @"  ", @"  ", @"  "};
        //static Color[] roolingColor = new Color[] { Color.Green, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White };

        private void timer_Tick(object sender, EventArgs e)
        {
            //只有综合测试过程中才更新
            if (testSemaphore.RunningTestName == TestSemaphore.GENERAL_RUNNING)
            {
                if (tick == int.MaxValue)
                    tick = 0;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (Items[i].Tag != null && Items[i].Tag is Board)
                    {
                        Items[i].Text = roolingChar[tick++ % roolingChar.Length] + (Items[i].Tag as Board).EqName;
                        //Items[i].SubItems[0].BackColor = roolingColor[tick++ % roolingColor.Length];
                    }
                }
            }
        }
    }
}
