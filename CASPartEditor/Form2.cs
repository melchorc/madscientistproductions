using System;
using System.Windows.Forms;
using System.IO;
using MadScience.Wrappers;

namespace CASPartEditor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            searchForKey(txtFindKey.Text);
        }

        private bool validateKey(string keyString)
        {
            bool retVal = true;

            if (keyString.Trim() == "") retVal = false;
            if (!keyString.StartsWith("key:")) retVal = false;
            if (!keyString.Contains(":")) retVal = false;
            string[] temp = keyString.Split(":".ToCharArray());
            if (temp.Length < 4) retVal = false;

            if (!retVal) { MessageBox.Show("Key is not in the correct format!"); return false; }
            else { return true; }
        }

        private bool searchForKey(string keyString)
        {

            // Validate keystring
            if (validateKey(keyString) == false)
            {
                return false;
            }


            string sims3root = MadScience.Helpers.findSims3Root();
            if (String.IsNullOrEmpty(sims3root))
            {
                return false;
            }

            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Searching for image... please wait";

            statusStrip1.Refresh();

            Stream input = File.OpenRead(sims3root + "\\GameData\\Shared\\Packages\\FullBuild2.package");

            Database db = new Database(input, true);

            input.Seek(0, SeekOrigin.Begin);

            DatabasePackedFile dbpf = new DatabasePackedFile();
            try
            {
                dbpf.Read(input);
            }
            catch (MadScience.Exceptions.NotAPackageException)
            {
                MessageBox.Show("bad file: {0}", sims3root + "\\GameData\\Shared\\Packages\\FullBuild2.package");
                input.Close();
                return false;
            }

            // Split the input key
            keyString = keyString.Replace("key:", "");
            string[] temp = keyString.Split(":".ToCharArray());

            uint typeID = MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
            uint groupID = MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
            ulong instanceID = MadScience.StringHelpers.ParseHex64("0x" + temp[2]);

            toolStripProgressBar1.Maximum = dbpf.Entries.Count;

            bool foundMatch = false;

            for (int i = 0; i < dbpf.Entries.Count; i++)
            {
                DatabasePackedFile.Entry entry = dbpf.Entries[i];
                toolStripProgressBar1.Value++;

                if (entry.Key.typeId == typeID && entry.Key.groupId == groupID && entry.Key.instanceId == instanceID)
                {
                    foundMatch = true;
                    MadScience.DDSPreview ddsP = new MadScience.DDSPreview();
                    ddsP.loadDDS(db.GetResourceStream(entry.Key));
                    ddsP.ShowDialog();
                    break;
                }

            }

            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "";
            statusStrip1.Refresh();

            input.Close();

            return foundMatch;
        }
    }
}
