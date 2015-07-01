namespace VPITest.UI
{
    partial class FormFind
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFind));
            this.ribbonControl1 = new DevComponents.DotNetBar.RibbonControl();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.cbFinishResult = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.label6 = new System.Windows.Forms.Label();
            this.btnQuery = new DevComponents.DotNetBar.ButtonX();
            this.tbBoardSn = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.tbTestKey = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbBoardType = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.label3 = new System.Windows.Forms.Label();
            this.cbNoLimitDate = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.label2 = new System.Windows.Forms.Label();
            this.dtiEnd = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.label1 = new System.Windows.Forms.Label();
            this.dtiBegin = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.cbLastMonth = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.dataGridView = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.cbCopyDiagnose = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.btnCopyAll = new DevComponents.DotNetBar.ButtonX();
            this.btnCopySelected = new DevComponents.DotNetBar.ButtonX();
            this.Column10 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column1 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column2 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column3 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column4 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column5 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column7 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column6 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.clnTestedItems = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column8 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column11 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column9 = new DevComponents.DotNetBar.Controls.DataGridViewButtonXColumn();
            this.Column12 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Column13 = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.groupPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtiEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtiBegin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.BackgroundImagePosition = DevComponents.DotNetBar.eBackgroundImagePosition.Center;
            // 
            // 
            // 
            this.ribbonControl1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonControl1.CaptionVisible = true;
            this.ribbonControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ribbonControl1.KeyTipsFont = new System.Drawing.Font("Tahoma", 7F);
            this.ribbonControl1.Location = new System.Drawing.Point(5, 1);
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.ribbonControl1.Size = new System.Drawing.Size(1079, 39);
            this.ribbonControl1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonControl1.SystemText.MaximizeRibbonText = "&Maximize the Ribbon";
            this.ribbonControl1.SystemText.MinimizeRibbonText = "Mi&nimize the Ribbon";
            this.ribbonControl1.SystemText.QatAddItemText = "&Add to Quick Access Toolbar";
            this.ribbonControl1.SystemText.QatCustomizeMenuLabel = "<b>Customize Quick Access Toolbar</b>";
            this.ribbonControl1.SystemText.QatCustomizeText = "&Customize Quick Access Toolbar...";
            this.ribbonControl1.SystemText.QatDialogAddButton = "&Add >>";
            this.ribbonControl1.SystemText.QatDialogCancelButton = "Cancel";
            this.ribbonControl1.SystemText.QatDialogCaption = "Customize Quick Access Toolbar";
            this.ribbonControl1.SystemText.QatDialogCategoriesLabel = "&Choose commands from:";
            this.ribbonControl1.SystemText.QatDialogOkButton = "OK";
            this.ribbonControl1.SystemText.QatDialogPlacementCheckbox = "&Place Quick Access Toolbar below the Ribbon";
            this.ribbonControl1.SystemText.QatDialogRemoveButton = "&Remove";
            this.ribbonControl1.SystemText.QatPlaceAboveRibbonText = "&Place Quick Access Toolbar above the Ribbon";
            this.ribbonControl1.SystemText.QatPlaceBelowRibbonText = "&Place Quick Access Toolbar below the Ribbon";
            this.ribbonControl1.SystemText.QatRemoveItemText = "&Remove from Quick Access Toolbar";
            this.ribbonControl1.TabGroupHeight = 14;
            this.ribbonControl1.TabIndex = 1;
            this.ribbonControl1.Text = "ribbonControl1";
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.cbFinishResult);
            this.groupPanel1.Controls.Add(this.label6);
            this.groupPanel1.Controls.Add(this.btnQuery);
            this.groupPanel1.Controls.Add(this.tbBoardSn);
            this.groupPanel1.Controls.Add(this.tbTestKey);
            this.groupPanel1.Controls.Add(this.label5);
            this.groupPanel1.Controls.Add(this.label4);
            this.groupPanel1.Controls.Add(this.cbBoardType);
            this.groupPanel1.Controls.Add(this.label3);
            this.groupPanel1.Controls.Add(this.cbNoLimitDate);
            this.groupPanel1.Controls.Add(this.label2);
            this.groupPanel1.Controls.Add(this.dtiEnd);
            this.groupPanel1.Controls.Add(this.label1);
            this.groupPanel1.Controls.Add(this.dtiBegin);
            this.groupPanel1.Controls.Add(this.cbLastMonth);
            this.groupPanel1.DisabledBackColor = System.Drawing.Color.Empty;
            this.groupPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupPanel1.Location = new System.Drawing.Point(5, 40);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(1079, 77);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 2;
            // 
            // cbFinishResult
            // 
            this.cbFinishResult.DisplayMember = "Text";
            this.cbFinishResult.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbFinishResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFinishResult.FormattingEnabled = true;
            this.cbFinishResult.ItemHeight = 15;
            this.cbFinishResult.Location = new System.Drawing.Point(503, 38);
            this.cbFinishResult.Name = "cbFinishResult";
            this.cbFinishResult.Size = new System.Drawing.Size(121, 21);
            this.cbFinishResult.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbFinishResult.TabIndex = 26;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label6.Location = new System.Drawing.Point(447, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 25;
            this.label6.Text = "结束原因";
            // 
            // btnQuery
            // 
            this.btnQuery.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnQuery.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnQuery.Location = new System.Drawing.Point(1011, 13);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 44);
            this.btnQuery.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnQuery.TabIndex = 24;
            this.btnQuery.Text = "查询";
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // tbBoardSn
            // 
            // 
            // 
            // 
            this.tbBoardSn.Border.Class = "TextBoxBorder";
            this.tbBoardSn.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.tbBoardSn.Location = new System.Drawing.Point(286, 39);
            this.tbBoardSn.Name = "tbBoardSn";
            this.tbBoardSn.Size = new System.Drawing.Size(125, 21);
            this.tbBoardSn.TabIndex = 21;
            // 
            // tbTestKey
            // 
            // 
            // 
            // 
            this.tbTestKey.Border.Class = "TextBoxBorder";
            this.tbTestKey.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.tbTestKey.Location = new System.Drawing.Point(54, 39);
            this.tbTestKey.Name = "tbTestKey";
            this.tbTestKey.Size = new System.Drawing.Size(128, 21);
            this.tbTestKey.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label5.Location = new System.Drawing.Point(231, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "板卡S/N号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label4.Location = new System.Drawing.Point(3, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "测试序号";
            // 
            // cbBoardType
            // 
            this.cbBoardType.DisplayMember = "Text";
            this.cbBoardType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbBoardType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoardType.FormattingEnabled = true;
            this.cbBoardType.ItemHeight = 15;
            this.cbBoardType.Location = new System.Drawing.Point(729, 4);
            this.cbBoardType.Name = "cbBoardType";
            this.cbBoardType.Size = new System.Drawing.Size(121, 21);
            this.cbBoardType.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbBoardType.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label3.Location = new System.Drawing.Point(668, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "板卡类型";
            // 
            // cbNoLimitDate
            // 
            // 
            // 
            // 
            this.cbNoLimitDate.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbNoLimitDate.Checked = true;
            this.cbNoLimitDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNoLimitDate.CheckValue = "Y";
            this.cbNoLimitDate.Location = new System.Drawing.Point(3, 4);
            this.cbNoLimitDate.Name = "cbNoLimitDate";
            this.cbNoLimitDate.Size = new System.Drawing.Size(100, 23);
            this.cbNoLimitDate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbNoLimitDate.TabIndex = 12;
            this.cbNoLimitDate.Text = "全部时间段";
            this.cbNoLimitDate.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(447, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "结束日期";
            // 
            // dtiEnd
            // 
            // 
            // 
            // 
            this.dtiEnd.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtiEnd.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiEnd.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtiEnd.ButtonDropDown.Visible = true;
            this.dtiEnd.Format = DevComponents.Editors.eDateTimePickerFormat.Long;
            this.dtiEnd.IsPopupCalendarOpen = false;
            this.dtiEnd.Location = new System.Drawing.Point(498, 7);
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtiEnd.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiEnd.MonthCalendar.CalendarDimensions = new System.Drawing.Size(1, 1);
            this.dtiEnd.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiEnd.MonthCalendar.DisplayMonth = new System.DateTime(2015, 6, 1, 0, 0, 0, 0);
            this.dtiEnd.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtiEnd.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiEnd.MonthCalendar.TodayButtonVisible = true;
            this.dtiEnd.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtiEnd.Name = "dtiEnd";
            this.dtiEnd.Size = new System.Drawing.Size(126, 21);
            this.dtiEnd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.dtiEnd.TabIndex = 10;
            this.dtiEnd.Value = new System.DateTime(2015, 6, 2, 8, 52, 49, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.Location = new System.Drawing.Point(234, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "开始日期";
            // 
            // dtiBegin
            // 
            // 
            // 
            // 
            this.dtiBegin.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtiBegin.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiBegin.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtiBegin.ButtonDropDown.Visible = true;
            this.dtiBegin.Format = DevComponents.Editors.eDateTimePickerFormat.Long;
            this.dtiBegin.IsPopupCalendarOpen = false;
            this.dtiBegin.Location = new System.Drawing.Point(285, 6);
            // 
            // 
            // 
            this.dtiBegin.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiBegin.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtiBegin.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiBegin.MonthCalendar.CalendarDimensions = new System.Drawing.Size(1, 1);
            this.dtiBegin.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtiBegin.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiBegin.MonthCalendar.DisplayMonth = new System.DateTime(2015, 6, 1, 0, 0, 0, 0);
            this.dtiBegin.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtiBegin.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiBegin.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtiBegin.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiBegin.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtiBegin.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtiBegin.MonthCalendar.TodayButtonVisible = true;
            this.dtiBegin.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtiBegin.Name = "dtiBegin";
            this.dtiBegin.Size = new System.Drawing.Size(128, 21);
            this.dtiBegin.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.dtiBegin.TabIndex = 8;
            this.dtiBegin.Value = new System.DateTime(2015, 6, 2, 8, 52, 35, 0);
            // 
            // cbLastMonth
            // 
            // 
            // 
            // 
            this.cbLastMonth.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbLastMonth.Location = new System.Drawing.Point(109, 4);
            this.cbLastMonth.Name = "cbLastMonth";
            this.cbLastMonth.Size = new System.Drawing.Size(100, 23);
            this.cbLastMonth.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbLastMonth.TabIndex = 7;
            this.cbLastMonth.Text = "近一月";
            this.cbLastMonth.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column10,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column7,
            this.Column6,
            this.clnTestedItems,
            this.Column8,
            this.Column11,
            this.Column9,
            this.Column12,
            this.Column13});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(21)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EnableHeadersVisualStyles = false;
            this.dataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridView.HighlightSelectedColumnHeaders = false;
            this.dataGridView.Location = new System.Drawing.Point(5, 117);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView.RowHeadersWidth = 10;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1079, 430);
            this.dataGridView.TabIndex = 3;
            // 
            // groupPanel2
            // 
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.cbCopyDiagnose);
            this.groupPanel2.Controls.Add(this.btnCopyAll);
            this.groupPanel2.Controls.Add(this.btnCopySelected);
            this.groupPanel2.DisabledBackColor = System.Drawing.Color.Empty;
            this.groupPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupPanel2.Location = new System.Drawing.Point(5, 547);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(1079, 58);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel2.TabIndex = 4;
            // 
            // cbCopyDiagnose
            // 
            // 
            // 
            // 
            this.cbCopyDiagnose.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbCopyDiagnose.Location = new System.Drawing.Point(6, 14);
            this.cbCopyDiagnose.Name = "cbCopyDiagnose";
            this.cbCopyDiagnose.Size = new System.Drawing.Size(271, 21);
            this.cbCopyDiagnose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbCopyDiagnose.TabIndex = 27;
            this.cbCopyDiagnose.Text = "同时导出诊断文件（诊断报告和诊断日志）";
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCopyAll.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCopyAll.Location = new System.Drawing.Point(460, 14);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(117, 26);
            this.btnCopyAll.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCopyAll.TabIndex = 26;
            this.btnCopyAll.Text = "导出全部";
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // btnCopySelected
            // 
            this.btnCopySelected.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCopySelected.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCopySelected.Location = new System.Drawing.Point(296, 14);
            this.btnCopySelected.Name = "btnCopySelected";
            this.btnCopySelected.Size = new System.Drawing.Size(117, 26);
            this.btnCopySelected.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCopySelected.TabIndex = 25;
            this.btnCopySelected.Text = "导出选中";
            this.btnCopySelected.Click += new System.EventHandler(this.btnCopySelected_Click);
            // 
            // Column10
            // 
            this.Column10.HeaderText = "选择";
            this.Column10.Name = "Column10";
            this.Column10.Width = 50;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "测试序号";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "测试时间";
            this.Column2.Name = "Column2";
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column2.TextAlignment = System.Drawing.StringAlignment.Center;
            this.Column2.Width = 150;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "测试时长";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "测试人员";
            this.Column4.Name = "Column4";
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "板卡类型";
            this.Column5.Name = "Column5";
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "板卡名称";
            this.Column7.Name = "Column7";
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "板卡S/N号";
            this.Column6.Name = "Column6";
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column6.Width = 200;
            // 
            // clnTestedItems
            // 
            this.clnTestedItems.HeaderText = "测试项目";
            this.clnTestedItems.Name = "clnTestedItems";
            this.clnTestedItems.Width = 200;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "测试结果";
            this.Column8.Name = "Column8";
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column11
            // 
            this.Column11.HeaderText = "结束原因";
            this.Column11.Name = "Column11";
            this.Column11.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column9
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column9.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column9.HeaderText = "用户报告";
            this.Column9.Name = "Column9";
            this.Column9.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column9.Text = null;
            this.Column9.Width = 380;
            // 
            // Column12
            // 
            this.Column12.HeaderText = "诊断报告";
            this.Column12.Name = "Column12";
            this.Column12.Visible = false;
            // 
            // Column13
            // 
            this.Column13.HeaderText = "诊断日志";
            this.Column13.Name = "Column13";
            this.Column13.Visible = false;
            // 
            // FormFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 617);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.groupPanel2);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormFind";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据查询";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormFind_Load);
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtiEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtiBegin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.RibbonControl ribbonControl1;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridView;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbNoLimitDate;
        private System.Windows.Forms.Label label2;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtiEnd;
        private System.Windows.Forms.Label label1;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtiBegin;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbLastMonth;
        private DevComponents.DotNetBar.Controls.TextBoxX tbBoardSn;
        private DevComponents.DotNetBar.Controls.TextBoxX tbTestKey;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbBoardType;
        private System.Windows.Forms.Label label3;
        private DevComponents.DotNetBar.ButtonX btnQuery;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private DevComponents.DotNetBar.ButtonX btnCopySelected;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbCopyDiagnose;
        private DevComponents.DotNetBar.ButtonX btnCopyAll;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbFinishResult;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column10;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column1;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column2;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column3;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column4;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column5;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column7;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column6;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn clnTestedItems;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column8;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column11;
        private DevComponents.DotNetBar.Controls.DataGridViewButtonXColumn Column9;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column12;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Column13;
    }
}
