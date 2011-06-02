namespace InstallHelper
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.installToPatternsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToHacksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToSkinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToMiscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToRootFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.picRemove = new System.Windows.Forms.PictureBox();
			this.picAccept = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblHeader = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.studioShieldButton1 = new StudioControls.Controls.StudioShieldButton();
			this.contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picRemove)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picAccept)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToPatternsToolStripMenuItem,
            this.installToHacksToolStripMenuItem,
            this.installToSkinsToolStripMenuItem,
            this.installToMiscToolStripMenuItem,
            this.installToRootFolderToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(179, 114);
			// 
			// installToPatternsToolStripMenuItem
			// 
			this.installToPatternsToolStripMenuItem.Name = "installToPatternsToolStripMenuItem";
			this.installToPatternsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.installToPatternsToolStripMenuItem.Text = "Install to Patterns";
			// 
			// installToHacksToolStripMenuItem
			// 
			this.installToHacksToolStripMenuItem.Name = "installToHacksToolStripMenuItem";
			this.installToHacksToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.installToHacksToolStripMenuItem.Text = "Install to Hacks";
			// 
			// installToSkinsToolStripMenuItem
			// 
			this.installToSkinsToolStripMenuItem.Name = "installToSkinsToolStripMenuItem";
			this.installToSkinsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.installToSkinsToolStripMenuItem.Text = "Install to Skins";
			// 
			// installToMiscToolStripMenuItem
			// 
			this.installToMiscToolStripMenuItem.Name = "installToMiscToolStripMenuItem";
			this.installToMiscToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.installToMiscToolStripMenuItem.Text = "Install to Misc";
			// 
			// installToRootFolderToolStripMenuItem
			// 
			this.installToRootFolderToolStripMenuItem.Name = "installToRootFolderToolStripMenuItem";
			this.installToRootFolderToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.installToRootFolderToolStripMenuItem.Text = "Install to root folder";
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(185, 9);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(335, 33);
			this.comboBox1.TabIndex = 6;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(47, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 25);
			this.label2.TabIndex = 7;
			this.label2.Text = "1) Install To:";
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(3, 16);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(502, 99);
			this.listView1.TabIndex = 16;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Filename";
			this.columnHeader1.Width = 200;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "FilePath";
			this.columnHeader2.Width = 0;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Type";
			this.columnHeader3.Width = 130;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Message";
			this.columnHeader4.Width = 200;
			// 
			// picRemove
			// 
			this.picRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.picRemove.Image = ((System.Drawing.Image)(resources.GetObject("picRemove.Image")));
			this.picRemove.Location = new System.Drawing.Point(13, 359);
			this.picRemove.Name = "picRemove";
			this.picRemove.Size = new System.Drawing.Size(50, 50);
			this.picRemove.TabIndex = 17;
			this.picRemove.TabStop = false;
			this.picRemove.Visible = false;
			// 
			// picAccept
			// 
			this.picAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.picAccept.Image = ((System.Drawing.Image)(resources.GetObject("picAccept.Image")));
			this.picAccept.Location = new System.Drawing.Point(13, 360);
			this.picAccept.Name = "picAccept";
			this.picAccept.Size = new System.Drawing.Size(51, 50);
			this.picAccept.TabIndex = 18;
			this.picAccept.TabStop = false;
			this.picAccept.Visible = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Enabled = false;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(50, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(127, 25);
			this.label3.TabIndex = 22;
			this.label3.Text = "2) In Folder:";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(81, 403);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(297, 13);
			this.linkLabel1.TabIndex = 24;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Click here to download the Framework Installer to correct this.";
			this.linkLabel1.Visible = false;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 421);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(534, 22);
			this.statusStrip1.TabIndex = 25;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(417, 17);
			this.toolStripStatusLabel1.Spring = true;
			this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblHeader
			// 
			this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHeader.Location = new System.Drawing.Point(70, 366);
			this.lblHeader.Name = "lblHeader";
			this.lblHeader.Size = new System.Drawing.Size(319, 50);
			this.lblHeader.TabIndex = 26;
			this.lblHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(65, 87);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(112, 27);
			this.button1.TabIndex = 27;
			this.button1.Text = "Open this folder";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(14, 155);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(506, 70);
			this.label1.TabIndex = 29;
			this.label1.Text = "Select the game folder to install these packages to, using the dropdown 1) Instal" +
				"l To.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.linkLabel2);
			this.groupBox1.Controls.Add(this.listView1);
			this.groupBox1.Location = new System.Drawing.Point(12, 236);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(508, 118);
			this.groupBox1.TabIndex = 30;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Files to Install:";
			// 
			// linkLabel2
			// 
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Location = new System.Drawing.Point(205, 0);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(305, 13);
			this.linkLabel2.TabIndex = 25;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "To uninstall files, click here to download the Sims 3 Dashboard.";
			this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.treeView1.FullRowSelect = true;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(186, 48);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(333, 104);
			this.treeView1.TabIndex = 31;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
			// 
			// studioShieldButton1
			// 
			this.studioShieldButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.studioShieldButton1.Enabled = false;
			this.studioShieldButton1.EscalationCustomProcessPath = "";
			this.studioShieldButton1.EscalationGoal = StudioControls.Controls.EscalationGoal.RestartThisApplication;
			this.studioShieldButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.studioShieldButton1.Location = new System.Drawing.Point(395, 367);
			this.studioShieldButton1.Name = "studioShieldButton1";
			this.studioShieldButton1.Size = new System.Drawing.Size(124, 39);
			this.studioShieldButton1.TabIndex = 28;
			this.studioShieldButton1.Text = "3) Install";
			this.studioShieldButton1.UseVisualStyleBackColor = true;
			this.studioShieldButton1.EscalationSuccessful += new StudioControls.Controls.StudioShieldButton.EscalationHandler(this.studioShieldButton1_EscalationSuccessful);
			this.studioShieldButton1.EscalationStarting += new StudioControls.Controls.StudioShieldButton.EscalationHandler(this.studioShieldButton1_EscalationStarting);
			this.studioShieldButton1.Click += new System.EventHandler(this.studioShieldButton1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 443);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.studioShieldButton1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.picAccept);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblHeader);
			this.Controls.Add(this.picRemove);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Sims 3 Install Helper";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.contextMenuStrip1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picRemove)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picAccept)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem installToPatternsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToHacksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToSkinsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToMiscToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem installToRootFolderToolStripMenuItem;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.PictureBox picRemove;
		private System.Windows.Forms.PictureBox picAccept;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private StudioControls.Controls.StudioShieldButton studioShieldButton1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.LinkLabel linkLabel2;
    }
}

