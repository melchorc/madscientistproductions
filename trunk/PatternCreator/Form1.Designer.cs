namespace PatternCreator
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPatternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPatternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseDDS = new System.Windows.Forms.Button();
            this.cmbSurfaceMat = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCreatorHomepage = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCreatorName = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtPatternTitle = new System.Windows.Forms.TextBox();
            this.txtPatternDesc = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.lblSpecularCustom = new System.Windows.Forms.Label();
            this.chkPalette4Blend = new System.Windows.Forms.CheckBox();
            this.chkPalette3Blend = new System.Windows.Forms.CheckBox();
            this.chkPalette2Blend = new System.Windows.Forms.CheckBox();
            this.chkPalette1Blend = new System.Windows.Forms.CheckBox();
            this.chkAllowDecal = new System.Windows.Forms.CheckBox();
            this.chkUseDefaultSpecular = new System.Windows.Forms.CheckBox();
            this.lblBackgroundColour = new System.Windows.Forms.Label();
            this.lblPalette4 = new System.Windows.Forms.Label();
            this.lblPalette3 = new System.Windows.Forms.Label();
            this.lblPalette2 = new System.Windows.Forms.Label();
            this.lblPalette1 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSourceDDS = new System.Windows.Forms.TextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chkShowRed = new System.Windows.Forms.CheckBox();
            this.chkShowGreen = new System.Windows.Forms.CheckBox();
            this.chkShowBlue = new System.Windows.Forms.CheckBox();
            this.chkShowAlpha = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Menu;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(773, 21);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPatternToolStripMenuItem,
            this.loadPatternToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 17);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // newPatternToolStripMenuItem
            // 
            this.newPatternToolStripMenuItem.Name = "newPatternToolStripMenuItem";
            this.newPatternToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.newPatternToolStripMenuItem.Text = "New Pattern";
            this.newPatternToolStripMenuItem.Click += new System.EventHandler(this.newPatternToolStripMenuItem_Click);
            // 
            // loadPatternToolStripMenuItem
            // 
            this.loadPatternToolStripMenuItem.Enabled = false;
            this.loadPatternToolStripMenuItem.Name = "loadPatternToolStripMenuItem";
            this.loadPatternToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.loadPatternToolStripMenuItem.Text = "Load Pattern";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(133, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(133, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnBrowseDDS
            // 
            this.btnBrowseDDS.Enabled = false;
            this.btnBrowseDDS.Location = new System.Drawing.Point(411, 17);
            this.btnBrowseDDS.Name = "btnBrowseDDS";
            this.btnBrowseDDS.Size = new System.Drawing.Size(52, 23);
            this.btnBrowseDDS.TabIndex = 6;
            this.btnBrowseDDS.Text = "browse";
            this.btnBrowseDDS.UseVisualStyleBackColor = true;
            this.btnBrowseDDS.Click += new System.EventHandler(this.button3_Click);
            // 
            // cmbSurfaceMat
            // 
            this.cmbSurfaceMat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSurfaceMat.Enabled = false;
            this.cmbSurfaceMat.FormattingEnabled = true;
            this.cmbSurfaceMat.Items.AddRange(new object[] {
            "cloth",
            "cment",
            "concrete",
            "cpet",
            "glass",
            "gravel",
            "livleath",
            "marble",
            "metal",
            "plas",
            "wood"});
            this.cmbSurfaceMat.Location = new System.Drawing.Point(341, 71);
            this.cmbSurfaceMat.Name = "cmbSurfaceMat";
            this.cmbSurfaceMat.Size = new System.Drawing.Size(122, 21);
            this.cmbSurfaceMat.TabIndex = 5;
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Enabled = false;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Items.AddRange(new object[] {
            "Fabric",
            "Leather_Fur",
            "Carpet_Rug",
            "Abstract",
            "Geometric",
            "Theme",
            "Wood",
            "Metal",
            "Weave_Wicker",
            "Paint",
            "Tile_Mosaic",
            "Plastic_Rubber",
            "Rock_Stone",
            "Masonry",
            "Miscellaneous"});
            this.cmbCategory.Location = new System.Drawing.Point(128, 71);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(116, 21);
            this.cmbCategory.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtCreatorHomepage);
            this.groupBox4.Controls.Add(this.dateTimePicker1);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.txtCreatorName);
            this.groupBox4.Location = new System.Drawing.Point(13, 28);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(476, 68);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Creator Details:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Homepage:";
            // 
            // txtCreatorHomepage
            // 
            this.txtCreatorHomepage.Enabled = false;
            this.txtCreatorHomepage.Location = new System.Drawing.Point(87, 42);
            this.txtCreatorHomepage.Name = "txtCreatorHomepage";
            this.txtCreatorHomepage.Size = new System.Drawing.Size(376, 20);
            this.txtCreatorHomepage.TabIndex = 4;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Enabled = false;
            this.dateTimePicker1.Location = new System.Drawing.Point(337, 16);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(126, 20);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(258, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Date Created:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Creator Name:";
            // 
            // txtCreatorName
            // 
            this.txtCreatorName.Enabled = false;
            this.txtCreatorName.Location = new System.Drawing.Point(87, 16);
            this.txtCreatorName.Name = "txtCreatorName";
            this.txtCreatorName.Size = new System.Drawing.Size(154, 20);
            this.txtCreatorName.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.txtPatternTitle);
            this.groupBox5.Controls.Add(this.txtPatternDesc);
            this.groupBox5.Controls.Add(this.cmbCategory);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.cmbSurfaceMat);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Location = new System.Drawing.Point(13, 102);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(476, 102);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Pattern Details:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(258, 74);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "Surface Type:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(27, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "In-Game Category:";
            // 
            // txtPatternTitle
            // 
            this.txtPatternTitle.Enabled = false;
            this.txtPatternTitle.Location = new System.Drawing.Point(128, 19);
            this.txtPatternTitle.Name = "txtPatternTitle";
            this.txtPatternTitle.Size = new System.Drawing.Size(203, 20);
            this.txtPatternTitle.TabIndex = 2;
            // 
            // txtPatternDesc
            // 
            this.txtPatternDesc.Enabled = false;
            this.txtPatternDesc.Location = new System.Drawing.Point(128, 45);
            this.txtPatternDesc.Name = "txtPatternDesc";
            this.txtPatternDesc.Size = new System.Drawing.Size(335, 20);
            this.txtPatternDesc.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(22, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Pattern Description:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(55, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Pattern Title:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Controls.Add(this.lblSpecularCustom);
            this.groupBox6.Controls.Add(this.chkPalette4Blend);
            this.groupBox6.Controls.Add(this.chkPalette3Blend);
            this.groupBox6.Controls.Add(this.chkPalette2Blend);
            this.groupBox6.Controls.Add(this.chkPalette1Blend);
            this.groupBox6.Controls.Add(this.chkAllowDecal);
            this.groupBox6.Controls.Add(this.chkUseDefaultSpecular);
            this.groupBox6.Controls.Add(this.lblBackgroundColour);
            this.groupBox6.Controls.Add(this.lblPalette4);
            this.groupBox6.Controls.Add(this.lblPalette3);
            this.groupBox6.Controls.Add(this.lblPalette2);
            this.groupBox6.Controls.Add(this.lblPalette1);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.label18);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.radioButton4);
            this.groupBox6.Controls.Add(this.radioButton3);
            this.groupBox6.Controls.Add(this.radioButton2);
            this.groupBox6.Controls.Add(this.radioButton1);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.txtSourceDDS);
            this.groupBox6.Controls.Add(this.btnBrowseDDS);
            this.groupBox6.Location = new System.Drawing.Point(13, 210);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(476, 213);
            this.groupBox6.TabIndex = 9;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Image Details:";
            this.groupBox6.Enter += new System.EventHandler(this.groupBox6_Enter);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(155, 187);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(25, 19);
            this.button4.TabIndex = 79;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblSpecularCustom
            // 
            this.lblSpecularCustom.AutoSize = true;
            this.lblSpecularCustom.Location = new System.Drawing.Point(186, 190);
            this.lblSpecularCustom.Name = "lblSpecularCustom";
            this.lblSpecularCustom.Size = new System.Drawing.Size(0, 13);
            this.lblSpecularCustom.TabIndex = 78;
            // 
            // chkPalette4Blend
            // 
            this.chkPalette4Blend.AutoSize = true;
            this.chkPalette4Blend.Checked = true;
            this.chkPalette4Blend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPalette4Blend.Enabled = false;
            this.chkPalette4Blend.Location = new System.Drawing.Point(250, 157);
            this.chkPalette4Blend.Name = "chkPalette4Blend";
            this.chkPalette4Blend.Size = new System.Drawing.Size(67, 17);
            this.chkPalette4Blend.TabIndex = 25;
            this.chkPalette4Blend.Text = "Blending";
            this.chkPalette4Blend.UseVisualStyleBackColor = true;
            // 
            // chkPalette3Blend
            // 
            this.chkPalette3Blend.AutoSize = true;
            this.chkPalette3Blend.Checked = true;
            this.chkPalette3Blend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPalette3Blend.Enabled = false;
            this.chkPalette3Blend.Location = new System.Drawing.Point(250, 111);
            this.chkPalette3Blend.Name = "chkPalette3Blend";
            this.chkPalette3Blend.Size = new System.Drawing.Size(67, 17);
            this.chkPalette3Blend.TabIndex = 24;
            this.chkPalette3Blend.Text = "Blending";
            this.chkPalette3Blend.UseVisualStyleBackColor = true;
            // 
            // chkPalette2Blend
            // 
            this.chkPalette2Blend.AutoSize = true;
            this.chkPalette2Blend.Checked = true;
            this.chkPalette2Blend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPalette2Blend.Enabled = false;
            this.chkPalette2Blend.Location = new System.Drawing.Point(17, 157);
            this.chkPalette2Blend.Name = "chkPalette2Blend";
            this.chkPalette2Blend.Size = new System.Drawing.Size(67, 17);
            this.chkPalette2Blend.TabIndex = 23;
            this.chkPalette2Blend.Text = "Blending";
            this.chkPalette2Blend.UseVisualStyleBackColor = true;
            // 
            // chkPalette1Blend
            // 
            this.chkPalette1Blend.AutoSize = true;
            this.chkPalette1Blend.Checked = true;
            this.chkPalette1Blend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPalette1Blend.Enabled = false;
            this.chkPalette1Blend.Location = new System.Drawing.Point(17, 111);
            this.chkPalette1Blend.Name = "chkPalette1Blend";
            this.chkPalette1Blend.Size = new System.Drawing.Size(67, 17);
            this.chkPalette1Blend.TabIndex = 22;
            this.chkPalette1Blend.Text = "Blending";
            this.chkPalette1Blend.UseVisualStyleBackColor = true;
            // 
            // chkAllowDecal
            // 
            this.chkAllowDecal.AutoSize = true;
            this.chkAllowDecal.Enabled = false;
            this.chkAllowDecal.Location = new System.Drawing.Point(190, 71);
            this.chkAllowDecal.Name = "chkAllowDecal";
            this.chkAllowDecal.Size = new System.Drawing.Size(146, 17);
            this.chkAllowDecal.TabIndex = 21;
            this.chkAllowDecal.Text = "Make this pattern a decal";
            this.chkAllowDecal.UseVisualStyleBackColor = true;
            this.chkAllowDecal.CheckedChanged += new System.EventHandler(this.chkAllowDecal_CheckedChanged);
            // 
            // chkUseDefaultSpecular
            // 
            this.chkUseDefaultSpecular.AutoSize = true;
            this.chkUseDefaultSpecular.Checked = true;
            this.chkUseDefaultSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultSpecular.Enabled = false;
            this.chkUseDefaultSpecular.Location = new System.Drawing.Point(9, 187);
            this.chkUseDefaultSpecular.Name = "chkUseDefaultSpecular";
            this.chkUseDefaultSpecular.Size = new System.Drawing.Size(146, 17);
            this.chkUseDefaultSpecular.TabIndex = 20;
            this.chkUseDefaultSpecular.Text = "Use default specular map";
            this.chkUseDefaultSpecular.UseVisualStyleBackColor = true;
            // 
            // lblBackgroundColour
            // 
            this.lblBackgroundColour.BackColor = System.Drawing.Color.Black;
            this.lblBackgroundColour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBackgroundColour.Enabled = false;
            this.lblBackgroundColour.Location = new System.Drawing.Point(109, 72);
            this.lblBackgroundColour.Name = "lblBackgroundColour";
            this.lblBackgroundColour.Size = new System.Drawing.Size(75, 15);
            this.lblBackgroundColour.TabIndex = 19;
            this.lblBackgroundColour.Click += new System.EventHandler(this.lblBackgroundColour_Click);
            // 
            // lblPalette4
            // 
            this.lblPalette4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPalette4.Enabled = false;
            this.lblPalette4.Location = new System.Drawing.Point(323, 141);
            this.lblPalette4.Name = "lblPalette4";
            this.lblPalette4.Size = new System.Drawing.Size(121, 40);
            this.lblPalette4.TabIndex = 18;
            this.lblPalette4.Click += new System.EventHandler(this.lblPalette4_Click);
            // 
            // lblPalette3
            // 
            this.lblPalette3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPalette3.Enabled = false;
            this.lblPalette3.Location = new System.Drawing.Point(323, 91);
            this.lblPalette3.Name = "lblPalette3";
            this.lblPalette3.Size = new System.Drawing.Size(121, 40);
            this.lblPalette3.TabIndex = 17;
            this.lblPalette3.Click += new System.EventHandler(this.lblPalette3_Click);
            // 
            // lblPalette2
            // 
            this.lblPalette2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPalette2.Enabled = false;
            this.lblPalette2.Location = new System.Drawing.Point(87, 140);
            this.lblPalette2.Name = "lblPalette2";
            this.lblPalette2.Size = new System.Drawing.Size(121, 40);
            this.lblPalette2.TabIndex = 16;
            this.lblPalette2.Click += new System.EventHandler(this.lblPalette2_Click);
            // 
            // lblPalette1
            // 
            this.lblPalette1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPalette1.Enabled = false;
            this.lblPalette1.Location = new System.Drawing.Point(87, 94);
            this.lblPalette1.Name = "lblPalette1";
            this.lblPalette1.Size = new System.Drawing.Size(121, 40);
            this.lblPalette1.TabIndex = 15;
            this.lblPalette1.Click += new System.EventHandler(this.lblPalette1_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Enabled = false;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(247, 141);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(62, 13);
            this.label19.TabIndex = 14;
            this.label19.Text = "Palette 4:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Enabled = false;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(247, 95);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(62, 13);
            this.label18.TabIndex = 13;
            this.label18.Text = "Palette 3:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Enabled = false;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(14, 141);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(62, 13);
            this.label17.TabIndex = 12;
            this.label17.Text = "Palette 2:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Enabled = false;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(14, 95);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Palette 1:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Enabled = false;
            this.label15.Location = new System.Drawing.Point(11, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(92, 13);
            this.label15.TabIndex = 10;
            this.label15.Text = "Background Filler:";
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Enabled = false;
            this.radioButton4.Location = new System.Drawing.Point(287, 47);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(31, 17);
            this.radioButton4.TabIndex = 9;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "4";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Enabled = false;
            this.radioButton3.Location = new System.Drawing.Point(250, 47);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(31, 17);
            this.radioButton3.TabIndex = 8;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "3";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(213, 47);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(31, 17);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "2";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Enabled = false;
            this.radioButton1.Location = new System.Drawing.Point(177, 47);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(31, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "1";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 49);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(160, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Number of recolourable palettes:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(11, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Source DDS:";
            // 
            // txtSourceDDS
            // 
            this.txtSourceDDS.Enabled = false;
            this.txtSourceDDS.Location = new System.Drawing.Point(87, 19);
            this.txtSourceDDS.Name = "txtSourceDDS";
            this.txtSourceDDS.Size = new System.Drawing.Size(318, 20);
            this.txtSourceDDS.TabIndex = 77;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(505, 47);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 256);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // chkShowRed
            // 
            this.chkShowRed.AutoSize = true;
            this.chkShowRed.Enabled = false;
            this.chkShowRed.Location = new System.Drawing.Point(507, 312);
            this.chkShowRed.Name = "chkShowRed";
            this.chkShowRed.Size = new System.Drawing.Size(46, 17);
            this.chkShowRed.TabIndex = 11;
            this.chkShowRed.Text = "Red";
            this.chkShowRed.UseVisualStyleBackColor = true;
            this.chkShowRed.CheckedChanged += new System.EventHandler(this.chkShowRed_CheckedChanged);
            // 
            // chkShowGreen
            // 
            this.chkShowGreen.AutoSize = true;
            this.chkShowGreen.Enabled = false;
            this.chkShowGreen.Location = new System.Drawing.Point(559, 312);
            this.chkShowGreen.Name = "chkShowGreen";
            this.chkShowGreen.Size = new System.Drawing.Size(55, 17);
            this.chkShowGreen.TabIndex = 12;
            this.chkShowGreen.Text = "Green";
            this.chkShowGreen.UseVisualStyleBackColor = true;
            this.chkShowGreen.CheckedChanged += new System.EventHandler(this.chkShowGreen_CheckedChanged);
            // 
            // chkShowBlue
            // 
            this.chkShowBlue.AutoSize = true;
            this.chkShowBlue.Enabled = false;
            this.chkShowBlue.Location = new System.Drawing.Point(615, 312);
            this.chkShowBlue.Name = "chkShowBlue";
            this.chkShowBlue.Size = new System.Drawing.Size(47, 17);
            this.chkShowBlue.TabIndex = 13;
            this.chkShowBlue.Text = "Blue";
            this.chkShowBlue.UseVisualStyleBackColor = true;
            this.chkShowBlue.CheckedChanged += new System.EventHandler(this.chkShowBlue_CheckedChanged);
            // 
            // chkShowAlpha
            // 
            this.chkShowAlpha.AutoSize = true;
            this.chkShowAlpha.Enabled = false;
            this.chkShowAlpha.Location = new System.Drawing.Point(668, 312);
            this.chkShowAlpha.Name = "chkShowAlpha";
            this.chkShowAlpha.Size = new System.Drawing.Size(53, 17);
            this.chkShowAlpha.TabIndex = 14;
            this.chkShowAlpha.Text = "Alpha";
            this.chkShowAlpha.UseVisualStyleBackColor = true;
            this.chkShowAlpha.CheckedChanged += new System.EventHandler(this.chkShowAlpha_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(503, 28);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(80, 13);
            this.label20.TabIndex = 15;
            this.label20.Text = "Image Preview:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(503, 362);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 48);
            this.label1.TabIndex = 16;
            this.label1.Text = "Please Wait... \r\nSaving in Progress....";
            this.label1.Visible = false;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(668, 335);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Preview Output";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(507, 334);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 24);
            this.button3.TabIndex = 18;
            this.button3.Text = "Show Source";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_2);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(773, 429);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.chkShowAlpha);
            this.Controls.Add(this.chkShowBlue);
            this.Controls.Add(this.chkShowGreen);
            this.Controls.Add(this.chkShowRed);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Delphy\'s Pattern Packager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnBrowseDDS;
        private System.Windows.Forms.ComboBox cmbSurfaceMat;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.ToolStripMenuItem loadPatternToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newPatternToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCreatorName;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtPatternTitle;
        private System.Windows.Forms.TextBox txtPatternDesc;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtSourceDDS;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblBackgroundColour;
        private System.Windows.Forms.Label lblPalette4;
        private System.Windows.Forms.Label lblPalette3;
        private System.Windows.Forms.Label lblPalette2;
        private System.Windows.Forms.Label lblPalette1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox chkUseDefaultSpecular;
        private System.Windows.Forms.CheckBox chkAllowDecal;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkShowRed;
        private System.Windows.Forms.CheckBox chkShowGreen;
        private System.Windows.Forms.CheckBox chkShowBlue;
        private System.Windows.Forms.CheckBox chkShowAlpha;
        private System.Windows.Forms.CheckBox chkPalette1Blend;
        private System.Windows.Forms.CheckBox chkPalette4Blend;
        private System.Windows.Forms.CheckBox chkPalette3Blend;
        private System.Windows.Forms.CheckBox chkPalette2Blend;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCreatorHomepage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lblSpecularCustom;
    }
}

