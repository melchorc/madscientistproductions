using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RSLTEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filename = "";

		private MadScience.Wrappers.RSLTFile rsltFile = new MadScience.Wrappers.RSLTFile();

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
        }

        private void showMatrix(MadScience.Wrappers.Matrix4by3 matrix)
        {
			txtMatrix11.Text = matrix.rc11.ToString();
            txtMatrix12.Text = matrix.rc12.ToString();
            txtMatrix13.Text = matrix.rc13.ToString();
			txtMatrix14.Text = matrix.rc14.ToString();
			txtMatrix21.Text = matrix.rc21.ToString();
			txtMatrix22.Text = matrix.rc22.ToString();
			txtMatrix23.Text = matrix.rc23.ToString();
			txtMatrix24.Text = matrix.rc24.ToString();
			txtMatrix31.Text = matrix.rc31.ToString();
			txtMatrix32.Text = matrix.rc32.ToString();
			txtMatrix33.Text = matrix.rc33.ToString();
			txtMatrix34.Text = matrix.rc34.ToString();
        }

        private void loadFile(string filename)
        {

            this.filename = filename;
            this.rsltFile.rslt = new MadScience.Wrappers.RSLT();

            toolStripStatusLabel1.Text = this.filename;

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            this.rsltFile.Load(input);
            input.Close();

			// Add routes
			for (int i = 0; i < this.rsltFile.rslt.Routes.Count; i++)
			{
				ListViewItem item = new ListViewItem();
				item.Text = (i + 1).ToString();
				item.SubItems.Add(this.rsltFile.rslt.Routes[i].slotName.ToString("X8"));

				lstRoutes.Items.Add(item);
			}

            saveToolStripMenuItem.Enabled = true;

            Console.WriteLine("Done loading");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rsltFile = new MadScience.Wrappers.RSLTFile();

            foreach (Control control in this.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox3.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            button2.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveVPXY(saveFile);
            saveFile.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Slot File|*.slot";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filename = saveFileDialog1.FileName;
                toolStripStatusLabel1.Text = this.filename;
                Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                saveVPXY(saveFile);
                saveFile.Close();
            }
        }

        private void saveVPXY(Stream output)
        {
            // Populate bounding box
            try
            {
				/*
                this.vpxyFile.vpxy.boundingbox.min.x = Convert.ToSingle(txtMatrix11.Text);
                this.vpxyFile.vpxy.boundingbox.min.y = Convert.ToSingle(txtMatrix12.Text);
                this.vpxyFile.vpxy.boundingbox.min.z = Convert.ToSingle(txtMatrix13.Text);
                this.vpxyFile.vpxy.boundingbox.max.x = Convert.ToSingle(txtMatrix21.Text);
                this.vpxyFile.vpxy.boundingbox.max.y = Convert.ToSingle(txtMatrix22.Text);
                this.vpxyFile.vpxy.boundingbox.max.z = Convert.ToSingle(txtMatrix23.Text);
				 * */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //this.vpxyFile.Save(output);

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Slot File|*.slot";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
			/*
            if (listView3.SelectedItems.Count > 0)
            {
                // Get the VPXYEntry
                vpxyFile.vpxy.seprEntries.RemoveAt(listView3.SelectedIndices[0]);
                showSeprList();

                textBox1.Text = "";
                //showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
			 */
        }

        private void button4_Click(object sender, EventArgs e)
        {
			/*
            if (listView3.SelectedItems.Count > 0)
            {
                if (MadScienceSmall.Helpers.validateKey(textBox1.Text))
                {
                    vpxyFile.vpxy.seprEntries[listView3.SelectedIndices[0]].tgiList[0] = new MadScience.Wrappers.ResourceKey(textBox1.Text);
                    listView3.SelectedItems[0].Text = vpxyFile.vpxy.seprEntries[listView3.SelectedIndices[0]].tgiList[0].ToString();
                }
            }
			 * */
        }

        private void button6_Click(object sender, EventArgs e)
        {
			/*
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "key:00000000:00000000:0000000000000000";
            }
            if (MadScienceSmall.Helpers.validateKey(textBox1.Text))
            {
                MadScience.Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(textBox1.Text);
                MadScience.Wrappers.VPXYEntry entry = new MadScience.Wrappers.VPXYEntry();
                entry.type = 1;
                entry.tgiList.Add(rKey);

                vpxyFile.vpxy.seprEntries.Add(entry);
                
                
                ListViewItem item = new ListViewItem();
                item.Text = (listView3.Items.Count).ToString();
                item.SubItems.Add(rKey.ToString());
                listView3.Items.Add(item);
                item = null;

                entry = null;
                //rKey = null;
            }
			 */
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
			/*
            if (listView3.SelectedItems.Count > 0)
            {
                textBox1.Text = listView3.SelectedItems[0].SubItems[1].Text;
                textBox1.Enabled = true;
                button5.Enabled = true;
                button4.Enabled = true;
            }
			 * */
        }

        private void button7_Click(object sender, EventArgs e)
        {
			/*
            if (listView2.SelectedItems.Count > 0)
            {
                //textBox2.Text = "";
                //StringBuilder sb = new StringBuilder();

                MadScience.Wrappers.VPXYEntry entry = this.vpxyFile.vpxy.linkEntries[listView2.SelectedIndices[0]];
                entry.tgiList.Clear();

                foreach (string line in textBox2.Lines)
                {
                    if (!String.IsNullOrEmpty(line.Trim()))
                    {
                        if (MadScienceSmall.Helpers.validateKey(line))
                        {
                            entry.tgiList.Add(new MadScience.Wrappers.ResourceKey(line));
                        }
                    }
                }

                listView2.SelectedItems[0].SubItems[1].Text = "List with " + entry.tgiList.Count.ToString() + " items";
            }
			 * */
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
			/*
            if (listView2.SelectedItems.Count > 0)
            {
                this.vpxyFile.vpxy.linkEntries.RemoveAt(listView2.SelectedIndices[0]);
                showLinkList();
            }
			 * */
        }

        private void button1_Click(object sender, EventArgs e)
        {
			/*
            MadScience.Wrappers.VPXYEntry entry = new MadScience.Wrappers.VPXYEntry();
            this.vpxyFile.vpxy.linkEntries.Add(entry);

            showLinkList();
			 * */
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void rCOLHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MadScience.Wrappers.RCOLHeaderEditor rForm = new MadScience.Wrappers.RCOLHeaderEditor();
            rForm.rcolHeader = this.rsltFile.rcolHeader;
            if (rForm.ShowDialog() == DialogResult.OK)
            {
                this.rsltFile.rcolHeader = rForm.rcolHeader;
            }
            rForm.Close();
            rForm = null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lstRoutes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstRoutes.SelectedItems.Count > 0)
			{
				MadScience.Wrappers.RSLTRoute entry = this.rsltFile.rslt.Routes[lstRoutes.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);
			}

		}

		private void lstContainers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstContainers.SelectedItems.Count > 0)
			{
				MadScience.Wrappers.RSLTContainer entry = this.rsltFile.rslt.Containers[lstContainers.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = entry.flags.ToString();
				txtFlags.Enabled = true;
				showMatrix(entry.matrix);
			}
		}

		private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstEffects.SelectedItems.Count > 0)
			{
				MadScience.Wrappers.RSLTEffect entry = this.rsltFile.rslt.Effects[lstEffects.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);
			}
		}

		private void lstIKTargets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstIKTargets.SelectedItems.Count > 0)
			{
				MadScience.Wrappers.RSLTIKTarget entry = this.rsltFile.rslt.IKTargets[lstIKTargets.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);
			}
		}
    }
}
