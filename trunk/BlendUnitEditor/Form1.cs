using System;
using System.Windows.Forms;
using System.IO;

namespace BlendUnitViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filename = "";
        private MadScience.Wrappers.BlendUnit blendunit = new MadScience.Wrappers.BlendUnit();

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
        }

        private void addListItem(string text, string subitem)
        {
            ListViewItem item = new ListViewItem();
            item.Text = text;
            item.SubItems.Add(subitem);
            listView1.Items.Add(item);

        }

        private void saveFile(string filename)
        {
            Stream output = File.OpenWrite(filename);
            blendunit.Save(output);
            output.Close();
        }

        private void loadFile(string filename)
        {

            this.filename = filename;

            MadScienceSmall.Helpers.logMessageToFile("Loading " + this.filename);

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);

            blendunit.Load(input);

            addListItem("Version", blendunit.version.ToString());
            uint fblendTgiOffset = blendunit.keytable.offset;
            addListItem("KeyTable offset", fblendTgiOffset.ToString());
            addListItem("KeyTable size", blendunit.keytable.size.ToString());
            ulong stringHash = blendunit.localeHash;
            addListItem("String hash", stringHash.ToString("X16"));
            txtStringLocaleHash.Text = stringHash.ToString("X16");

            addListItem("Num Indexers", blendunit.indexers.Count.ToString());

            for (int i = 0; i < blendunit.indexers.Count; i++)
            {
                addListItem("Indexer #" + i.ToString(), blendunit.indexers[i].ToString());
            }

            if (blendunit.bidirectional == 1) chkBiDirectional.Checked = true;
            else chkBiDirectional.Checked = false;

            addListItem("BiDirectional", blendunit.bidirectional.ToString());
            uint casPanelGroup = blendunit.casPanelGroup;
            string casPanelName = "";
            switch (casPanelGroup)
            {
                case 2:
                    casPanelName = "Head and Ears";
                    chkListCasPanelGroup.SetItemChecked(0, true);
                    break;
                case 8:
                    casPanelName = "Mouth";
                    chkListCasPanelGroup.SetItemChecked(1, true);
                    break;
                case 16:
                    casPanelName = "Nose";
                    chkListCasPanelGroup.SetItemChecked(2, true);
                    break;
                case 64:
                    casPanelName = "Eyelash";
                    chkListCasPanelGroup.SetItemChecked(3, true);
                    break;
                case 128:
                    casPanelName = "Eyes";
                    chkListCasPanelGroup.SetItemChecked(4, true);
                    break;
            }
            addListItem("Cas Panel Group", casPanelGroup.ToString() + " " + casPanelName);
            uint sortIndex = blendunit.casPanelSubGroup;
            txtSortIndex.Text = sortIndex.ToString();

            addListItem("Cas Panel Sub Group", sortIndex.ToString());
            
            int tgiCount = blendunit.keytable.keys.Count;
            for (int i = 0; i < tgiCount; i++)
            {
                addListItem("TGI #" + (i + 1).ToString(), blendunit.keytable.keys[i].ToString());
                if (i == 0) { txtSlideLeft.Text = blendunit.keytable.keys[i].ToString(); }
                if (i == 1) { txtSlideRight.Text = blendunit.keytable.keys[i].ToString(); }

            }

            input.Close();
        }

        private void chkListCasPanelGroup_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Uncheck all other boxes
            CheckedListBox lView = (CheckedListBox)sender;
            for (int i = 0; i < lView.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    lView.SetItemChecked(i, false);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            blendunit = new MadScience.Wrappers.BlendUnit();

            txtSlideLeft.Text = "";
            txtSlideRight.Text = "";
            txtSortIndex.Text = "";
            txtStringLocaleHash.Text = "";

            for (int i = 0; i < chkListCasPanelGroup.Items.Count; i++)
            {
                chkListCasPanelGroup.SetItemChecked(i, false);
            }

            chkBiDirectional.Checked = false;


        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveBlendUnit(saveFile);
            saveFile.Close();
        }

        private void saveBlendUnit(Stream saveFile)
        {
            // Save everything to the class

            // Clear down the indexers and tgi lists
            blendunit.indexers.Clear();
            blendunit.keytable.keys.Clear();

            if (!String.IsNullOrEmpty(txtSlideLeft.Text) && !String.IsNullOrEmpty(txtSlideRight.Text))
            {
                if (MadScienceSmall.Helpers.validateKey(txtSlideLeft.Text) && MadScienceSmall.Helpers.validateKey(txtSlideRight.Text))
                {
                    // Both sliders
                    blendunit.indexers.Add(0);
                    blendunit.indexers.Add(1);
                    MadScience.Wrappers.ResourceKey kTempLeft = new MadScience.Wrappers.ResourceKey(txtSlideLeft.Text);
                    MadScience.Wrappers.ResourceKey kTempRight = new MadScience.Wrappers.ResourceKey(txtSlideRight.Text);

                    blendunit.keytable.keys.Add(kTempLeft);
                    blendunit.keytable.keys.Add(kTempRight);

                    kTempLeft = null;
                    kTempRight = null;
                }
            }
            else
            {
                // Only one
                blendunit.indexers.Add(0);
                if (!String.IsNullOrEmpty(txtSlideLeft.Text) && MadScienceSmall.Helpers.validateKey(txtSlideLeft.Text))
                {
                    MadScience.Wrappers.ResourceKey kTemp = new MadScience.Wrappers.ResourceKey(txtSlideLeft.Text);
                    blendunit.keytable.keys.Add(kTemp);
                    kTemp = null;
                }
                if (!String.IsNullOrEmpty(txtSlideRight.Text) && MadScienceSmall.Helpers.validateKey(txtSlideRight.Text))
                {
                    MadScience.Wrappers.ResourceKey kTemp = new MadScience.Wrappers.ResourceKey(txtSlideRight.Text);
                    blendunit.keytable.keys.Add(kTemp);
                    kTemp = null;
                }

            }

            if (!chkBiDirectional.Checked)
            {
                blendunit.bidirectional = 0;
            }
            else
            {
                blendunit.bidirectional = 1;
            }

            blendunit.localeHash = MadScienceSmall.StringHelpers.ParseHex64("0x" + txtStringLocaleHash.Text);

            blendunit.casPanelSubGroup = Convert.ToUInt32(txtSortIndex.Text);

            if (chkListCasPanelGroup.GetItemChecked(0) == true) blendunit.casPanelGroup = 2;
            if (chkListCasPanelGroup.GetItemChecked(1) == true) blendunit.casPanelGroup = 8;
            if (chkListCasPanelGroup.GetItemChecked(2) == true) blendunit.casPanelGroup = 16;
            if (chkListCasPanelGroup.GetItemChecked(3) == true) blendunit.casPanelGroup = 64;
            if (chkListCasPanelGroup.GetItemChecked(4) == true) blendunit.casPanelGroup = 128;

            blendunit.Save(saveFile);

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Blend Unit File|*.blendunit";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream saveFile = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                saveBlendUnit(saveFile);
                saveFile.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Blend Unit File|*.blendunit";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }
        }

        private void chkListCasPanelGroup_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



    }
}
