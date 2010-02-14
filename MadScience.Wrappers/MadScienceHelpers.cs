using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MadScienceSmall
{
    public partial class Helpers
    {


        public static bool validateKey(string keyString)
        {
            return validateKey(keyString, true);
        }

        public static bool validateKey(string keyString, bool showMessage)
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(keyString))
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
            System.IO.StreamWriter sw = System.IO.File.AppendText(Path.Combine(Application.StartupPath, Application.ProductName + ".log"));
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

