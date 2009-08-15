namespace MadScience.Wrappers
{
    partial class RCOLHeaderEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtInternalChunks = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtExternalChunks = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtDatatype = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtInternalChunks);
            this.groupBox1.Location = new System.Drawing.Point(3, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 188);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Internal Chunks:";
            // 
            // txtInternalChunks
            // 
            this.txtInternalChunks.Location = new System.Drawing.Point(6, 19);
            this.txtInternalChunks.Multiline = true;
            this.txtInternalChunks.Name = "txtInternalChunks";
            this.txtInternalChunks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInternalChunks.Size = new System.Drawing.Size(274, 163);
            this.txtInternalChunks.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtExternalChunks);
            this.groupBox2.Location = new System.Drawing.Point(295, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 188);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "External Resources:";
            // 
            // txtExternalChunks
            // 
            this.txtExternalChunks.Location = new System.Drawing.Point(6, 19);
            this.txtExternalChunks.Multiline = true;
            this.txtExternalChunks.Name = "txtExternalChunks";
            this.txtExternalChunks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExternalChunks.Size = new System.Drawing.Size(274, 163);
            this.txtExternalChunks.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Version:";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(60, 6);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(100, 20);
            this.txtVersion.TabIndex = 3;
            // 
            // txtDatatype
            // 
            this.txtDatatype.Location = new System.Drawing.Point(228, 6);
            this.txtDatatype.Name = "txtDatatype";
            this.txtDatatype.Size = new System.Drawing.Size(100, 20);
            this.txtDatatype.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(166, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Data type:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(478, 225);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(97, 32);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 223);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(457, 31);
            this.label3.TabIndex = 7;
            this.label3.Text = "Keys are in the format: key:TTTTTTTT:GGGGGGGG:IIIIIIIIIIIIIIII, one per line.  To" +
                " remove a key, delete the line.";
            // 
            // RCOLHeaderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 263);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDatatype);
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RCOLHeaderEditor";
            this.Text = "Edit RCOL Header";
            this.Load += new System.EventHandler(this.RCOLHeaderEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtInternalChunks;
        private System.Windows.Forms.TextBox txtExternalChunks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox txtDatatype;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label3;
    }
}