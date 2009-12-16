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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.installToPatternsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToHacksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToSkinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToMiscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.installToRootFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.contextMenuStrip1.SuspendLayout();
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
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(406, 11);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Change";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(13, 151);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(468, 100);
			this.textBox1.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Diagnostics:";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(55, 12);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(344, 20);
			this.textBox2.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(13, 83);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(371, 52);
			this.label1.TabIndex = 0;
			this.label1.Text = "Check the paths above, and click Go to install the Framework";
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(378, 83);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(103, 39);
			this.button2.TabIndex = 6;
			this.button2.Text = "Go";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(55, 38);
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(344, 20);
			this.textBox3.TabIndex = 8;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(406, 37);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 7;
			this.button3.Text = "Change";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "TS3:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 41);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(28, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "WA:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(491, 263);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "Sims 3 Framework Installer";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
    }
}

