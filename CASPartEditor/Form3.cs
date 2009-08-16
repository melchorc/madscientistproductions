using System;
using System.Windows.Forms;

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
            picRenderBackground.BackColor = MadScience.Helpers.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picRenderBackground_Click(object sender, EventArgs e)
        {
            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = picRenderBackground.BackColor;
            if (cpd.ShowDialog() == DialogResult.OK)
            {
                picRenderBackground.BackColor = cpd.Color;
                MadScience.Helpers.saveRegistryValue("renderBackgroundColour", MadScience.Helpers.convertColour(picRenderBackground.BackColor));
            }
        }
    }
}
