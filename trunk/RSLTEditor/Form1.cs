using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace RSLTEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filename = "";
		private int clickedTab = 0;

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

		private MadScience.Wrappers.Matrix4by3 getMatrix()
		{
			MadScience.Wrappers.Matrix4by3 matrix = new MadScience.Wrappers.Matrix4by3();
			matrix.rc11 = Convert.ToSingle(txtMatrix11.Text, CultureInfo.InvariantCulture);
			matrix.rc12 = Convert.ToSingle(txtMatrix12.Text, CultureInfo.InvariantCulture);
			matrix.rc13 = Convert.ToSingle(txtMatrix13.Text, CultureInfo.InvariantCulture);
			matrix.rc14 = Convert.ToSingle(txtMatrix14.Text, CultureInfo.InvariantCulture);
			matrix.rc21 = Convert.ToSingle(txtMatrix21.Text, CultureInfo.InvariantCulture);
			matrix.rc22 = Convert.ToSingle(txtMatrix22.Text, CultureInfo.InvariantCulture);
			matrix.rc23 = Convert.ToSingle(txtMatrix23.Text, CultureInfo.InvariantCulture);
			matrix.rc24 = Convert.ToSingle(txtMatrix24.Text, CultureInfo.InvariantCulture);
			matrix.rc31 = Convert.ToSingle(txtMatrix31.Text, CultureInfo.InvariantCulture);
			matrix.rc32 = Convert.ToSingle(txtMatrix32.Text, CultureInfo.InvariantCulture);
			matrix.rc33 = Convert.ToSingle(txtMatrix33.Text, CultureInfo.InvariantCulture);
			matrix.rc34 = Convert.ToSingle(txtMatrix34.Text, CultureInfo.InvariantCulture);

			return matrix;
		}

        private void loadFile(string filename)
        {

            this.filename = filename;
			this.rsltFile = new MadScience.Wrappers.RSLTFile();

            toolStripStatusLabel1.Text = this.filename;

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            this.rsltFile.Load(input);
            input.Close();

			showRouteEntries();
			showContainerEntries();
			showEffectEntries();
			showIKTargetEntries();

            saveToolStripMenuItem.Enabled = true;
			

            Console.WriteLine("Done loading");
        }

		private void saveRSLT(Stream output)
		{
			this.rsltFile.Save(output);
		}

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rsltFile = new MadScience.Wrappers.RSLTFile();

            foreach (Control control in this.Controls)
            {
                MadScience.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox3.Controls)
            {
                MadScience.Helpers.resetControl(control);
            }
            btnRouteDelete.Enabled = false;
			btnEffectDelete.Enabled = false;
			btnContainerDelete.Enabled = false;
			btnIKTargetDelete.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveRSLT(saveFile);
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
				saveRSLT(saveFile);
                saveFile.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Slot File|*.slot";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }

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

		private void showRouteEntries()
		{
			// Add routes
			lstRoutes.Items.Clear();
			btnRouteAdd.Enabled = true;
			if (this.rsltFile.rslt.Routes.Count > 0) { btnRouteDelete.Enabled = true; } else { btnRouteDelete.Enabled = false; }

			for (int i = 0; i < this.rsltFile.rslt.Routes.Count; i++)
			{
				ListViewItem item = new ListViewItem();
				item.Text = (i + 1).ToString();
				item.SubItems.Add(this.rsltFile.rslt.Routes[i].slotName.ToString("X8"));

				lstRoutes.Items.Add(item);
			}

			//tabControl1.TabPages[0].Text = "Routes (" + this.rsltFile.rslt.Routes.Count.ToString() + ")";
		}

		private void showContainerEntries()
		{
			lstContainers.Items.Clear();
			btnContainerAdd.Enabled = true;
			if (this.rsltFile.rslt.Containers.Count > 0) { btnContainerDelete.Enabled = true; } else { btnContainerDelete.Enabled = false; }

			for (int i = 0; i < this.rsltFile.rslt.Containers.Count; i++)
			{
				ListViewItem item = new ListViewItem();
				item.Text = (i + 1).ToString();
				item.SubItems.Add(this.rsltFile.rslt.Containers[i].slotName.ToString("X8"));

				lstContainers.Items.Add(item);
			}

			//tabControl1.TabPages[1].Text = "Containers (" + this.rsltFile.rslt.Containers.Count.ToString() + ")";

		}

		private void showEffectEntries()
		{
			lstEffects.Items.Clear();
			btnEffectAdd.Enabled = true;
			if (this.rsltFile.rslt.Effects.Count > 0) { btnEffectDelete.Enabled = true; } else { btnEffectDelete.Enabled = false; }

			for (int i = 0; i < this.rsltFile.rslt.Effects.Count; i++)
			{
				ListViewItem item = new ListViewItem();
				item.Text = (i + 1).ToString();
				item.SubItems.Add(this.rsltFile.rslt.Effects[i].slotName.ToString("X8"));

				lstEffects.Items.Add(item);
			}

			//tabControl1.TabPages[2].Text = "Effects (" + this.rsltFile.rslt.Effects.Count.ToString() + ")";

		}

		private void showIKTargetEntries()
		{
			lstIKTargets.Items.Clear();
			btnIKTargetAdd.Enabled = true;
			if (this.rsltFile.rslt.IKTargets.Count > 0) { btnIKTargetDelete.Enabled = true; } else { btnIKTargetDelete.Enabled = false; }

			for (int i = 0; i < this.rsltFile.rslt.IKTargets.Count; i++)
			{
				ListViewItem item = new ListViewItem();
				item.Text = (i + 1).ToString();
				item.SubItems.Add(this.rsltFile.rslt.IKTargets[i].slotName.ToString("X8"));

				lstIKTargets.Items.Add(item);
			}

			//tabControl1.TabPages[3].Text = "IKTargets (" + this.rsltFile.rslt.IKTargets.Count.ToString() + ")";

		}


		private void lstRoutes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstRoutes.SelectedItems.Count > 0)
			{
				this.clickedTab = 0;

				MadScience.Wrappers.RSLTEntry entry = this.rsltFile.rslt.Routes[lstRoutes.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);

				btnCommit.Enabled = true;
			}

		}

		private void lstContainers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstContainers.SelectedItems.Count > 0)
			{
				this.clickedTab = 1;

				MadScience.Wrappers.RSLTEntry entry = this.rsltFile.rslt.Containers[lstContainers.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = entry.flags.ToString();
				txtFlags.Enabled = true;
				showMatrix(entry.matrix);

				btnCommit.Enabled = true;
			}
		}

		private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstEffects.SelectedItems.Count > 0)
			{
				this.clickedTab = 2;

				MadScience.Wrappers.RSLTEntry entry = this.rsltFile.rslt.Effects[lstEffects.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);

				btnCommit.Enabled = true;
			}
		}

		private void lstIKTargets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstIKTargets.SelectedItems.Count > 0)
			{
				this.clickedTab = 3;

				MadScience.Wrappers.RSLTEntry entry = this.rsltFile.rslt.IKTargets[lstIKTargets.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);

				btnCommit.Enabled = true;
			}
		}

		private void btnRouteAdd_Click(object sender, EventArgs e)
		{
			MadScience.Wrappers.RSLTEntry route = new MadScience.Wrappers.RSLTEntry();
			ListViewItem item = new ListViewItem();

			item.Text = (lstRoutes.Items.Count + 1).ToString();
			item.SubItems.Add(route.slotName.ToString("X8"));

			// Setup default matrix
			route.matrix.rc11 = 1f;
			route.matrix.rc22 = 1f;
			route.matrix.rc33 = 1f;

			// Add to list
			this.rsltFile.rslt.Routes.Add(route);

			lstRoutes.Items.Add(item);


		}

		private void btnRouteDelete_Click(object sender, EventArgs e)
		{
			if (lstRoutes.SelectedItems.Count > 0)
			{
				int selected = lstRoutes.SelectedIndices[0];
				this.rsltFile.rslt.Routes.RemoveAt(selected);

				showRouteEntries();

				int toSelect = selected;
				if (selected > (lstRoutes.Items.Count - 1))
				{
					toSelect -= 1;
				}
				if (toSelect > -1)
				{
					lstRoutes.Items[toSelect].Selected = true;
					lstRoutes.Items[toSelect].EnsureVisible();
				}

			}
		}

		private void btnContainerAdd_Click(object sender, EventArgs e)
		{
			MadScience.Wrappers.RSLTEntry container = new MadScience.Wrappers.RSLTEntry();
			ListViewItem item = new ListViewItem();

			item.Text = (lstContainers.Items.Count + 1).ToString();
			item.SubItems.Add(container.slotName.ToString("X8"));

			// Setup default matrix
			container.matrix.rc11 = 1f;
			container.matrix.rc22 = 1f;
			container.matrix.rc33 = 1f;

			// Add to list
			this.rsltFile.rslt.Containers.Add(container);

			lstContainers.Items.Add(item);

		}

		private void btnContainerDelete_Click(object sender, EventArgs e)
		{
			if (lstContainers.SelectedItems.Count > 0)
			{
				int selected = lstContainers.SelectedIndices[0];
				this.rsltFile.rslt.Containers.RemoveAt(selected);

				showContainerEntries();

				int toSelect = selected;
				if (selected > (lstContainers.Items.Count - 1))
				{
					toSelect -= 1;
				}
				if (toSelect > -1)
				{
					lstContainers.Items[toSelect].Selected = true;
					lstContainers.Items[toSelect].EnsureVisible();
				}

			}
		}

		private void btnEffectAdd_Click(object sender, EventArgs e)
		{
			MadScience.Wrappers.RSLTEntry effect = new MadScience.Wrappers.RSLTEntry();
			ListViewItem item = new ListViewItem();

			item.Text = (lstEffects.Items.Count + 1).ToString();
			item.SubItems.Add(effect.slotName.ToString("X8"));

			// Setup default matrix
			effect.matrix.rc11 = 1f;
			effect.matrix.rc22 = 1f;
			effect.matrix.rc33 = 1f;

			// Add to list
			this.rsltFile.rslt.Effects.Add(effect);

			lstEffects.Items.Add(item);

		}

		private void btnEffectDelete_Click(object sender, EventArgs e)
		{
			if (lstEffects.SelectedItems.Count > 0)
			{
				int selected = lstEffects.SelectedIndices[0];
				this.rsltFile.rslt.Effects.RemoveAt(selected);

				showEffectEntries();

				int toSelect = selected;
				if (selected > (lstEffects.Items.Count - 1))
				{
					toSelect -= 1;
				}
				if (toSelect > -1)
				{
					lstEffects.Items[toSelect].Selected = true;
					lstEffects.Items[toSelect].EnsureVisible();
				}

			}
		}

		private void btnIKTargetAdd_Click(object sender, EventArgs e)
		{
			MadScience.Wrappers.RSLTEntry iktarget = new MadScience.Wrappers.RSLTEntry();
			ListViewItem item = new ListViewItem();

			item.Text = (lstIKTargets.Items.Count + 1).ToString();
			item.SubItems.Add(iktarget.slotName.ToString("X8"));

			// Setup default matrix
			iktarget.matrix.rc11 = 1f;
			iktarget.matrix.rc22 = 1f;
			iktarget.matrix.rc33 = 1f;

			// Add to list
			this.rsltFile.rslt.IKTargets.Add(iktarget);

			lstIKTargets.Items.Add(item);

		}

		private void btnIKTargetDelete_Click(object sender, EventArgs e)
		{
			if (lstIKTargets.SelectedItems.Count > 0)
			{
				int selected = lstIKTargets.SelectedIndices[0];
				this.rsltFile.rslt.IKTargets.RemoveAt(selected);

				showIKTargetEntries();

				int toSelect = selected;
				if (selected > (lstIKTargets.Items.Count - 1))
				{
					toSelect -= 1;
				}
				if (toSelect > -1)
				{
					lstIKTargets.Items[toSelect].Selected = true;
					lstIKTargets.Items[toSelect].EnsureVisible();
				}

			}

		}

		private void btnCommit_Click(object sender, EventArgs e)
		{
			// Make entry
			MadScience.Wrappers.RSLTEntry entry = new MadScience.Wrappers.RSLTEntry();

			entry.slotName = MadScience.StringHelpers.ParseHex32("0x" + txtSlotName.Text);
			entry.boneName = MadScience.StringHelpers.ParseHex32("0x" + txtBoneName.Text);

			if (!String.IsNullOrEmpty(txtFlags.Text)) entry.flags = Convert.ToUInt32(txtFlags.Text);

			entry.matrix.rc11 = Convert.ToSingle(txtMatrix11.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc12 = Convert.ToSingle(txtMatrix12.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc13 = Convert.ToSingle(txtMatrix13.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc14 = Convert.ToSingle(txtMatrix14.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc21 = Convert.ToSingle(txtMatrix21.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc22 = Convert.ToSingle(txtMatrix22.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc23 = Convert.ToSingle(txtMatrix23.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc24 = Convert.ToSingle(txtMatrix24.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc31 = Convert.ToSingle(txtMatrix31.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc32 = Convert.ToSingle(txtMatrix32.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc33 = Convert.ToSingle(txtMatrix33.Text, CultureInfo.InvariantCulture);
			entry.matrix.rc34 = Convert.ToSingle(txtMatrix34.Text, CultureInfo.InvariantCulture);

			switch (this.clickedTab)
			{
				case 0:
					if (lstRoutes.SelectedItems.Count > 0)
					{
						this.rsltFile.rslt.Routes[lstRoutes.SelectedIndices[0]] = entry;
						lstRoutes.SelectedItems[0].SubItems[1].Text = txtSlotName.Text;
					}
					break;
				case 1:
					if (lstContainers.SelectedItems.Count > 0)
					{
						this.rsltFile.rslt.Containers[lstContainers.SelectedIndices[0]] = entry;
						lstContainers.SelectedItems[0].SubItems[1].Text = txtSlotName.Text;
					}
					break;
				case 2:
					if (lstEffects.SelectedItems.Count > 0)
					{
						this.rsltFile.rslt.Effects[lstEffects.SelectedIndices[0]] = entry;
						lstEffects.SelectedItems[0].SubItems[1].Text = txtSlotName.Text;
					}
					break;
				case 3:
					if (lstIKTargets.SelectedItems.Count > 0)
					{
						this.rsltFile.rslt.IKTargets[lstIKTargets.SelectedIndices[0]] = entry;
						lstIKTargets.SelectedItems[0].SubItems[1].Text = txtSlotName.Text;
					}
					break;
			}
		}
    }
}
