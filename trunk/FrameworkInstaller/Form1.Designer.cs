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
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.textBox8 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button5 = new System.Windows.Forms.Button();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.chkFrameworkAmb = new System.Windows.Forms.CheckBox();
			this.chkHasAmb = new System.Windows.Forms.CheckBox();
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
			this.rescanFrameworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.disableFrameworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picAccept)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picRemove)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(598, 31);
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
			this.textBox2.Location = new System.Drawing.Point(230, 57);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(362, 20);
			this.textBox2.TabIndex = 5;
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(577, 363);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(109, 39);
			this.button2.TabIndex = 6;
			this.button2.Text = "Install";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// textBox3
			// 
			this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox3.Location = new System.Drawing.Point(230, 80);
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(362, 20);
			this.textBox3.TabIndex = 8;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(598, 54);
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
			this.picAccept.Location = new System.Drawing.Point(19, 353);
			this.picAccept.Name = "picAccept";
			this.picAccept.Size = new System.Drawing.Size(51, 50);
			this.picAccept.TabIndex = 11;
			this.picAccept.TabStop = false;
			this.picAccept.Visible = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(73, 353);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(501, 63);
			this.label1.TabIndex = 12;
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// picRemove
			// 
			this.picRemove.Image = ((System.Drawing.Image)(resources.GetObject("picRemove.Image")));
			this.picRemove.Location = new System.Drawing.Point(19, 353);
			this.picRemove.Name = "picRemove";
			this.picRemove.Size = new System.Drawing.Size(50, 50);
			this.picRemove.TabIndex = 13;
			this.picRemove.TabStop = false;
			this.picRemove.Visible = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textBox5);
			this.groupBox1.Controls.Add(this.textBox6);
			this.groupBox1.Controls.Add(this.textBox7);
			this.groupBox1.Controls.Add(this.textBox8);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.button5);
			this.groupBox1.Controls.Add(this.textBox4);
			this.groupBox1.Controls.Add(this.chkFrameworkAmb);
			this.groupBox1.Controls.Add(this.chkHasAmb);
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
			this.groupBox1.Size = new System.Drawing.Size(679, 135);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "The Sims 3 Games I have:";
			// 
			// textBox5
			// 
			this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox5.Location = new System.Drawing.Point(189, 34);
			this.textBox5.Name = "textBox5";
			this.textBox5.ReadOnly = true;
			this.textBox5.Size = new System.Drawing.Size(35, 20);
			this.textBox5.TabIndex = 28;
			// 
			// textBox6
			// 
			this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox6.Location = new System.Drawing.Point(189, 57);
			this.textBox6.Name = "textBox6";
			this.textBox6.ReadOnly = true;
			this.textBox6.Size = new System.Drawing.Size(35, 20);
			this.textBox6.TabIndex = 27;
			// 
			// textBox7
			// 
			this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox7.Location = new System.Drawing.Point(189, 81);
			this.textBox7.Name = "textBox7";
			this.textBox7.ReadOnly = true;
			this.textBox7.Size = new System.Drawing.Size(35, 20);
			this.textBox7.TabIndex = 25;
			// 
			// textBox8
			// 
			this.textBox8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox8.Location = new System.Drawing.Point(189, 103);
			this.textBox8.Name = "textBox8";
			this.textBox8.ReadOnly = true;
			this.textBox8.Size = new System.Drawing.Size(35, 20);
			this.textBox8.TabIndex = 26;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(186, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 24;
			this.label4.Text = "Patch:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(118, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 13);
			this.label3.TabIndex = 23;
			this.label3.Text = "Framework:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 22;
			this.label2.Text = "Game:";
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(598, 102);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 21;
			this.button5.Text = "Change";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// textBox4
			// 
			this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox4.Location = new System.Drawing.Point(230, 103);
			this.textBox4.Name = "textBox4";
			this.textBox4.ReadOnly = true;
			this.textBox4.Size = new System.Drawing.Size(362, 20);
			this.textBox4.TabIndex = 20;
			// 
			// chkFrameworkAmb
			// 
			this.chkFrameworkAmb.AutoSize = true;
			this.chkFrameworkAmb.Location = new System.Drawing.Point(159, 106);
			this.chkFrameworkAmb.Name = "chkFrameworkAmb";
			this.chkFrameworkAmb.Size = new System.Drawing.Size(15, 14);
			this.chkFrameworkAmb.TabIndex = 19;
			this.chkFrameworkAmb.UseVisualStyleBackColor = true;
			// 
			// chkHasAmb
			// 
			this.chkHasAmb.AutoSize = true;
			this.chkHasAmb.Location = new System.Drawing.Point(9, 104);
			this.chkHasAmb.Name = "chkHasAmb";
			this.chkHasAmb.Size = new System.Drawing.Size(71, 17);
			this.chkHasAmb.TabIndex = 18;
			this.chkHasAmb.Text = "Ambitions";
			this.chkHasAmb.UseVisualStyleBackColor = true;
			// 
			// chkFrameworkHELS
			// 
			this.chkFrameworkHELS.AutoSize = true;
			this.chkFrameworkHELS.Location = new System.Drawing.Point(159, 83);
			this.chkFrameworkHELS.Name = "chkFrameworkHELS";
			this.chkFrameworkHELS.Size = new System.Drawing.Size(15, 14);
			this.chkFrameworkHELS.TabIndex = 17;
			this.chkFrameworkHELS.UseVisualStyleBackColor = true;
			// 
			// chkHasHELS
			// 
			this.chkHasHELS.AutoSize = true;
			this.chkHasHELS.Location = new System.Drawing.Point(9, 81);
			this.chkHasHELS.Name = "chkHasHELS";
			this.chkHasHELS.Size = new System.Drawing.Size(116, 17);
			this.chkHasHELS.TabIndex = 16;
			this.chkHasHELS.Text = "High-End Loft Stuff";
			this.chkHasHELS.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(598, 78);
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
			this.textBox1.Location = new System.Drawing.Point(230, 34);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(362, 20);
			this.textBox1.TabIndex = 15;
			// 
			// chkFrameworkWA
			// 
			this.chkFrameworkWA.AutoSize = true;
			this.chkFrameworkWA.Location = new System.Drawing.Point(159, 60);
			this.chkFrameworkWA.Name = "chkFrameworkWA";
			this.chkFrameworkWA.Size = new System.Drawing.Size(15, 14);
			this.chkFrameworkWA.TabIndex = 13;
			this.chkFrameworkWA.UseVisualStyleBackColor = true;
			// 
			// chkFrameworkTS3
			// 
			this.chkFrameworkTS3.AutoSize = true;
			this.chkFrameworkTS3.Location = new System.Drawing.Point(159, 38);
			this.chkFrameworkTS3.Name = "chkFrameworkTS3";
			this.chkFrameworkTS3.Size = new System.Drawing.Size(15, 14);
			this.chkFrameworkTS3.TabIndex = 12;
			this.chkFrameworkTS3.UseVisualStyleBackColor = true;
			// 
			// chkHasTS3
			// 
			this.chkHasTS3.AutoSize = true;
			this.chkHasTS3.Location = new System.Drawing.Point(9, 35);
			this.chkHasTS3.Name = "chkHasTS3";
			this.chkHasTS3.Size = new System.Drawing.Size(79, 17);
			this.chkHasTS3.TabIndex = 11;
			this.chkHasTS3.Text = "The Sims 3";
			this.chkHasTS3.UseVisualStyleBackColor = true;
			// 
			// chkHasWA
			// 
			this.chkHasWA.AutoSize = true;
			this.chkHasWA.Location = new System.Drawing.Point(9, 58);
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
			this.listView1.Location = new System.Drawing.Point(10, 174);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(673, 170);
			this.listView1.TabIndex = 15;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Diagnostics";
			this.columnHeader1.Width = 650;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.Size = new System.Drawing.Size(698, 24);
			this.menuStrip1.TabIndex = 16;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanFrameworkToolStripMenuItem,
            this.toolStripMenuItem2,
            this.disableFrameworkToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// rescanFrameworkToolStripMenuItem
			// 
			this.rescanFrameworkToolStripMenuItem.Name = "rescanFrameworkToolStripMenuItem";
			this.rescanFrameworkToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.rescanFrameworkToolStripMenuItem.Text = "Re-scan Framework";
			this.rescanFrameworkToolStripMenuItem.Click += new System.EventHandler(this.rescanFrameworkToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(175, 6);
			// 
			// disableFrameworkToolStripMenuItem
			// 
			this.disableFrameworkToolStripMenuItem.Name = "disableFrameworkToolStripMenuItem";
			this.disableFrameworkToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.disableFrameworkToolStripMenuItem.Text = "Disable Framework";
			this.disableFrameworkToolStripMenuItem.Click += new System.EventHandler(this.disableFrameworkToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(175, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.progressBar1.Location = new System.Drawing.Point(-2, 426);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(700, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 17;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(118, 105);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 29;
			this.label5.Text = "Global";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(698, 447);
			this.Controls.Add(this.progressBar1);
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
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.CheckBox chkFrameworkAmb;
		private System.Windows.Forms.CheckBox chkHasAmb;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.ToolStripMenuItem rescanFrameworkToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.Label label5;
    }
}

