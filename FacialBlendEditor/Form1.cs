using System;
using System.Windows.Forms;
using System.IO;
using MadScience.Wrappers;

namespace FacialBlendEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filename = "";
        private MadScience.Wrappers.FacialBlend faceblend = new MadScience.Wrappers.FacialBlend();

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
        }

        private void loadFile(string filename)
        {

            this.filename = filename;
            this.faceblend = new MadScience.Wrappers.FacialBlend();

            foreach (Control control in this.Controls)
            {
                Console.WriteLine(control.Name + " " + control.ToString());
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox1.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            this.label12.Text = "";

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            faceblend.Load(input);
            input.Close();

            txtUnk1.Text = faceblend.blendType.ToString();
            txtBlendGeometry.Text = faceblend.blendTgi.ToString();
            txtPartName.Text = faceblend.partName;
            label12.Text = MadScience.StringHelpers.HashFNV64(txtPartName.Text).ToString("X16");

            for (int i = 0; i < faceblend.geomBoneEntries.Count; i++)
            {
                cmbChooseGeomEntry.Items.Add("Entry #" + (i + 1));
            }

            cmbChooseGeomEntry.SelectedIndex = -1;
            if (faceblend.geomBoneEntries.Count > 0)
            {
                cmbChooseGeomEntry.SelectedIndex = 0;
            }

            for (int i = 0; i < faceblend.keytable.keys.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "TGI #" + i.ToString();
                item.SubItems.Add(faceblend.keytable.keys[i].ToString());
                listView1.Items.Add(item);
                item = null;
            }

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.faceblend = new MadScience.Wrappers.FacialBlend();

            foreach (Control control in this.Controls)
            {
                Console.WriteLine(control.Name + " " + control.ToString());
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox1.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            this.label12.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Face Blend File|*.facialblend";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveFacialBlend(saveFile);
            saveFile.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Facial Blend File|*.facialblend";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream saveFile = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                saveFacialBlend(saveFile);
                saveFile.Close();
            }
        }

        private void saveFacialBlend(Stream saveFile)
        {
            faceblend.partName = txtPartName.Text;
            faceblend.blendTgi = new MadScience.Wrappers.ResourceKey(txtBlendGeometry.Text);
            faceblend.blendType = Convert.ToUInt32(txtUnk1.Text);
            faceblend.Save(saveFile);

        }

        private void cmbChooseGeomEntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChooseGeomEntry.SelectedIndex < faceblend.geomBoneEntries.Count)
            {
                showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
            else
            {
                foreach (Control control in this.groupBox1.Controls)
                {
                    MadScienceSmall.Helpers.resetControl(control);
                }
            }
        }

        private void showGbEntry(int index)
        {
            if (index == -1) return;

            MadScience.Wrappers.FacialBlendGeomBoneEntry gbEntry = faceblend.geomBoneEntries[index];

            if (gbEntry.hasGeomAndBone == 1) chkHasGeomAndBone.Checked = true;
            else chkHasGeomAndBone.Checked = false;

            if (gbEntry.hasBoneEntry == 1) chkHasBone.Checked = true;
            else chkHasBone.Checked = false;

            if (gbEntry.hasGeomEntry == 1) chkHasGeom.Checked = true;
            else chkHasGeom.Checked = false;

            MadScienceSmall.Helpers.resetControl(checkedListAge);
            MadScienceSmall.Helpers.resetControl(checkedListAge2);
            MadScienceSmall.Helpers.resetControl(checkedListGender);
            MadScienceSmall.Helpers.resetControl(checkedListGender2);

            txtAmount.Text = gbEntry.amount.ToString();
            txtAmount2.Text = gbEntry.amount2.ToString();

            txtGeomIndex.Text = gbEntry.geomEntryIndex.ToString();
            txtBoneIndex.Text = gbEntry.boneIndex.ToString();

            cmbTGIlist.Items.Clear();
            for (int i = 0; i < faceblend.keytable.keys.Count; i++)
            {
                cmbTGIlist.Items.Add(faceblend.keytable.keys[i].ToString());
            }
            if (gbEntry.geomEntryIndex > 0)
            {
                if (gbEntry.geomEntryIndex < cmbTGIlist.Items.Count)
                {
                    cmbTGIlist.SelectedIndex = (int)gbEntry.geomEntryIndex;
                }
            }
            else
            {
                if (gbEntry.boneIndex < cmbTGIlist.Items.Count)
                {
                    cmbTGIlist.SelectedIndex = (int)gbEntry.boneIndex;
                }
            }
            //txtTGILink.Text = faceblend.tgiList[(int)gbEntry.geomEntryIndex].ToString();

            if ((gbEntry.ageGenderFlags & 0x2) == 0x2) checkedListAge.SetItemChecked(0, true); // Toddler
            if ((gbEntry.ageGenderFlags & 0x4) == 0x4) checkedListAge.SetItemChecked(1, true); // Child
            if ((gbEntry.ageGenderFlags & 0x8) == 0x8) checkedListAge.SetItemChecked(2, true); // Teen
            if ((gbEntry.ageGenderFlags & 0x10) == 0x10) checkedListAge.SetItemChecked(3, true); // YoungAdult
            if ((gbEntry.ageGenderFlags & 0x20) == 0x20) checkedListAge.SetItemChecked(4, true); // Adult
            if ((gbEntry.ageGenderFlags & 0x40) == 0x40) checkedListAge.SetItemChecked(5, true); // Elder

            if ((gbEntry.ageGenderFlags & 0x1000) == 0x1000) checkedListGender.SetItemChecked(0, true); // Male
            if ((gbEntry.ageGenderFlags & 0x2000) == 0x2000) checkedListGender.SetItemChecked(1, true); // Female

            if ((gbEntry.ageGenderFlags2 & 0x2) == 0x2) checkedListAge2.SetItemChecked(0, true); // Toddler
            if ((gbEntry.ageGenderFlags2 & 0x4) == 0x4) checkedListAge2.SetItemChecked(1, true); // Child
            if ((gbEntry.ageGenderFlags2 & 0x8) == 0x8) checkedListAge2.SetItemChecked(2, true); // Teen
            if ((gbEntry.ageGenderFlags2 & 0x10) == 0x10) checkedListAge2.SetItemChecked(3, true); // YoungAdult
            if ((gbEntry.ageGenderFlags2 & 0x20) == 0x20) checkedListAge2.SetItemChecked(4, true); // Adult
            if ((gbEntry.ageGenderFlags2 & 0x40) == 0x40) checkedListAge2.SetItemChecked(5, true); // Elder

            if ((gbEntry.ageGenderFlags2 & 0x1000) == 0x1000) checkedListGender2.SetItemChecked(0, true); // Male
            if ((gbEntry.ageGenderFlags2 & 0x2000) == 0x2000) checkedListGender2.SetItemChecked(1, true); // Female


            //txtNewTGI.Text = gbEntry.regionFlag.ToString();

            switch (gbEntry.regionFlag)
            {
                case 0x400:
                    cmbRegionType.SelectedIndex = 0;
                    break;
                case 0x100:
                    cmbRegionType.SelectedIndex = 1;
                    break;
                case 0x10:
                    cmbRegionType.SelectedIndex = 2;
                    break;
                case 0x800:
                    cmbRegionType.SelectedIndex = 3;
                    break;
                case 0x1:
                    cmbRegionType.SelectedIndex = 4;
                    break;
                case 0x40:
                    cmbRegionType.SelectedIndex = 5;
                    break;
                case 0x80:
                    cmbRegionType.SelectedIndex = 6;
                    break;
                case 0x200:
                    cmbRegionType.SelectedIndex = 7;
                    break;
                case 0x4:
                    cmbRegionType.SelectedIndex = 8;
                    break;
                case 0x2:
                    cmbRegionType.SelectedIndex = 9;
                    break;
                case 0x20:
                    cmbRegionType.SelectedIndex = 10;
                    break;
                case 0x8:
                    cmbRegionType.SelectedIndex = 11;
                    break;

            }

        }

        private void btnGeomBoneCommit_Click(object sender, EventArgs e)
        {
            // Figure out the current maximum
            int curMax = faceblend.geomBoneEntries.Count;
            int toAdd = (cmbChooseGeomEntry.SelectedIndex + 1) - curMax;
            for (int i = 0; i < toAdd; i++)
            {
                MadScience.Wrappers.FacialBlendGeomBoneEntry gbEntryT = new MadScience.Wrappers.FacialBlendGeomBoneEntry();
                faceblend.geomBoneEntries.Add(gbEntryT);
                gbEntryT = null;
            }

            // Commit
            MadScience.Wrappers.FacialBlendGeomBoneEntry gbEntry = faceblend.geomBoneEntries[cmbChooseGeomEntry.SelectedIndex];
            gbEntry.amount = Convert.ToUInt32(txtAmount.Text);
            gbEntry.amount2 = Convert.ToUInt32(txtAmount2.Text);
            //gbEntry.boneIndex = Convert.ToUInt32(txtBoneIndex.Text);
            gbEntry.boneIndex = Convert.ToUInt32(cmbTGIlist.SelectedIndex);
            gbEntry.geomEntryIndex = Convert.ToUInt32(txtGeomIndex.Text);
            gbEntry.hasBoneEntry = Convert.ToUInt32(chkHasBone.Checked);
            gbEntry.hasGeomAndBone = Convert.ToUInt32(chkHasGeomAndBone.Checked);
            gbEntry.hasGeomEntry = Convert.ToUInt32(chkHasGeom.Checked);
            switch (cmbRegionType.SelectedIndex)
            {
                case 0:
                    gbEntry.regionFlag = (uint)FacialRegions.Body;
                    break;
                case 1:
                    gbEntry.regionFlag = (uint)FacialRegions.Brow;
                    break;
                case 2:
                    gbEntry.regionFlag = (uint)FacialRegions.Ears;
                    break;
                case 3:
                    gbEntry.regionFlag = (uint)FacialRegions.Eyelashes;
                    break;
                case 4:
                    gbEntry.regionFlag = (uint)FacialRegions.Eyes;
                    break;
                case 5:
                    gbEntry.regionFlag = (uint)FacialRegions.Face;
                    break;
                case 6:
                    gbEntry.regionFlag = (uint)FacialRegions.Head;
                    break;
                case 7:
                    gbEntry.regionFlag = (uint)FacialRegions.Jaw;
                    break;
                case 8:
                    gbEntry.regionFlag = (uint)FacialRegions.Mouth;
                    break;
                case 9:
                    gbEntry.regionFlag = (uint)FacialRegions.Nose;
                    break;
                case 10:
                    gbEntry.regionFlag = (uint)FacialRegions.TranslateEyes;
                    break;
                case 11:
                    gbEntry.regionFlag = (uint)FacialRegions.TranslateMouth;
                    break;

            }

            uint ageGenderFlag = 0;
            if (checkedListAge.GetItemChecked(0)) ageGenderFlag += 0x2;
            if (checkedListAge.GetItemChecked(1)) ageGenderFlag += 0x4;
            if (checkedListAge.GetItemChecked(2)) ageGenderFlag += 0x8;
            if (checkedListAge.GetItemChecked(3)) ageGenderFlag += 0x10;
            if (checkedListAge.GetItemChecked(4)) ageGenderFlag += 0x20;
            if (checkedListAge.GetItemChecked(5)) ageGenderFlag += 0x40;
            if (checkedListGender.GetItemChecked(0)) ageGenderFlag += 0x1000;
            if (checkedListGender.GetItemChecked(1)) ageGenderFlag += 0x2000;
            ageGenderFlag += 0x10000;

            uint ageGenderFlag2 = 0;
            if (checkedListAge2.GetItemChecked(0)) ageGenderFlag2 += 0x2;
            if (checkedListAge2.GetItemChecked(1)) ageGenderFlag2 += 0x4;
            if (checkedListAge2.GetItemChecked(2)) ageGenderFlag2 += 0x8;
            if (checkedListAge2.GetItemChecked(3)) ageGenderFlag2 += 0x10;
            if (checkedListAge2.GetItemChecked(4)) ageGenderFlag2 += 0x20;
            if (checkedListAge2.GetItemChecked(5)) ageGenderFlag2 += 0x40;
            if (checkedListGender2.GetItemChecked(0)) ageGenderFlag2 += 0x1000;
            if (checkedListGender2.GetItemChecked(1)) ageGenderFlag2 += 0x2000;
            ageGenderFlag2 += 0x10000;

            gbEntry.ageGenderFlags = ageGenderFlag;
            gbEntry.ageGenderFlags2 = ageGenderFlag2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNewTGI.Text) == false && MadScienceSmall.Helpers.validateKey(txtNewTGI.Text))
            {
                MadScience.Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(txtNewTGI.Text);
                faceblend.keytable.keys.Add(rKey);
                ListViewItem item = new ListViewItem();
                item.Text = "TGI #" + listView1.Items.Count.ToString();
                item.SubItems.Add(rKey.ToString());
                listView1.Items.Add(item);
                //rKey = null;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                txtNewTGI.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Check if anything uses this TGI
                faceblend.keytable.keys[listView1.SelectedIndices[0]] = new MadScience.Wrappers.ResourceKey(txtNewTGI.Text);
                listView1.Items.Clear();
                for (int i = 0; i < faceblend.keytable.keys.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = "TGI #" + i.ToString();
                    item.SubItems.Add(faceblend.keytable.keys[i].ToString());
                    listView1.Items.Add(item);
                    item = null;
                }
                showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                faceblend.keytable.keys.RemoveAt(listView1.SelectedIndices[0]);
                listView1.Items.Clear();
                for (int i = 0; i < faceblend.keytable.keys.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = "TGI #" + i.ToString();
                    item.SubItems.Add(faceblend.keytable.keys[i].ToString());
                    listView1.Items.Add(item);
                    item = null;
                }
                txtNewTGI.Text = "";
                showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
        }

        private void chkHasBone_CheckedChanged(object sender, EventArgs e)
        {
            checkedListAge2.Enabled = chkHasBone.Checked;
            checkedListGender2.Enabled = chkHasBone.Checked;
            txtAmount2.Enabled = chkHasBone.Checked;
        }

        private void txtNewTGI_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbTGIlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBoneIndex.Text = cmbTGIlist.SelectedIndex.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            int maxEntry = faceblend.geomBoneEntries.Count;

            FacialBlendGeomBoneEntry gbEntry = new FacialBlendGeomBoneEntry();
            
            faceblend.geomBoneEntries.Add(gbEntry);

            cmbChooseGeomEntry.Items.Add("Entry #" + (maxEntry + 1).ToString());
            cmbChooseGeomEntry.SelectedIndex = maxEntry;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (cmbChooseGeomEntry.SelectedIndex > -1)
            {
                int entryNo = cmbChooseGeomEntry.SelectedIndex;
                faceblend.geomBoneEntries.RemoveAt(entryNo);

                cmbChooseGeomEntry.Items.Clear();
                for (int i = 0; i < faceblend.geomBoneEntries.Count; i++)
                {
                    cmbChooseGeomEntry.Items.Add("Entry #" + (i + 1));
                }

                cmbChooseGeomEntry.SelectedIndex = 0;
            }
        }

    }
}
