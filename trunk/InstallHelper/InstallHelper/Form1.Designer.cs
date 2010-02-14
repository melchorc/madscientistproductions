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
			this.picRemove = new System.Windows.Forms.PictureBox();
			this.picAccept = new System.Windows.Forms.PictureBox();
			this.button2 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblHeader = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picRemove)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picAccept)).BeginInit();
			this.statusStrip1.SuspendLayout();
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
			this.contextMenuStrip1.Size = new System.Drawing.Size(171, 114);
			// 
			// installToPatternsToolStripMenuItem
			// 
			this.installToPatternsToolStripMenuItem.Name = "installToPatternsToolStripMenuItem";
			this.installToPatternsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.installToPatternsToolStripMenuItem.Text = "Install to Patterns";
			// 
			// installToHacksToolStripMenuItem
			// 
			this.installToHacksToolStripMenuItem.Name = "installToHacksToolStripMenuItem";
			this.installToHacksToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.installToHacksToolStripMenuItem.Text = "Install to Hacks";
			// 
			// installToSkinsToolStripMenuItem
			// 
			this.installToSkinsToolStripMenuItem.Name = "installToSkinsToolStripMenuItem";
			this.installToSkinsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.installToSkinsToolStripMenuItem.Text = "Install to Skins";
			// 
			// installToMiscToolStripMenuItem
			// 
			this.installToMiscToolStripMenuItem.Name = "installToMiscToolStripMenuItem";
			this.installToMiscToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.installToMiscToolStripMenuItem.Text = "Install to Misc";
			// 
			// installToRootFolderToolStripMenuItem
			// 
			this.installToRootFolderToolStripMenuItem.Name = "installToRootFolderToolStripMenuItem";
			this.installToRootFolderToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.installToRootFolderToolStripMenuItem.Text = "Install to root folder";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(147, 9);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(335, 39);
			this.comboBox1.TabIndex = 6;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(24, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 29);
			this.label2.TabIndex = 7;
			this.label2.Text = "Install To:";
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(13, 187);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(469, 138);
			this.listView1.TabIndex = 16;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Filename";
			this.columnHeader1.Width = 320;
			// 
			// picRemove
			// 
			this.picRemove.Image = ((System.Drawing.Image)(resources.GetObject("picRemove.Image")));
			this.picRemove.Location = new System.Drawing.Point(15, 337);
			this.picRemove.Name = "picRemove";
			this.picRemove.Size = new System.Drawing.Size(50, 50);
			this.picRemove.TabIndex = 17;
			this.picRemove.TabStop = false;
			this.picRemove.Visible = false;
			// 
			// picAccept
			// 
			this.picAccept.Image = ((System.Drawing.Image)(resources.GetObject("picAccept.Image")));
			this.picAccept.Location = new System.Drawing.Point(14, 337);
			this.picAccept.Name = "picAccept";
			this.picAccept.Size = new System.Drawing.Size(51, 50);
			this.picAccept.TabIndex = 18;
			this.picAccept.TabStop = false;
			this.picAccept.Visible = false;
			// 
			// button2
			// 
			this.button2.Enabled = false;
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(373, 343);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(103, 39);
			this.button2.TabIndex = 20;
			this.button2.Text = "Install";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(24, 54);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(115, 29);
			this.label3.TabIndex = 22;
			this.label3.Text = "In Folder:";
			// 
			// listBox1
			// 
			this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 20;
			this.listBox1.Location = new System.Drawing.Point(147, 54);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(334, 124);
			this.listBox1.TabIndex = 23;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(73, 392);
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
			this.statusStrip1.Location = new System.Drawing.Point(0, 405);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(494, 22);
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
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(346, 17);
			this.toolStripStatusLabel1.Spring = true;
			// 
			// lblHeader
			// 
			this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHeader.Location = new System.Drawing.Point(71, 330);
			this.lblHeader.Name = "lblHeader";
			this.lblHeader.Size = new System.Drawing.Size(296, 62);
			this.lblHeader.TabIndex = 26;
			this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(29, 97);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(112, 27);
			this.button1.TabIndex = 27;
			this.button1.Text = "Open this folder";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 0;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Type";
			this.columnHeader3.Width = 120;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(494, 427);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.lblHeader);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.picAccept);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.picRemove);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}

