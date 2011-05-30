using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Sims3Dashboard
{
    public partial class CacheInfo : Form
    {
        public CacheInfo()
        {
            InitializeComponent();
        }

        private void CacheInfo_Load(object sender, EventArgs e)
        {
            getCacheDetails();
        }

        MadScience.ListViewSorter Sorter = new MadScience.ListViewSorter();

        private void getCacheDetails()
        {
            string TS3MyDocs = MadScience.Helpers.getRegistryValue("cacheFolders");
            if (String.IsNullOrEmpty(TS3MyDocs))
            {

                string myDocs = MadScience.Helpers.findMyDocs();
                if (String.IsNullOrEmpty(myDocs)) return;

                TS3MyDocs = Path.Combine(Path.Combine(myDocs, "Electronic Arts"), MadScience.Helpers.gamesInstalled.baseName);
            }
            listView2.Items.Clear();

            showCacheInfo(Path.Combine(TS3MyDocs, "CASPartCache.package"), "CASPartCache");
            showCacheInfo(Path.Combine(TS3MyDocs, "compositorCache.package"), "compositorCache");
            showCacheInfo(Path.Combine(TS3MyDocs, "scriptCache.package"), "scriptCache");
            showCacheInfo(Path.Combine(TS3MyDocs, "simCompositorCache.package"), "simCompositorCache");

            DirectoryInfo dir;
            FileInfo[] myFiles;

            if (Directory.Exists(Path.Combine(TS3MyDocs, "WorldCaches")))
            {
                dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "WorldCaches"));
                myFiles = dir.GetFiles("*.package");
                foreach (FileInfo f in myFiles)
                {
                    showCacheInfo(f.FullName, "WorldCaches\\" + f.Name);
                }
            }

            if (Directory.Exists(Path.Combine(TS3MyDocs, "Thumbnails")))
            {
                dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "Thumbnails"));
                myFiles = dir.GetFiles("*.package");
                foreach (FileInfo f in myFiles)
                {
                    showCacheInfo(f.FullName, "Thumbnails\\" + f.Name);
                }
            }

            //showCacheInfo("", "");

        }

        private void showCacheInfo(string cachePath, string cacheName)
        {
            if (!String.IsNullOrEmpty(cachePath) && !File.Exists(cachePath)) return;


            // CAS Part Cache
            ListViewItem item = new ListViewItem();

            item.Text = cacheName;
            item.SubItems.Add(cachePath);

            if (!String.IsNullOrEmpty(cachePath))
            {
                if (!File.Exists(cachePath))
                {
                    item.SubItems.Add("0");
                    item.SubItems.Add("0");

                }
                else
                {

                    FileInfo f = new FileInfo(cachePath);
                    if (f.Length > 0)
                    {
                        item.SubItems.Add(((f.Length / 1024) + 1).ToString());
                    }
                    else
                    {
                        item.SubItems.Add("0");
                    }

                    Stream casPartCache = File.OpenRead(cachePath);
                    MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(casPartCache, true);
                    item.SubItems.Add(db.dbpf.Entries.Count.ToString());
                    casPartCache.Close();
                }
            }
            else
            {
                item.SubItems.Add("");
                item.SubItems.Add("");
            }

            listView2.Items.Add(item);
        }

        private void clearCache(string cachePath, string cacheName)
        {
            if (String.IsNullOrEmpty(cacheName))
            {
                // Clear ALL caches
                if (MessageBox.Show("You are about to clear all caches.  Are you sure?", "Clear All Caches", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    string myDocs = MadScience.Helpers.findMyDocs();
                    if (String.IsNullOrEmpty(myDocs)) return;

                    string TS3MyDocs = Path.Combine(Path.Combine(myDocs, "Electronic Arts"), "The Sims 3");

                    DirectoryInfo dir;
                    FileInfo[] myFiles;

                    if (Directory.Exists(Path.Combine(TS3MyDocs, "WorldCaches")))
                    {
                        dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "WorldCaches"));
                        myFiles = dir.GetFiles("*.package");
                        foreach (FileInfo f in myFiles)
                        {
                            File.Delete(f.FullName);
                        }
                    }

                    if (Directory.Exists(Path.Combine(TS3MyDocs, "Thumbnails")))
                    {
                        dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "Thumbnails"));
                        myFiles = dir.GetFiles("*.package");
                        foreach (FileInfo f in myFiles)
                        {
                            File.Delete(f.FullName);
                        }
                    }

                    try
                    {
                        File.Delete(Path.Combine(TS3MyDocs, "CASPartCache.package"));
                        File.Delete(Path.Combine(TS3MyDocs, "compositorCache.package"));
                        File.Delete(Path.Combine(TS3MyDocs, "scriptCache.package"));
                        File.Delete(Path.Combine(TS3MyDocs, "simCompositorCache.package"));
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            else
            {
                if (MessageBox.Show("You are about to delete the " + cacheName + " cache.  Are you sure?", "Clear " + cacheName, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    File.Delete(cachePath);
                }
            }


        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter.Sort(listView2, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            clearCache("", "");
            getCacheDetails();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if (listView2.Items[i].Checked) clearCache(listView2.Items[i].SubItems[1].Text, listView2.Items[i].Text);
            }
            getCacheDetails();
        }


        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            int numChecked = listView2.CheckedItems.Count;
            if (numChecked > 0)
            {
                button6.Enabled = true;
            }
            else
            {
                button6.Enabled = false;
            }
        }

    }
}
