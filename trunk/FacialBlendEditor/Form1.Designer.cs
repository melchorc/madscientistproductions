namespace FacialBlendEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBlendGeometry = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPartName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbTGIlist = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtBoneIndex = new System.Windows.Forms.TextBox();
            this.btnGeomBoneCommit = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGeomIndex = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAmount2 = new System.Windows.Forms.TextBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.checkedListGender2 = new System.Windows.Forms.CheckedListBox();
            this.checkedListAge2 = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.checkedListGender = new System.Windows.Forms.CheckedListBox();
            this.checkedListAge = new System.Windows.Forms.CheckedListBox();
            this.label90 = new System.Windows.Forms.Label();
            this.label89 = new System.Windows.Forms.Label();
            this.chkHasBone = new System.Windows.Forms.CheckBox();
            this.chkHasGeom = new System.Windows.Forms.CheckBox();
            this.chkHasGeomAndBone = new System.Windows.Forms.CheckBox();
            this.cmbRegionType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbChooseGeomEntry = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtNewTGI = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtUnk1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Menu;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(395, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(122, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Blend Geometry:";
            // 
            // txtBlendGeometry
            // 
            this.txtBlendGeometry.Location = new System.Drawing.Point(112, 33);
            this.txtBlendGeometry.Name = "txtBlendGeometry";
            this.txtBlendGeometry.Size = new System.Drawing.Size(277, 20);
            this.txtBlendGeometry.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Part Name:";
            // 
            // txtPartName
            // 
            this.txtPartName.Location = new System.Drawing.Point(112, 62);
            this.txtPartName.Name = "txtPartName";
            this.txtPartName.Size = new System.Drawing.Size(226, 20);
            this.txtPartName.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbTGIlist);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtBoneIndex);
            this.groupBox1.Controls.Add(this.btnGeomBoneCommit);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtGeomIndex);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtAmount2);
            this.groupBox1.Controls.Add(this.txtAmount);
            this.groupBox1.Controls.Add(this.checkedListGender2);
            this.groupBox1.Controls.Add(this.checkedListAge2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.checkedListGender);
            this.groupBox1.Controls.Add(this.checkedListAge);
            this.groupBox1.Controls.Add(this.label90);
            this.groupBox1.Controls.Add(this.label89);
            this.groupBox1.Controls.Add(this.chkHasBone);
            this.groupBox1.Controls.Add(this.chkHasGeom);
            this.groupBox1.Controls.Add(this.chkHasGeomAndBone);
            this.groupBox1.Controls.Add(this.cmbRegionType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 149);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 361);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Geom/Bone Entry";
            // 
            // cmbTGIlist
            // 
            this.cmbTGIlist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTGIlist.FormattingEnabled = true;
            this.cmbTGIlist.Location = new System.Drawing.Point(69, 331);
            this.cmbTGIlist.Name = "cmbTGIlist";
            this.cmbTGIlist.Size = new System.Drawing.Size(285, 21);
            this.cmbTGIlist.TabIndex = 43;
            this.cmbTGIlist.SelectedIndexChanged += new System.EventHandler(this.cmbTGIlist_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 334);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "TGI Link:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 41;
            this.label10.Text = "Bone Index:";
            // 
            // txtBoneIndex
            // 
            this.txtBoneIndex.Location = new System.Drawing.Point(93, 75);
            this.txtBoneIndex.Name = "txtBoneIndex";
            this.txtBoneIndex.Size = new System.Drawing.Size(100, 20);
            this.txtBoneIndex.TabIndex = 40;
            // 
            // btnGeomBoneCommit
            // 
            this.btnGeomBoneCommit.Location = new System.Drawing.Point(272, 299);
            this.btnGeomBoneCommit.Name = "btnGeomBoneCommit";
            this.btnGeomBoneCommit.Size = new System.Drawing.Size(75, 23);
            this.btnGeomBoneCommit.TabIndex = 38;
            this.btnGeomBoneCommit.Text = "Commit";
            this.btnGeomBoneCommit.UseVisualStyleBackColor = true;
            this.btnGeomBoneCommit.Click += new System.EventHandler(this.btnGeomBoneCommit_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Geom Index:";
            // 
            // txtGeomIndex
            // 
            this.txtGeomIndex.Location = new System.Drawing.Point(93, 49);
            this.txtGeomIndex.Name = "txtGeomIndex";
            this.txtGeomIndex.Size = new System.Drawing.Size(100, 20);
            this.txtGeomIndex.TabIndex = 36;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(247, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Amount #2:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(244, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Amount:";
            // 
            // txtAmount2
            // 
            this.txtAmount2.Location = new System.Drawing.Point(247, 228);
            this.txtAmount2.Name = "txtAmount2";
            this.txtAmount2.Size = new System.Drawing.Size(100, 20);
            this.txtAmount2.TabIndex = 33;
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(247, 117);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(100, 20);
            this.txtAmount.TabIndex = 32;
            // 
            // checkedListGender2
            // 
            this.checkedListGender2.FormattingEnabled = true;
            this.checkedListGender2.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.checkedListGender2.Location = new System.Drawing.Point(136, 228);
            this.checkedListGender2.Name = "checkedListGender2";
            this.checkedListGender2.Size = new System.Drawing.Size(105, 94);
            this.checkedListGender2.TabIndex = 31;
            // 
            // checkedListAge2
            // 
            this.checkedListAge2.FormattingEnabled = true;
            this.checkedListAge2.Items.AddRange(new object[] {
            "Toddler",
            "Child",
            "Teen",
            "Young Adult",
            "Adult",
            "Elder"});
            this.checkedListAge2.Location = new System.Drawing.Point(15, 228);
            this.checkedListAge2.Name = "checkedListAge2";
            this.checkedListAge2.Size = new System.Drawing.Size(115, 94);
            this.checkedListAge2.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(133, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Gender #2:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Age #2:";
            // 
            // checkedListGender
            // 
            this.checkedListGender.FormattingEnabled = true;
            this.checkedListGender.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.checkedListGender.Location = new System.Drawing.Point(136, 117);
            this.checkedListGender.Name = "checkedListGender";
            this.checkedListGender.Size = new System.Drawing.Size(105, 94);
            this.checkedListGender.TabIndex = 27;
            // 
            // checkedListAge
            // 
            this.checkedListAge.FormattingEnabled = true;
            this.checkedListAge.Items.AddRange(new object[] {
            "Toddler",
            "Child",
            "Teen",
            "Young Adult",
            "Adult",
            "Elder"});
            this.checkedListAge.Location = new System.Drawing.Point(15, 117);
            this.checkedListAge.Name = "checkedListAge";
            this.checkedListAge.Size = new System.Drawing.Size(115, 94);
            this.checkedListAge.TabIndex = 26;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label90.Location = new System.Drawing.Point(133, 101);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(52, 13);
            this.label90.TabIndex = 25;
            this.label90.Text = "Gender:";
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label89.Location = new System.Drawing.Point(12, 101);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(33, 13);
            this.label89.TabIndex = 24;
            this.label89.Text = "Age:";
            // 
            // chkHasBone
            // 
            this.chkHasBone.AutoSize = true;
            this.chkHasBone.Location = new System.Drawing.Point(199, 78);
            this.chkHasBone.Name = "chkHasBone";
            this.chkHasBone.Size = new System.Drawing.Size(106, 17);
            this.chkHasBone.TabIndex = 4;
            this.chkHasBone.Text = "Has Bone Entry?";
            this.chkHasBone.UseVisualStyleBackColor = true;
            this.chkHasBone.CheckedChanged += new System.EventHandler(this.chkHasBone_CheckedChanged);
            // 
            // chkHasGeom
            // 
            this.chkHasGeom.AutoSize = true;
            this.chkHasGeom.Location = new System.Drawing.Point(199, 51);
            this.chkHasGeom.Name = "chkHasGeom";
            this.chkHasGeom.Size = new System.Drawing.Size(109, 17);
            this.chkHasGeom.TabIndex = 3;
            this.chkHasGeom.Text = "Has Geom Entry?";
            this.chkHasGeom.UseVisualStyleBackColor = true;
            // 
            // chkHasGeomAndBone
            // 
            this.chkHasGeomAndBone.AutoSize = true;
            this.chkHasGeomAndBone.Location = new System.Drawing.Point(199, 25);
            this.chkHasGeomAndBone.Name = "chkHasGeomAndBone";
            this.chkHasGeomAndBone.Size = new System.Drawing.Size(155, 17);
            this.chkHasGeomAndBone.TabIndex = 2;
            this.chkHasGeomAndBone.Text = "Has both Geom and Bone?";
            this.chkHasGeomAndBone.UseVisualStyleBackColor = true;
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
            this.cmbRegionType.Location = new System.Drawing.Point(72, 22);
            this.cmbRegionType.Name = "cmbRegionType";
            this.cmbRegionType.Size = new System.Drawing.Size(121, 21);
            this.cmbRegionType.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Region:";
            // 
            // cmbChooseGeomEntry
            // 
            this.cmbChooseGeomEntry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChooseGeomEntry.FormattingEnabled = true;
            this.cmbChooseGeomEntry.Location = new System.Drawing.Point(112, 88);
            this.cmbChooseGeomEntry.Name = "cmbChooseGeomEntry";
            this.cmbChooseGeomEntry.Size = new System.Drawing.Size(119, 21);
            this.cmbChooseGeomEntry.TabIndex = 1;
            this.cmbChooseGeomEntry.SelectedIndexChanged += new System.EventHandler(this.cmbChooseGeomEntry_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 91);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Geom / Bone Entry:";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(15, 515);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(367, 98);
            this.listView1.TabIndex = 21;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "TGI";
            this.columnHeader2.Width = 250;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(307, 645);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 22;
            this.button1.Text = "Add TGI";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 645);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 23;
            this.button2.Text = "Delete TGI";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtNewTGI
            // 
            this.txtNewTGI.Location = new System.Drawing.Point(15, 619);
            this.txtNewTGI.Name = "txtNewTGI";
            this.txtNewTGI.Size = new System.Drawing.Size(367, 20);
            this.txtNewTGI.TabIndex = 24;
            this.txtNewTGI.TextChanged += new System.EventHandler(this.txtNewTGI_TextChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(226, 645);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 25;
            this.button3.Text = "Save TGI";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(237, 91);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 13);
            this.label12.TabIndex = 26;
            // 
            // txtUnk1
            // 
            this.txtUnk1.Location = new System.Drawing.Point(342, 62);
            this.txtUnk1.Name = "txtUnk1";
            this.txtUnk1.Size = new System.Drawing.Size(47, 20);
            this.txtUnk1.TabIndex = 27;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(112, 115);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 28;
            this.button4.Text = "Add Entry";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(234, 115);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 29;
            this.button5.Text = "Delete Entry";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 683);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.txtUnk1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtNewTGI);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbChooseGeomEntry);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtPartName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBlendGeometry);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Facial Blend Editor by Delphy";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBlendGeometry;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPartName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkHasBone;
        private System.Windows.Forms.CheckBox chkHasGeom;
        private System.Windows.Forms.CheckBox chkHasGeomAndBone;
        private System.Windows.Forms.ComboBox cmbRegionType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtGeomIndex;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAmount2;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.CheckedListBox checkedListGender2;
        private System.Windows.Forms.CheckedListBox checkedListAge2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckedListBox checkedListGender;
        private System.Windows.Forms.CheckedListBox checkedListAge;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.ComboBox cmbChooseGeomEntry;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnGeomBoneCommit;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtBoneIndex;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ComboBox cmbTGIlist;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtNewTGI;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtUnk1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}

