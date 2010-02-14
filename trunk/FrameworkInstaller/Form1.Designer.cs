namespace FrameworkInstaller
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.button1 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.picAccept = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.picRemove = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkFrameworkHELS = new System.Windows.Forms.CheckBox();
			this.chkHasHELS = new System.Windows.Forms.CheckBox();
			this.button4 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.chkFrameworkWA = new System.Windows.Forms.CheckBox();
			this.chkFrameworkTS3 = new System.Windows.Forms.CheckBox();
			this.chkHasTS3 = new System.Windows.Forms.CheckBox();
			this.chkHasWA = new System.Windows.Forms.CheckBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disableFrameworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.picAccept)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picRemove)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(383, 40);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Change";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox2
			// 
			this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox2.Location = new System.Drawing.Point(32, 42);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(344, 20);
			this.textBox2.TabIndex = 5;
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(379, 409);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(103, 39);
			this.button2.TabIndex = 6;
			this.button2.Text = "Install";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// textBox3
			// 
			this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox3.Location = new System.Drawing.Point(32, 94);
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(344, 20);
			this.textBox3.TabIndex = 8;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(383, 93);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 7;
			this.button3.Text = "Change";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// picAccept
			// 
			this.picAccept.Image = ((System.Drawing.Image)(resources.GetObject("picAccept.Image")));
			this.picAccept.Location = new System.Drawing.Point(18, 406);
			this.picAccept.Name = "picAccept";
			this.picAccept.Size = new System.Drawing.Size(51, 50);
			this.picAccept.TabIndex = 11;
			this.picAccept.TabStop = false;
			this.picAccept.Visible = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(75, 399);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 63);
			this.label1.TabIndex = 12;
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// picRemove
			// 
			this.picRemove.Image = ((System.Drawing.Image)(resources.GetObject("picRemove.Image")));
			this.picRemove.Location = new System.Drawing.Point(19, 406);
			this.picRemove.Name = "picRemove";
			this.picRemove.Size = new System.Drawing.Size(50, 50);
			this.picRemove.TabIndex = 13;
			this.picRemove.TabStop = false;
			this.picRemove.Visible = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkFrameworkHELS);
			this.groupBox1.Controls.Add(this.chkHasHELS);
			this.groupBox1.Controls.Add(this.button4);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Controls.Add(this.chkFrameworkWA);
			this.groupBox1.Controls.Add(this.chkFrameworkTS3);
			this.groupBox1.Controls.Add(this.chkHasTS3);
			this.groupBox1.Controls.Add(this.chkHasWA);
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.button3);
			this.groupBox1.Controls.Add(this.textBox3);
			this.groupBox1.Location = new System.Drawing.Point(10, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(469, 188);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "The Sims 3 Games I have:";
			// 
			// chkFrameworkHELS
			// 
			this.chkFrameworkHELS.AutoSize = true;
			this.chkFrameworkHELS.Location = new System.Drawing.Point(276, 124);
			this.chkFrameworkHELS.Name = "chkFrameworkHELS";
			this.chkFrameworkHELS.Size = new System.Drawing.Size(100, 17);
			this.chkFrameworkHELS.TabIndex = 17;
			this.chkFrameworkHELS.Text = "Has Framework";
			this.chkFrameworkHELS.UseVisualStyleBackColor = true;
			// 
			// chkHasHELS
			// 
			this.chkHasHELS.AutoSize = true;
			this.chkHasHELS.Location = new System.Drawing.Point(9, 124);
			this.chkHasHELS.Name = "chkHasHELS";
			this.chkHasHELS.Size = new System.Drawing.Size(116, 17);
			this.chkHasHELS.TabIndex = 16;
			this.chkHasHELS.Text = "High-End Loft Stuff";
			this.chkHasHELS.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(383, 146);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 14;
			this.button4.Text = "Change";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox1.Location = new System.Drawing.Point(32, 147);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(344, 20);
			this.textBox1.TabIndex = 15;
			// 
			// chkFrameworkWA
			// 
			this.chkFrameworkWA.AutoSize = true;
			this.chkFrameworkWA.Location = new System.Drawing.Point(276, 71);
			this.chkFrameworkWA.Name = "chkFrameworkWA";
			this.chkFrameworkWA.Size = new System.Drawing.Size(100, 17);
			this.chkFrameworkWA.TabIndex = 13;
			this.chkFrameworkWA.Text = "Has Framework";
			this.chkFrameworkWA.UseVisualStyleBackColor = true;
			// 
			// chkFrameworkTS3
			// 
			this.chkFrameworkTS3.AutoSize = true;
			this.chkFrameworkTS3.Location = new System.Drawing.Point(276, 19);
			this.chkFrameworkTS3.Name = "chkFrameworkTS3";
			this.chkFrameworkTS3.Size = new System.Drawing.Size(100, 17);
			this.chkFrameworkTS3.TabIndex = 12;
			this.chkFrameworkTS3.Text = "Has Framework";
			this.chkFrameworkTS3.UseVisualStyleBackColor = true;
			// 
			// chkHasTS3
			// 
			this.chkHasTS3.AutoSize = true;
			this.chkHasTS3.Location = new System.Drawing.Point(9, 19);
			this.chkHasTS3.Name = "chkHasTS3";
			this.chkHasTS3.Size = new System.Drawing.Size(79, 17);
			this.chkHasTS3.TabIndex = 11;
			this.chkHasTS3.Text = "The Sims 3";
			this.chkHasTS3.UseVisualStyleBackColor = true;
			// 
			// chkHasWA
			// 
			this.chkHasWA.AutoSize = true;
			this.chkHasWA.Location = new System.Drawing.Point(9, 71);
			this.chkHasWA.Name = "chkHasWA";
			this.chkHasWA.Size = new System.Drawing.Size(111, 17);
			this.chkHasWA.TabIndex = 10;
			this.chkHasWA.Text = "World Adventures";
			this.chkHasWA.UseVisualStyleBackColor = true;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(10, 226);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(469, 170);
			this.listView1.TabIndex = 15;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Diagnostics";
			this.columnHeader1.Width = 440;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.Size = new System.Drawing.Size(492, 24);
			this.menuStrip1.TabIndex = 16;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disableFrameworkToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// disableFrameworkToolStripMenuItem
			// 
			this.disableFrameworkToolStripMenuItem.Name = "disableFrameworkToolStripMenuItem";
			this.disableFrameworkToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.disableFrameworkToolStripMenuItem.Text = "Disable Framework";
			this.disableFrameworkToolStripMenuItem.Click += new System.EventHandler(this.disableFrameworkToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(492, 465);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.picRemove);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.picAccept);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "Sims 3 Framework Installer";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.picAccept)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picRemove)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.PictureBox picAccept;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox picRemove;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkHasTS3;
		private System.Windows.Forms.CheckBox chkHasWA;
		private System.Windows.Forms.CheckBox chkFrameworkWA;
		private System.Windows.Forms.CheckBox chkFrameworkTS3;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disableFrameworkToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.CheckBox chkFrameworkHELS;
		private System.Windows.Forms.CheckBox chkHasHELS;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.TextBox textBox1;
    }
}

