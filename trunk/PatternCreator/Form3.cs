using System;
using System.Windows.Forms;

namespace PatternCreator
{
    public partial class Form3 : Form
    {
        string myXML = "";

        public Form3()
        {
            InitializeComponent();
        }

        public void xmlText(string myxml)
        {
            textBox1.Text = myxml;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.myXML = textBox1.Text;
        }
    }
}
