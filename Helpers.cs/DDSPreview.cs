using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace MadScience
{
    public partial class DDSPreview : Form
    {
        public DDSPreview()
        {
            InitializeComponent();
        }

        DdsFileTypePlugin.DdsFile ddsFile = new DdsFileTypePlugin.DdsFile();
        bool lockImage = false;
        public void loadDDS(string filename)
        {
            this.lockImage = true;

            FileInfo f = new FileInfo(filename);

            ddsFile = new DdsFileTypePlugin.DdsFile();
            Stream input = File.Open(f.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            ddsFile.Load(input);
            pictureBox1.Image = ddsFile.Image();

            toolStripStatusLabel1.Text = f.Name;
            toolStripStatusLabel2.Text = "W: " + ddsFile.m_header.m_width.ToString();
            toolStripStatusLabel3.Text = "H: " + ddsFile.m_header.m_height.ToString();
            if (ddsFile.m_header.m_pixelFormat.m_rgbBitCount.ToString() != "0")
            {
                toolStripStatusLabel4.Text = ddsFile.m_header.m_pixelFormat.m_rgbBitCount.ToString() + "bit";
            }
            toolStripStatusLabel5.Text = ddsFile.m_header.fileFormat;

            chkShowRed.Checked = true;
            chkShowGreen.Checked = true;
            chkShowBlue.Checked = true;
            chkShowAlpha.Checked = true;

            input.Close();

            this.lockImage = false;
        }

        public void loadDDS(Stream input)
        {
            this.lockImage = true;

            ddsFile.Load(input);
            pictureBox1.Image = ddsFile.Image();

            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "W: " + ddsFile.m_header.m_width.ToString();
            toolStripStatusLabel3.Text = "H: " + ddsFile.m_header.m_height.ToString();
            if (ddsFile.m_header.m_pixelFormat.m_rgbBitCount.ToString() != "0" && ddsFile.m_header.m_pixelFormat.m_rgbBitCount.ToString() != "")
            {
                toolStripStatusLabel4.Text = ddsFile.m_header.m_pixelFormat.m_rgbBitCount.ToString() + "bit";
                toolStripStatusLabel4.Visible = true;
            }
            else
            {
                toolStripStatusLabel4.Visible = false;
            }

            toolStripStatusLabel5.Text = ddsFile.m_header.fileFormat;

            chkShowRed.Checked = true;
            chkShowGreen.Checked = true;
            chkShowBlue.Checked = true;
            chkShowAlpha.Checked = false;

            input.Close();

            this.lockImage = false;
        }

        private void chkShowRed_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void chkShowGreen_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void chkShowBlue_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void chkShowAlpha_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void showDDSChannels()
        {
            if (!this.lockImage)
            {
                pictureBox1.Image = ddsFile.Image(chkShowRed.Checked, chkShowGreen.Checked, chkShowBlue.Checked, chkShowAlpha.Checked, false);
            }
        }

        private void CopyStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Sims 3 Texture|*.dds";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != "")
            {
                FileStream saveFile = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
                CopyStream(ddsFile.DDS(), saveFile);
                saveFile.Close();
            }
        }

        private void DDSPreview_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar);
        }

        private void DDSPreview_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("KeyDown: " + e.KeyCode);
        }
    }
}
