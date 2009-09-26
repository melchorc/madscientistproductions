using System;
using System.Windows.Forms;
using System.IO;

namespace CASPartEditor
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.setSims3Root();
            lblSims3RootFolder.Text = MadScience.Helpers.findSims3Root();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            lblSims3RootFolder.Text = MadScience.Helpers.findSims3Root();
            picRenderBackground.BackColor = MadScience.Colours.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));
            lstGlobalPackages.DataSource = MadScience.Helpers.globalPackageFiles;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] arr = new string[MadScience.Helpers.globalPackageFiles.Count];
            MadScience.Helpers.globalPackageFiles.CopyTo(arr, 0);
            string fileString = String.Join("?", arr);
            MadScience.Helpers.saveRegistryValue("globalPackageFiles", fileString);
            this.Close();
        }

        private void picRenderBackground_Click(object sender, EventArgs e)
        {
            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = picRenderBackground.BackColor;
            if (cpd.ShowDialog() == DialogResult.OK)
            {
                picRenderBackground.BackColor = cpd.Color;
                MadScience.Helpers.saveRegistryValue("renderBackgroundColour", MadScience.Colours.convertColour(picRenderBackground.BackColor));
            }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.FileName = "";
            fd.Filter = "Sims 3 Package|*.package|All Files|*.*";
            fd.InitialDirectory = Path.Combine(lblSims3RootFolder.Text,@"Mods\Packages\");
            fd.Multiselect = true;
            fd.ShowDialog();
            foreach (string file in fd.FileNames)
            {
                if(!MadScience.Helpers.globalPackageFiles.Contains(file))
                    MadScience.Helpers.globalPackageFiles.Add(file);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string file;
            if (lstGlobalPackages.SelectedItem != null)
            {
                file = (string)lstGlobalPackages.SelectedItem;
                MadScience.Helpers.globalPackageFiles.Remove(file);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.globalPackageFiles.Clear();
        }
    }
}
