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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
