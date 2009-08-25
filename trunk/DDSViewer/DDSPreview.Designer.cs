namespace DDSViewer
{
    partial class DDSPreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DDSPreview));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chkShowAlpha = new System.Windows.Forms.CheckBox();
            this.chkShowBlue = new System.Windows.Forms.CheckBox();
            this.chkShowGreen = new System.Windows.Forms.CheckBox();
            this.chkShowRed = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.txtSourceDDS = new System.Windows.Forms.TextBox();
            this.btnBrowseDDS = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(514, 514);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // chkShowAlpha
            // 
            this.chkShowAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowAlpha.AutoSize = true;
            this.chkShowAlpha.Location = new System.Drawing.Point(166, 630);
            this.chkShowAlpha.Name = "chkShowAlpha";
            this.chkShowAlpha.Size = new System.Drawing.Size(53, 17);
            this.chkShowAlpha.TabIndex = 32;
            this.chkShowAlpha.Text = "Alpha";
            this.chkShowAlpha.UseVisualStyleBackColor = true;
            this.chkShowAlpha.CheckedChanged += new System.EventHandler(this.chkShowAlpha_CheckedChanged);
            // 
            // chkShowBlue
            // 
            this.chkShowBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowBlue.AutoSize = true;
            this.chkShowBlue.Location = new System.Drawing.Point(113, 630);
            this.chkShowBlue.Name = "chkShowBlue";
            this.chkShowBlue.Size = new System.Drawing.Size(47, 17);
            this.chkShowBlue.TabIndex = 31;
            this.chkShowBlue.Text = "Blue";
            this.chkShowBlue.UseVisualStyleBackColor = true;
            this.chkShowBlue.CheckedChanged += new System.EventHandler(this.chkShowBlue_CheckedChanged);
            // 
            // chkShowGreen
            // 
            this.chkShowGreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowGreen.AutoSize = true;
            this.chkShowGreen.Location = new System.Drawing.Point(57, 630);
            this.chkShowGreen.Name = "chkShowGreen";
            this.chkShowGreen.Size = new System.Drawing.Size(55, 17);
            this.chkShowGreen.TabIndex = 30;
            this.chkShowGreen.Text = "Green";
            this.chkShowGreen.UseVisualStyleBackColor = true;
            this.chkShowGreen.CheckedChanged += new System.EventHandler(this.chkShowGreen_CheckedChanged);
            // 
            // chkShowRed
            // 
            this.chkShowRed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowRed.AutoSize = true;
            this.chkShowRed.Location = new System.Drawing.Point(5, 630);
            this.chkShowRed.Name = "chkShowRed";
            this.chkShowRed.Size = new System.Drawing.Size(46, 17);
            this.chkShowRed.TabIndex = 29;
            this.chkShowRed.Text = "Red";
            this.chkShowRed.UseVisualStyleBackColor = true;
            this.chkShowRed.CheckedChanged += new System.EventHandler(this.chkShowRed_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel5});
            this.statusStrip1.Location = new System.Drawing.Point(0, 651);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 33;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(686, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "FileName";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(21, 17);
            this.toolStripStatusLabel2.Text = "W";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(18, 17);
            this.toolStripStatusLabel3.Text = "H";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(23, 17);
            this.toolStripStatusLabel4.Text = "bit";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(29, 17);
            this.toolStripStatusLabel5.Text = "pxF";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 622);
            this.panel1.TabIndex = 34;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(705, 625);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 35;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSourceDDS
            // 
            this.txtSourceDDS.Location = new System.Drawing.Point(246, 628);
            this.txtSourceDDS.Name = "txtSourceDDS";
            this.txtSourceDDS.Size = new System.Drawing.Size(318, 20);
            this.txtSourceDDS.TabIndex = 79;
            // 
            // btnBrowseDDS
            // 
            this.btnBrowseDDS.Location = new System.Drawing.Point(570, 626);
            this.btnBrowseDDS.Name = "btnBrowseDDS";
            this.btnBrowseDDS.Size = new System.Drawing.Size(52, 23);
            this.btnBrowseDDS.TabIndex = 78;
            this.btnBrowseDDS.Text = "browse";
            this.btnBrowseDDS.UseVisualStyleBackColor = true;
            this.btnBrowseDDS.Click += new System.EventHandler(this.btnBrowseDDS_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DDSPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 673);
            this.Controls.Add(this.txtSourceDDS);
            this.Controls.Add(this.btnBrowseDDS);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.chkShowAlpha);
            this.Controls.Add(this.chkShowBlue);
            this.Controls.Add(this.chkShowGreen);
            this.Controls.Add(this.chkShowRed);
            this.Name = "DDSPreview";
            this.Text = "DDSPreview";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DDSPreview_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DDSPreview_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkShowAlpha;
        private System.Windows.Forms.CheckBox chkShowBlue;
        private System.Windows.Forms.CheckBox chkShowGreen;
        private System.Windows.Forms.CheckBox chkShowRed;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.TextBox txtSourceDDS;
        private System.Windows.Forms.Button btnBrowseDDS;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}