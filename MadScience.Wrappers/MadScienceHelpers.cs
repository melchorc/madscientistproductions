using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MadScienceSmall
{
    public partial class Helpers
    {
        public static void resetControl(System.Windows.Forms.Control control)
        {
            if (control is System.Windows.Forms.TextBox)
            {
                control.Text = "";
            }
            if (control is System.Windows.Forms.ComboBox)
            {
                System.Windows.Forms.ComboBox cmb = (System.Windows.Forms.ComboBox)control;
                if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
            }
            if (control is System.Windows.Forms.CheckBox)
            {
                System.Windows.Forms.CheckBox chk = (System.Windows.Forms.CheckBox)control;
                chk.Checked = false;
            }
            if (control is System.Windows.Forms.CheckedListBox)
            {
                System.Windows.Forms.CheckedListBox chkList = (System.Windows.Forms.CheckedListBox)control;
                for (int i = 0; i < chkList.Items.Count; i++)
                {
                    chkList.SetItemChecked(i, false);
                }
            }
            if (control is System.Windows.Forms.ListView)
            {
                System.Windows.Forms.ListView lstView = (System.Windows.Forms.ListView)control;
                lstView.Items.Clear();
            }

        }

        public static bool validateKey(string keyString)
        {
            return validateKey(keyString, true);
        }

        public static bool validateKey(string keyString, bool showMessage)
        {
            bool retVal = true;

            if (keyString.Trim() == "")
            {
                return false;
            }
            if (!keyString.StartsWith("key:")) retVal = false;
            if (!keyString.Contains(":")) retVal = false;
            string[] temp = keyString.Split(":".ToCharArray());
            if (temp.Length < 4) retVal = false;

            if (!retVal)
            {
                if (showMessage) { 
                    throw new Exception("Key is not in the correct format!"); 
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void logMessageToFile(string message)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(Application.StartupPath + "\\" + Application.ProductName + ".log");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, message);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}

