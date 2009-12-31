namespace RSLTEditor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rCOLHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.lstRoutes = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.btnRouteAdd = new System.Windows.Forms.Button();
			this.btnRouteDelete = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtMatrix34 = new System.Windows.Forms.TextBox();
			this.txtMatrix33 = new System.Windows.Forms.TextBox();
			this.txtMatrix32 = new System.Windows.Forms.TextBox();
			this.txtMatrix31 = new System.Windows.Forms.TextBox();
			this.txtMatrix24 = new System.Windows.Forms.TextBox();
			this.txtMatrix14 = new System.Windows.Forms.TextBox();
			this.txtMatrix23 = new System.Windows.Forms.TextBox();
			this.txtMatrix22 = new System.Windows.Forms.TextBox();
			this.txtMatrix21 = new System.Windows.Forms.TextBox();
			this.txtMatrix13 = new System.Windows.Forms.TextBox();
			this.txtMatrix12 = new System.Windows.Forms.TextBox();
			this.txtMatrix11 = new System.Windows.Forms.TextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.lstContainers = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.btnContainerAdd = new System.Windows.Forms.Button();
			this.btnContainerDelete = new System.Windows.Forms.Button();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.lstEffects = new System.Windows.Forms.ListView();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.btnEffectAdd = new System.Windows.Forms.Button();
			this.btnEffectDelete = new System.Windows.Forms.Button();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.lstIKTargets = new System.Windows.Forms.ListView();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
			this.btnIKTargetAdd = new System.Windows.Forms.Button();
			this.btnIKTargetDelete = new System.Windows.Forms.Button();
			this.btnCommit = new System.Windows.Forms.Button();
			this.txtSlotName = new System.Windows.Forms.TextBox();
			this.txtBoneName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtFlags = new System.Windows.Forms.TextBox();
			this.menuStrip1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.SystemColors.Menu;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.Size = new System.Drawing.Size(690, 24);
			this.menuStrip1.TabIndex = 31;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.newToolStripMenuItem.Text = "New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.saveAsToolStripMenuItem.Text = "Save As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(122, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rCOLHeaderToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// rCOLHeaderToolStripMenuItem
			// 
			this.rCOLHeaderToolStripMenuItem.Name = "rCOLHeaderToolStripMenuItem";
			this.rCOLHeaderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.rCOLHeaderToolStripMenuItem.Text = "RCOL Header";
			this.rCOLHeaderToolStripMenuItem.Click += new System.EventHandler(this.rCOLHeaderToolStripMenuItem_Click);
			// 
			// lstRoutes
			// 
			this.lstRoutes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
			this.lstRoutes.FullRowSelect = true;
			this.lstRoutes.GridLines = true;
			this.lstRoutes.HideSelection = false;
			this.lstRoutes.Location = new System.Drawing.Point(6, 4);
			this.lstRoutes.Name = "lstRoutes";
			this.lstRoutes.Size = new System.Drawing.Size(219, 265);
			this.lstRoutes.TabIndex = 32;
			this.lstRoutes.UseCompatibleStateImageBehavior = false;
			this.lstRoutes.View = System.Windows.Forms.View.Details;
			this.lstRoutes.SelectedIndexChanged += new System.EventHandler(this.lstRoutes_SelectedIndexChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "#";
			this.columnHeader3.Width = 50;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Slot Name";
			this.columnHeader4.Width = 150;
			// 
			// btnRouteAdd
			// 
			this.btnRouteAdd.Enabled = false;
			this.btnRouteAdd.Location = new System.Drawing.Point(150, 275);
			this.btnRouteAdd.Name = "btnRouteAdd";
			this.btnRouteAdd.Size = new System.Drawing.Size(75, 23);
			this.btnRouteAdd.TabIndex = 44;
			this.btnRouteAdd.Text = "Add Item";
			this.btnRouteAdd.UseVisualStyleBackColor = true;
			this.btnRouteAdd.Click += new System.EventHandler(this.btnRouteAdd_Click);
			// 
			// btnRouteDelete
			// 
			this.btnRouteDelete.Enabled = false;
			this.btnRouteDelete.Location = new System.Drawing.Point(8, 275);
			this.btnRouteDelete.Name = "btnRouteDelete";
			this.btnRouteDelete.Size = new System.Drawing.Size(91, 23);
			this.btnRouteDelete.TabIndex = 45;
			this.btnRouteDelete.Text = "Delete Item";
			this.btnRouteDelete.UseVisualStyleBackColor = true;
			this.btnRouteDelete.Click += new System.EventHandler(this.btnRouteDelete_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtMatrix34);
			this.groupBox3.Controls.Add(this.txtMatrix33);
			this.groupBox3.Controls.Add(this.txtMatrix32);
			this.groupBox3.Controls.Add(this.txtMatrix31);
			this.groupBox3.Controls.Add(this.txtMatrix24);
			this.groupBox3.Controls.Add(this.txtMatrix14);
			this.groupBox3.Controls.Add(this.txtMatrix23);
			this.groupBox3.Controls.Add(this.txtMatrix22);
			this.groupBox3.Controls.Add(this.txtMatrix21);
			this.groupBox3.Controls.Add(this.txtMatrix13);
			this.groupBox3.Controls.Add(this.txtMatrix12);
			this.groupBox3.Controls.Add(this.txtMatrix11);
			this.groupBox3.Location = new System.Drawing.Point(245, 124);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(435, 99);
			this.groupBox3.TabIndex = 48;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Matrix";
			// 
			// txtMatrix34
			// 
			this.txtMatrix34.Location = new System.Drawing.Point(324, 71);
			this.txtMatrix34.Name = "txtMatrix34";
			this.txtMatrix34.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix34.TabIndex = 11;
			this.txtMatrix34.Text = "0";
			// 
			// txtMatrix33
			// 
			this.txtMatrix33.Location = new System.Drawing.Point(218, 71);
			this.txtMatrix33.Name = "txtMatrix33";
			this.txtMatrix33.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix33.TabIndex = 10;
			this.txtMatrix33.Text = "0";
			// 
			// txtMatrix32
			// 
			this.txtMatrix32.Location = new System.Drawing.Point(112, 71);
			this.txtMatrix32.Name = "txtMatrix32";
			this.txtMatrix32.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix32.TabIndex = 9;
			this.txtMatrix32.Text = "0";
			// 
			// txtMatrix31
			// 
			this.txtMatrix31.Location = new System.Drawing.Point(6, 71);
			this.txtMatrix31.Name = "txtMatrix31";
			this.txtMatrix31.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix31.TabIndex = 8;
			this.txtMatrix31.Text = "0";
			// 
			// txtMatrix24
			// 
			this.txtMatrix24.Location = new System.Drawing.Point(324, 45);
			this.txtMatrix24.Name = "txtMatrix24";
			this.txtMatrix24.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix24.TabIndex = 7;
			this.txtMatrix24.Text = "0";
			// 
			// txtMatrix14
			// 
			this.txtMatrix14.Location = new System.Drawing.Point(324, 19);
			this.txtMatrix14.Name = "txtMatrix14";
			this.txtMatrix14.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix14.TabIndex = 6;
			this.txtMatrix14.Text = "0";
			// 
			// txtMatrix23
			// 
			this.txtMatrix23.Location = new System.Drawing.Point(218, 45);
			this.txtMatrix23.Name = "txtMatrix23";
			this.txtMatrix23.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix23.TabIndex = 5;
			this.txtMatrix23.Text = "0";
			// 
			// txtMatrix22
			// 
			this.txtMatrix22.Location = new System.Drawing.Point(112, 45);
			this.txtMatrix22.Name = "txtMatrix22";
			this.txtMatrix22.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix22.TabIndex = 4;
			this.txtMatrix22.Text = "0";
			// 
			// txtMatrix21
			// 
			this.txtMatrix21.Location = new System.Drawing.Point(6, 45);
			this.txtMatrix21.Name = "txtMatrix21";
			this.txtMatrix21.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix21.TabIndex = 3;
			this.txtMatrix21.Text = "0";
			// 
			// txtMatrix13
			// 
			this.txtMatrix13.Location = new System.Drawing.Point(218, 19);
			this.txtMatrix13.Name = "txtMatrix13";
			this.txtMatrix13.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix13.TabIndex = 2;
			this.txtMatrix13.Text = "0";
			// 
			// txtMatrix12
			// 
			this.txtMatrix12.Location = new System.Drawing.Point(112, 19);
			this.txtMatrix12.Name = "txtMatrix12";
			this.txtMatrix12.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix12.TabIndex = 1;
			this.txtMatrix12.Text = "0";
			// 
			// txtMatrix11
			// 
			this.txtMatrix11.Location = new System.Drawing.Point(6, 19);
			this.txtMatrix11.Name = "txtMatrix11";
			this.txtMatrix11.Size = new System.Drawing.Size(100, 20);
			this.txtMatrix11.TabIndex = 0;
			this.txtMatrix11.Text = "0";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 360);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(690, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 49;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(675, 17);
			this.toolStripStatusLabel1.Spring = true;
			this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Location = new System.Drawing.Point(0, 27);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(239, 329);
			this.tabControl1.TabIndex = 50;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.lstRoutes);
			this.tabPage1.Controls.Add(this.btnRouteAdd);
			this.tabPage1.Controls.Add(this.btnRouteDelete);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(231, 303);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Routes";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.lstContainers);
			this.tabPage2.Controls.Add(this.btnContainerAdd);
			this.tabPage2.Controls.Add(this.btnContainerDelete);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(231, 303);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Containers";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// lstContainers
			// 
			this.lstContainers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.lstContainers.FullRowSelect = true;
			this.lstContainers.GridLines = true;
			this.lstContainers.HideSelection = false;
			this.lstContainers.Location = new System.Drawing.Point(6, 4);
			this.lstContainers.Name = "lstContainers";
			this.lstContainers.Size = new System.Drawing.Size(219, 265);
			this.lstContainers.TabIndex = 46;
			this.lstContainers.UseCompatibleStateImageBehavior = false;
			this.lstContainers.View = System.Windows.Forms.View.Details;
			this.lstContainers.SelectedIndexChanged += new System.EventHandler(this.lstContainers_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "#";
			this.columnHeader1.Width = 50;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Slot Name";
			this.columnHeader2.Width = 150;
			// 
			// btnContainerAdd
			// 
			this.btnContainerAdd.Enabled = false;
			this.btnContainerAdd.Location = new System.Drawing.Point(150, 275);
			this.btnContainerAdd.Name = "btnContainerAdd";
			this.btnContainerAdd.Size = new System.Drawing.Size(75, 23);
			this.btnContainerAdd.TabIndex = 47;
			this.btnContainerAdd.Text = "Add Item";
			this.btnContainerAdd.UseVisualStyleBackColor = true;
			this.btnContainerAdd.Click += new System.EventHandler(this.btnContainerAdd_Click);
			// 
			// btnContainerDelete
			// 
			this.btnContainerDelete.Enabled = false;
			this.btnContainerDelete.Location = new System.Drawing.Point(8, 275);
			this.btnContainerDelete.Name = "btnContainerDelete";
			this.btnContainerDelete.Size = new System.Drawing.Size(91, 23);
			this.btnContainerDelete.TabIndex = 48;
			this.btnContainerDelete.Text = "Delete Item";
			this.btnContainerDelete.UseVisualStyleBackColor = true;
			this.btnContainerDelete.Click += new System.EventHandler(this.btnContainerDelete_Click);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.lstEffects);
			this.tabPage3.Controls.Add(this.btnEffectAdd);
			this.tabPage3.Controls.Add(this.btnEffectDelete);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(231, 303);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Effects";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// lstEffects
			// 
			this.lstEffects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
			this.lstEffects.FullRowSelect = true;
			this.lstEffects.GridLines = true;
			this.lstEffects.HideSelection = false;
			this.lstEffects.Location = new System.Drawing.Point(6, 4);
			this.lstEffects.Name = "lstEffects";
			this.lstEffects.Size = new System.Drawing.Size(219, 265);
			this.lstEffects.TabIndex = 46;
			this.lstEffects.UseCompatibleStateImageBehavior = false;
			this.lstEffects.View = System.Windows.Forms.View.Details;
			this.lstEffects.SelectedIndexChanged += new System.EventHandler(this.lstEffects_SelectedIndexChanged);
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "#";
			this.columnHeader5.Width = 50;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Slot Name";
			this.columnHeader6.Width = 150;
			// 
			// btnEffectAdd
			// 
			this.btnEffectAdd.Enabled = false;
			this.btnEffectAdd.Location = new System.Drawing.Point(150, 275);
			this.btnEffectAdd.Name = "btnEffectAdd";
			this.btnEffectAdd.Size = new System.Drawing.Size(75, 23);
			this.btnEffectAdd.TabIndex = 47;
			this.btnEffectAdd.Text = "Add Item";
			this.btnEffectAdd.UseVisualStyleBackColor = true;
			this.btnEffectAdd.Click += new System.EventHandler(this.btnEffectAdd_Click);
			// 
			// btnEffectDelete
			// 
			this.btnEffectDelete.Enabled = false;
			this.btnEffectDelete.Location = new System.Drawing.Point(8, 275);
			this.btnEffectDelete.Name = "btnEffectDelete";
			this.btnEffectDelete.Size = new System.Drawing.Size(91, 23);
			this.btnEffectDelete.TabIndex = 48;
			this.btnEffectDelete.Text = "Delete Item";
			this.btnEffectDelete.UseVisualStyleBackColor = true;
			this.btnEffectDelete.Click += new System.EventHandler(this.btnEffectDelete_Click);
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.lstIKTargets);
			this.tabPage4.Controls.Add(this.btnIKTargetAdd);
			this.tabPage4.Controls.Add(this.btnIKTargetDelete);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(231, 303);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "IKTargets";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// lstIKTargets
			// 
			this.lstIKTargets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
			this.lstIKTargets.FullRowSelect = true;
			this.lstIKTargets.GridLines = true;
			this.lstIKTargets.HideSelection = false;
			this.lstIKTargets.Location = new System.Drawing.Point(6, 4);
			this.lstIKTargets.Name = "lstIKTargets";
			this.lstIKTargets.Size = new System.Drawing.Size(219, 265);
			this.lstIKTargets.TabIndex = 46;
			this.lstIKTargets.UseCompatibleStateImageBehavior = false;
			this.lstIKTargets.View = System.Windows.Forms.View.Details;
			this.lstIKTargets.SelectedIndexChanged += new System.EventHandler(this.lstIKTargets_SelectedIndexChanged);
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "#";
			this.columnHeader7.Width = 50;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Slot Name";
			this.columnHeader8.Width = 150;
			// 
			// btnIKTargetAdd
			// 
			this.btnIKTargetAdd.Enabled = false;
			this.btnIKTargetAdd.Location = new System.Drawing.Point(150, 275);
			this.btnIKTargetAdd.Name = "btnIKTargetAdd";
			this.btnIKTargetAdd.Size = new System.Drawing.Size(75, 23);
			this.btnIKTargetAdd.TabIndex = 47;
			this.btnIKTargetAdd.Text = "Add Item";
			this.btnIKTargetAdd.UseVisualStyleBackColor = true;
			this.btnIKTargetAdd.Click += new System.EventHandler(this.btnIKTargetAdd_Click);
			// 
			// btnIKTargetDelete
			// 
			this.btnIKTargetDelete.Enabled = false;
			this.btnIKTargetDelete.Location = new System.Drawing.Point(8, 275);
			this.btnIKTargetDelete.Name = "btnIKTargetDelete";
			this.btnIKTargetDelete.Size = new System.Drawing.Size(91, 23);
			this.btnIKTargetDelete.TabIndex = 48;
			this.btnIKTargetDelete.Text = "Delete Item";
			this.btnIKTargetDelete.UseVisualStyleBackColor = true;
			this.btnIKTargetDelete.Click += new System.EventHandler(this.btnIKTargetDelete_Click);
			// 
			// btnCommit
			// 
			this.btnCommit.Enabled = false;
			this.btnCommit.Location = new System.Drawing.Point(605, 229);
			this.btnCommit.Name = "btnCommit";
			this.btnCommit.Size = new System.Drawing.Size(75, 23);
			this.btnCommit.TabIndex = 51;
			this.btnCommit.Text = "Commit";
			this.btnCommit.UseVisualStyleBackColor = true;
			// 
			// txtSlotName
			// 
			this.txtSlotName.Location = new System.Drawing.Point(320, 46);
			this.txtSlotName.Name = "txtSlotName";
			this.txtSlotName.Size = new System.Drawing.Size(137, 20);
			this.txtSlotName.TabIndex = 52;
			// 
			// txtBoneName
			// 
			this.txtBoneName.Location = new System.Drawing.Point(320, 72);
			this.txtBoneName.Name = "txtBoneName";
			this.txtBoneName.Size = new System.Drawing.Size(137, 20);
			this.txtBoneName.TabIndex = 53;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(255, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 54;
			this.label1.Text = "Slot Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(248, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 55;
			this.label2.Text = "Bone Name:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(279, 102);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 56;
			this.label3.Text = "Flags:";
			// 
			// txtFlags
			// 
			this.txtFlags.Location = new System.Drawing.Point(320, 98);
			this.txtFlags.Name = "txtFlags";
			this.txtFlags.Size = new System.Drawing.Size(100, 20);
			this.txtFlags.TabIndex = 57;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(690, 382);
			this.Controls.Add(this.txtFlags);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtBoneName);
			this.Controls.Add(this.txtSlotName);
			this.Controls.Add(this.btnCommit);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "Slot Machine RSLT Editor by Delphy";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ListView lstRoutes;
        private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button btnRouteAdd;
		private System.Windows.Forms.Button btnRouteDelete;
		private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtMatrix23;
        private System.Windows.Forms.TextBox txtMatrix22;
        private System.Windows.Forms.TextBox txtMatrix21;
        private System.Windows.Forms.TextBox txtMatrix13;
        private System.Windows.Forms.TextBox txtMatrix12;
        private System.Windows.Forms.TextBox txtMatrix11;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rCOLHeaderToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.TextBox txtMatrix24;
		private System.Windows.Forms.TextBox txtMatrix14;
		private System.Windows.Forms.TextBox txtMatrix34;
		private System.Windows.Forms.TextBox txtMatrix33;
		private System.Windows.Forms.TextBox txtMatrix32;
		private System.Windows.Forms.TextBox txtMatrix31;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.ListView lstContainers;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button btnContainerAdd;
		private System.Windows.Forms.Button btnContainerDelete;
		private System.Windows.Forms.ListView lstEffects;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.Button btnEffectAdd;
		private System.Windows.Forms.Button btnEffectDelete;
		private System.Windows.Forms.ListView lstIKTargets;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.Button btnIKTargetAdd;
		private System.Windows.Forms.Button btnIKTargetDelete;
		private System.Windows.Forms.Button btnCommit;
		private System.Windows.Forms.TextBox txtSlotName;
		private System.Windows.Forms.TextBox txtBoneName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtFlags;
    }
}

