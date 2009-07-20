using System;
using System.Windows.Forms;
using Microsoft.Win32;
            
namespace PatternCreator
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("You need to enter your creator name before you can click OK. :)");
                return;
            }
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName, true);
            key.SetValue("acceptLicense", "true");
            key.SetValue("creatorName", textBox2.Text);
            key.Close();

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName, true);
            key.SetValue("acceptLicense", "false");
            key.Close();

            Application.Exit();
        }
    }
}
