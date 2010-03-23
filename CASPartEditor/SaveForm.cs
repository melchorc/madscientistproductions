using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MadScience;
using System.Globalization;

namespace CASPartEditor
{
    public partial class SaveForm : Form
    {
        ulong _customInstance = 0;

        string newString;
        string oldString;
        ulong oldInstance;

        public SaveForm(string oldID, string newID, ulong instance)
        {
            InitializeComponent();
            oldString = oldID;
            newString = newID;
            oldInstance = instance;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = StringHelpers.HashFNV64(textBox1.Text).ToString("X",CultureInfo.InvariantCulture);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = ulong.TryParse(textBox2.Text, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out _customInstance);
        }

        public ulong Instance
        {
            get
            {
                return _customInstance;
            }
        }

        private void rDefault_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = oldString;
            textBox2.Text = oldInstance.ToString("X", CultureInfo.InvariantCulture);
        }

        private void rNew_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = newString;
        }

    }
}
