namespace CASPartEditor
{
    partial class SaveForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.rNew = new System.Windows.Forms.RadioButton();
            this.rDefault = new System.Windows.Forms.RadioButton();
            this.rCustom = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rNew
            // 
            this.rNew.AutoSize = true;
            this.rNew.Location = new System.Drawing.Point(12, 12);
            this.rNew.Name = "rNew";
            this.rNew.Size = new System.Drawing.Size(93, 17);
            this.rNew.TabIndex = 0;
            this.rNew.TabStop = true;
            this.rNew.Text = "New CAS Part";
            this.rNew.UseVisualStyleBackColor = true;
            this.rNew.CheckedChanged += new System.EventHandler(this.rNew_CheckedChanged);
            // 
            // rDefault
            // 
            this.rDefault.AutoSize = true;
            this.rDefault.Location = new System.Drawing.Point(12, 60);
            this.rDefault.Name = "rDefault";
            this.rDefault.Size = new System.Drawing.Size(125, 17);
            this.rDefault.TabIndex = 1;
            this.rDefault.TabStop = true;
            this.rDefault.Text = "Default Replacement";
            this.rDefault.UseVisualStyleBackColor = true;
            this.rDefault.CheckedChanged += new System.EventHandler(this.rDefault_CheckedChanged);
            // 
            // rCustom
            // 
            this.rCustom.AutoSize = true;
            this.rCustom.Location = new System.Drawing.Point(12, 108);
            this.rCustom.Name = "rCustom";
            this.rCustom.Size = new System.Drawing.Size(162, 17);
            this.rCustom.TabIndex = 2;
            this.rCustom.TabStop = true;
            this.rCustom.Text = "Custom Instance (Advanced)";
            this.rCustom.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 32);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.label1.Size = new System.Drawing.Size(282, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Generates a new unique instance and creates a new item.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 80);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.label2.Size = new System.Drawing.Size(339, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Saves the CAS part with the same instance, replacing the current item.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(346, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Enter a hexadecimal instance number or text that will be FNV64 hashed.";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(108, 144);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 20);
            this.textBox1.TabIndex = 6;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(71, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Text:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(84, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "0x";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(108, 170);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(111, 20);
            this.textBox2.TabIndex = 8;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(310, 203);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "&OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(229, 203);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // SaveForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(395, 238);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rCustom);
            this.Controls.Add(this.rDefault);
            this.Controls.Add(this.rNew);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Save As...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rNew;
        private System.Windows.Forms.RadioButton rDefault;
        private System.Windows.Forms.RadioButton rCustom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}