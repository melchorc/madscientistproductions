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
            this.rsltFile.rslt = new MadScience.Wrappers.RSLT();

            toolStripStatusLabel1.Text = this.filename;

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            this.rsltFile.Load(input);
            input.Close();

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
            btnRouteDelete.Enabled = false;
			btnEffectDelete.Enabled = false;
			btnContainerDelete.Enabled = false;
			btnIKTargetDelete.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            //saveVPXY(saveFile);
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
                //saveVPXY(saveFile);
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

		private void lstRoutes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstRoutes.SelectedItems.Count > 0)
			{
				this.clickedTab = 0;

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
				this.clickedTab = 1;

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
				this.clickedTab = 2;

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
				this.clickedTab = 3;

				MadScience.Wrappers.RSLTIKTarget entry = this.rsltFile.rslt.IKTargets[lstIKTargets.SelectedIndices[0]];
				txtBoneName.Text = entry.boneName.ToString("X8");
				txtSlotName.Text = entry.slotName.ToString("X8");
				txtFlags.Text = "";
				txtFlags.Enabled = false;
				showMatrix(entry.matrix);
			}
		}

		private void btnRouteAdd_Click(object sender, EventArgs e)
		{
			MadScience.Wrappers.RSLTRoute route = new MadScience.Wrappers.RSLTRoute();
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

		}

		private void btnContainerAdd_Click(object sender, EventArgs e)
		{

		}

		private void btnContainerDelete_Click(object sender, EventArgs e)
		{

		}

		private void btnEffectAdd_Click(object sender, EventArgs e)
		{

		}

		private void btnEffectDelete_Click(object sender, EventArgs e)
		{

		}

		private void btnIKTargetAdd_Click(object sender, EventArgs e)
		{

		}

		private void btnIKTargetDelete_Click(object sender, EventArgs e)
		{

		}
    }
}
