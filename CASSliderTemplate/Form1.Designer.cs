namespace CASSliderTemplate
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSliderName = new System.Windows.Forms.TextBox();
            this.txtSliderString = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSubgroup = new System.Windows.Forms.TextBox();
            this.chkListCasPanelGroup = new System.Windows.Forms.CheckedListBox();
            this.cmbRegionType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.chkMale = new System.Windows.Forms.CheckBox();
            this.chkFemale = new System.Windows.Forms.CheckBox();
            this.chkMFLink = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Slider Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "String to display:";
            // 
            // txtSliderName
            // 
            this.txtSliderName.Location = new System.Drawing.Point(95, 6);
            this.txtSliderName.Name = "txtSliderName";
            this.txtSliderName.Size = new System.Drawing.Size(171, 20);
            this.txtSliderName.TabIndex = 2;
            // 
            // txtSliderString
            // 
            this.txtSliderString.Location = new System.Drawing.Point(95, 32);
            this.txtSliderString.Name = "txtSliderString";
            this.txtSliderString.Size = new System.Drawing.Size(171, 20);
            this.txtSliderString.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(191, 281);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(312, 309);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 29);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "CAS Panel:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Sub group:";
            // 
            // txtSubgroup
            // 
            this.txtSubgroup.Location = new System.Drawing.Point(95, 228);
            this.txtSubgroup.Name = "txtSubgroup";
            this.txtSubgroup.Size = new System.Drawing.Size(127, 20);
            this.txtSubgroup.TabIndex = 9;
            this.txtSubgroup.Text = "2";
            // 
            // chkListCasPanelGroup
            // 
            this.chkListCasPanelGroup.FormattingEnabled = true;
            this.chkListCasPanelGroup.Items.AddRange(new object[] {
            "Head and Ears",
            "Mouth",
            "Nose",
            "Eyelashes",
            "Eyes"});
            this.chkListCasPanelGroup.Location = new System.Drawing.Point(95, 128);
            this.chkListCasPanelGroup.Name = "chkListCasPanelGroup";
            this.chkListCasPanelGroup.Size = new System.Drawing.Size(127, 94);
            this.chkListCasPanelGroup.TabIndex = 10;
            this.chkListCasPanelGroup.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkListCasPanelGroup_ItemCheck);
            // 
            // cmbRegionType
            // 
            this.cmbRegionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRegionType.FormattingEnabled = true;
            this.cmbRegionType.Items.AddRange(new object[] {
            "Body",
            "Brow",
            "Ears",
            "Eyelashes",
            "Eyes",
            "Face",
            "Head",
            "Jaw",
            "Mouth",
            "Nose",
            "TranslateEyes",
            "TranslateMouth"});
            this.cmbRegionType.Location = new System.Drawing.Point(95, 254);
            this.cmbRegionType.Name = "cmbRegionType";
            this.cmbRegionType.Size = new System.Drawing.Size(127, 21);
            this.cmbRegionType.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 257);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Region:";
            // 
            // chkMale
            // 
            this.chkMale.AutoSize = true;
            this.chkMale.Checked = true;
            this.chkMale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMale.Location = new System.Drawing.Point(95, 58);
            this.chkMale.Name = "chkMale";
            this.chkMale.Size = new System.Drawing.Size(96, 17);
            this.chkMale.TabIndex = 13;
            this.chkMale.Text = "Generate Male";
            this.chkMale.UseVisualStyleBackColor = true;
            // 
            // chkFemale
            // 
            this.chkFemale.AutoSize = true;
            this.chkFemale.Checked = true;
            this.chkFemale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFemale.Location = new System.Drawing.Point(95, 80);
            this.chkFemale.Name = "chkFemale";
            this.chkFemale.Size = new System.Drawing.Size(107, 17);
            this.chkFemale.TabIndex = 14;
            this.chkFemale.Text = "Generate Female";
            this.chkFemale.UseVisualStyleBackColor = true;
            // 
            // chkMFLink
            // 
            this.chkMFLink.AutoSize = true;
            this.chkMFLink.Location = new System.Drawing.Point(95, 103);
            this.chkMFLink.Name = "chkMFLink";
            this.chkMFLink.Size = new System.Drawing.Size(158, 17);
            this.chkMFLink.TabIndex = 15;
            this.chkMFLink.Text = "Link Male && Female to same";
            this.chkMFLink.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 335);
            this.Controls.Add(this.chkMFLink);
            this.Controls.Add(this.chkFemale);
            this.Controls.Add(this.chkMale);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbRegionType);
            this.Controls.Add(this.chkListCasPanelGroup);
            this.Controls.Add(this.txtSubgroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtSliderString);
            this.Controls.Add(this.txtSliderName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "CAS Slider Template setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSliderName;
        private System.Windows.Forms.TextBox txtSliderString;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSubgroup;
        private System.Windows.Forms.CheckedListBox chkListCasPanelGroup;
        private System.Windows.Forms.ComboBox cmbRegionType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox chkMale;
        private System.Windows.Forms.CheckBox chkFemale;
        private System.Windows.Forms.CheckBox chkMFLink;
    }
}

