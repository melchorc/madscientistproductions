using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
//using System.Linq;
//using Gibbed.Sims3.FileFormats;

using MadScience.Wrappers;

using Microsoft.Win32;

namespace PatternCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MadScience.Helpers.logMessageToFile("Starting " + Application.ProductName + " " + Application.ProductVersion);

            MadScience.Helpers.checkAndShowLicense(Application.ProductName);

            this.Text = this.Text + " v" + Application.ProductVersion.ToString();

            MadScience.Helpers.logMessageToFile("Finished initialising");
        }

        //public metaEntry.typesToMeta lookupList;

        //private ArrayList indexEntries = new ArrayList();

        private bool lockImage = false;
        private bool isPreviewImage = false;

        /*
        private void lookupTypes()
        {
            TextReader r = new StreamReader(Application.StartupPath + "\\metaTypes.xml");
            XmlSerializer s = new XmlSerializer(typeof(metaEntry.typesToMeta));
            this.lookupList = (metaEntry.typesToMeta)s.Deserialize(r);
            r.Close();

        }
        */

        private void lblBackgroundColour_Click(object sender, EventArgs e)
        {
            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = lblBackgroundColour.BackColor;
            cpd.ShowDialog();

            lblBackgroundColour.BackColor = cpd.Color;

            isPreviewImage = true;
            showDDSChannels();
        }

        private void lblPalette1_Click(object sender, EventArgs e)
        {

            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = lblPalette1.BackColor;
            cpd.ShowDialog();

            lblPalette1.BackColor = cpd.Color;

            isPreviewImage = true;
            showDDSChannels();

        }

        private void lblPalette2_Click(object sender, EventArgs e)
        {
            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = lblPalette2.BackColor;
            cpd.ShowDialog();

            lblPalette2.BackColor = cpd.Color;

            isPreviewImage = true;
            showDDSChannels();

        }

        private void lblPalette3_Click(object sender, EventArgs e)
        {
            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = lblPalette3.BackColor;
            cpd.ShowDialog();

            lblPalette3.BackColor = cpd.Color;

            isPreviewImage = true;
            showDDSChannels();

        }

        private void lblPalette4_Click(object sender, EventArgs e)
        {
            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = lblPalette4.BackColor;
            cpd.ShowDialog();

            lblPalette4.BackColor = cpd.Color;

            isPreviewImage = true;
            showDDSChannels();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "DDS files|*.dds";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                txtSourceDDS.Text = openFileDialog1.FileName;

                Stream input = File.Open(txtSourceDDS.Text, FileMode.Open);

                DdsFileTypePlugin.DdsFile ddsFile = new DdsFileTypePlugin.DdsFile();
                ddsFile.Load(input);
                input.Close();

                pictureBox1.Image = ddsFile.Image();

                this.lockImage = true;
                chkShowRed.Enabled = chkShowRed.Checked = true;
                chkShowGreen.Enabled = chkShowGreen.Checked = true;
                chkShowBlue.Enabled = chkShowBlue.Checked = true;
                chkShowAlpha.Enabled = true;
                chkShowAlpha.Checked = false;
                button3.Enabled = true;
                button2.Enabled = false;

                this.lockImage = false;

                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;

                chkAllowDecal.Enabled = true;
                chkUseDefaultSpecular.Enabled = true;
                button4.Enabled = true;

            }
        }

        private void toggleColourPalettes(bool bg, bool p1, bool p2, bool p3, bool p4)
        {

            this.lockImage = true;

            // Turn off all DDS channels
            chkShowAlpha.Checked = false;
            chkShowBlue.Checked = false;
            chkShowGreen.Checked = false;
            chkShowRed.Checked = false;

            label15.Enabled = bg;
            lblBackgroundColour.Enabled = bg;
            button2.Enabled = bg;

            label16.Enabled = p1;
            lblPalette1.Enabled = p1;
            chkPalette1Blend.Enabled = p1;
            chkShowRed.Enabled = p1;
            chkShowRed.Checked = p1;

            label17.Enabled = p2;
            lblPalette2.Enabled = p2;
            chkPalette2Blend.Enabled = p2;
            chkShowGreen.Enabled = p2;
            chkShowGreen.Checked = p2;

            label18.Enabled = p3;
            lblPalette3.Enabled = p3;
            chkPalette3Blend.Enabled = p3;
            chkShowBlue.Enabled = p3;
            chkShowBlue.Checked = p3;

            label19.Enabled = p4;
            lblPalette4.Enabled = p4;
            chkPalette4Blend.Enabled = p4;
            chkShowAlpha.Enabled = p4;
            chkShowAlpha.Checked = p4;

            this.lockImage = false;

            showDDSChannels();

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                toggleColourPalettes(true, true, false, false, false);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                toggleColourPalettes(true, true, true, false, false);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                toggleColourPalettes(true, true, true, true, false);
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                toggleColourPalettes(true, true, true, true, true);
            }
        }

        /*
        private static void colourFill(Image mask, Image dst, Color c2, uint channel, bool blend)
        {
            FastPixel pixel = new FastPixel(mask as Bitmap);
            FastPixel pixel2 = new FastPixel(dst as Bitmap);
            pixel.Lock();
            pixel2.Lock();
            for (int i = 0; i < pixel.Width; i++)
            {
                for (int j = 0; j < pixel.Height; j++)
                {
                    Color color = pixel.GetPixel(i, j);
                    Color color2 = pixel2.GetPixel(i, j);
                    float num3 = 0f;
                    if (channel == uint.MaxValue)
                    {
                        num3 = 1f;
                    }
                    else if ((channel & 0xff000000) == 0xff000000)
                    {
                        num3 = 0.003921569f * color.R;
                    }
                    else if ((channel & 0xff0000) == 0xff0000)
                    {
                        num3 = 0.003921569f * color.G;
                    }
                    else if ((channel & 0xff00) == 0xff00)
                    {
                        num3 = 0.003921569f * color.B;
                    }
                    else if ((channel & 0xff) == 0xff)
                    {
                        num3 = 0.003921569f * color.A;
                    }
                    Color black = Color.Black;
                    if (!blend)
                    {
                        black = (num3 == 0f) ? color2 : Color.FromArgb(0xff, c2);
                    }
                    else
                    {
                        black = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c2.R * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c2.G * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c2.B * (0.003921569f * c2.A))) * num3)))));
                    }
                    pixel2.SetPixel(i, j, black);
                }
            }
            pixel.Unlock(false);
            pixel2.Unlock(true);
        }
        */
        /*
        private Image imagePreview(Image sourceImage)
        {
            Image destImage = new Bitmap(256, 256);
            Graphics.FromImage(destImage);

            Color white = Color.White;
            uint maxValue = 0;

            colourFill(sourceImage, destImage, lblBackgroundColour.BackColor, 0, false);

            if (chkShowRed.Checked)
            {
                maxValue = ((uint) ((((((uint) Convert.ToSingle(1.00)) * 0xff) << 0x18) + ((((uint) Convert.ToSingle(0.00) * 0xff) << 0x10)) + ((((uint) Convert.ToSingle(0.00)) * 0xff) << 8))) + (((uint) Convert.ToSingle(0.00)) * 0xff));
                white = Color.FromArgb(lblPalette1.BackColor.A, lblPalette1.BackColor.R, lblPalette1.BackColor.G, lblPalette1.BackColor.B);
                colourFill(sourceImage, destImage, white, maxValue, chkPalette1Blend.Checked);
            }
            if (chkShowGreen.Checked)
            {
                maxValue = ((0 * 0xff) << 0x18) + ((1 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((0 * 0xff));
                white = Color.FromArgb(lblPalette2.BackColor.A, lblPalette2.BackColor.R, lblPalette2.BackColor.G, lblPalette2.BackColor.B);
                colourFill(sourceImage, destImage, white, maxValue, chkPalette2Blend.Checked);
            }
            if (chkShowBlue.Checked)
            {
                maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((1 * 0xff) << 8) + ((0 * 0xff));
                white = Color.FromArgb(lblPalette3.BackColor.A, lblPalette3.BackColor.R, lblPalette3.BackColor.G, lblPalette3.BackColor.B);
                colourFill(sourceImage, destImage, white, maxValue, chkPalette3Blend.Checked);
            }
            if (chkShowAlpha.Checked)
            {
                maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((1 * 0xff));
                white = Color.FromArgb(lblPalette4.BackColor.A, lblPalette4.BackColor.R, lblPalette4.BackColor.G, lblPalette4.BackColor.B);
                colourFill(sourceImage, destImage, white, maxValue, chkPalette4Blend.Checked);
            }

            return destImage;
        }
        */

        private void showDDSChannels()
        {
            if (!this.lockImage && txtSourceDDS.Text.Trim() != "")
            {

                Stream input = File.Open(txtSourceDDS.Text, FileMode.Open);

                DdsFileTypePlugin.DdsFile ddsFile = new DdsFileTypePlugin.DdsFile();
                ddsFile.Load(input);

                if (isPreviewImage)
                {
                    //pictureBox1.Image = imagePreview(ddsFile.Image(chkShowRed.Checked, chkShowGreen.Checked, chkShowBlue.Checked, chkShowAlpha.Checked, false));
                    Color colorbg = Color.Empty;
                    Color color1 = Color.Empty;
                    Color color2 = Color.Empty;
                    Color color3 = Color.Empty;
                    Color color4 = Color.Empty;
                    if (chkShowRed.Checked) color1 = lblPalette1.BackColor;
                    if (chkShowGreen.Checked) color2 = lblPalette2.BackColor;
                    if (chkShowBlue.Checked) color3 = lblPalette3.BackColor;
                    if (chkShowAlpha.Checked) color4 = lblPalette4.BackColor;

                    pictureBox1.Image = ddsFile.Image(colorbg, color1, color2, color3, color4);
                    //pictureBox1.Image = ddsFile.PreviewImage(chkShowRed.Checked, lblPalette1.BackColor, chkShowGreen.Checked, lblPalette2.BackColor, chkShowBlue.Checked, lblPalette3.BackColor, chkShowAlpha.Checked, lblPalette4.BackColor);

                }
                else
                {
                    pictureBox1.Image = ddsFile.Image(chkShowRed.Checked, chkShowGreen.Checked, chkShowBlue.Checked, chkShowAlpha.Checked, true);
                }

                input.Close();

            }
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

        private void newPatternToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check for registry key
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName, true);

            if (key.GetValue("creatorName") != null)
            {
                txtCreatorName.Text = key.GetValue("creatorName").ToString();
            }
            if (key.GetValue("creatorHomepage") != null)
            {
                txtCreatorHomepage.Text = key.GetValue("creatorHomepage").ToString();
            }

            key.Close();

            clearEverything();
            toggleEverything(true);

        }

        private void toggleEverything(bool toggle)
        {
            txtCreatorName.Enabled = toggle;
            txtCreatorHomepage.Enabled = toggle;
            dateTimePicker1.Enabled = toggle;

            txtPatternTitle.Enabled = toggle;
            txtPatternDesc.Enabled = toggle;
            cmbCategory.Enabled = toggle;
            cmbSurfaceMat.Enabled = toggle;

            txtSourceDDS.Enabled = toggle;
            btnBrowseDDS.Enabled = toggle;

            saveAsToolStripMenuItem.Enabled = toggle;

        }
        private void clearEverything()
        {

            txtPatternTitle.Text = "";
            txtPatternDesc.Text = "";
            cmbCategory.Text = "";
            cmbSurfaceMat.Text = "";

            txtSourceDDS.Text = "";
            pictureBox1.Image = null;

            radioButton1.Checked = radioButton1.Enabled = false;
            radioButton2.Checked = radioButton2.Enabled = false;
            radioButton3.Checked = radioButton3.Enabled = false;
            radioButton4.Checked = radioButton4.Enabled = false;

            chkAllowDecal.Enabled = false;
            chkUseDefaultSpecular.Enabled = false;
            button4.Enabled = false;

            this.lockImage = true;
            toggleColourPalettes(false, false, false, false, false);
            this.lockImage = false;

            button3.Enabled = false;
            button2.Enabled = false;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //        private string sanitiseString(string input)
        //{
        //var s = from ch in input where char.IsLetterOrDigit(ch) select ch;
        //return UnicodeEncoding.ASCII.GetString(UnicodeEncoding.ASCII.GetBytes(s.ToArray()));
        //}

        private void button3_Click_1(object sender, EventArgs e)
        {

            MessageBox.Show(MadScience.Helpers.sanitiseString(dateTimePicker1.Value.ToString()));
        }

        private bool validateFields()
        {
            if (MadScience.Helpers.sanitiseString(txtCreatorName.Text.Trim()) == "") { return false; }
            if (txtCreatorHomepage.Text.Trim() == "") { return false; }
            if (MadScience.Helpers.sanitiseString(txtPatternTitle.Text.Trim()) == "") { return false; }
            if (MadScience.Helpers.sanitiseString(txtPatternDesc.Text.Trim()) == "") { return false; }
            if (cmbCategory.Text.Trim() == "") { return false; }
            if (cmbSurfaceMat.Text.Trim() == "") { return false; }
            if (txtSourceDDS.Text.Trim() == "") { return false; }
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false && radioButton4.Checked == false) { return false; }

            return true;

        }

        private int numChannels()
        {
            if (radioButton1.Checked) { return 1; }
            if (radioButton2.Checked) { return 2; }
            if (radioButton3.Checked) { return 3; }
            if (radioButton4.Checked) { return 4; }

            return 0;
        }

        /*
        private string convertColour(Color color)
        {
            // Converts a colour dialog box colour into a 0 to 1 style colour
            float newR = color.R / 255f;
            float newG = color.G / 255f;
            float newB = color.B / 255f;

            // Alpha is always 1
            int alpha = 1;

            //CultureInfo englishCulture = new CultureInfo("");
            string red = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newR);
            string green = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newG);
            string blue = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newB);

            return red + "," + green + "," + blue + "," + alpha.ToString();

        }
        */

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (validateFields() == false)
            {
                MessageBox.Show("You have a missing or invalid field!  Please double check everything and try again.");
                return;
            }

            // Save folders in registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName, true);
            key.SetValue("creatorName", txtCreatorName.Text);
            key.SetValue("creatorHomepage", txtCreatorHomepage.Text);
            key.Close();

            //saveFileDialog1.Filter = "Sims 3 Pack|*.Sims3Pack";
            saveFileDialog1.Filter = "Sims 3 Package|*.package|Sims 3 Pack|*.Sims3Pack";
            saveFileDialog1.Title = "Save Pattern As....";
            saveFileDialog1.FileName = "";
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Trim() != "")
            {

                // Check for .package on the end - if it exists, strip it
                string savePath = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.LastIndexOf("\\") + 1);

                string saveName = "";

                FileInfo f = new FileInfo(saveFileDialog1.FileName);
                saveName = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf("\\") + 1).Replace(f.Extension, "");

                toggleEverything(false);

                label1.Visible = true;
                label1.Refresh();
                this.Refresh();

                // Construct name for XML
                string fName = MadScience.Helpers.sanitiseString(saveName);

                string pName = "DPP_" + MadScience.Helpers.sanitiseString(txtCreatorName.Text.Trim()) + "_" + fName + "_" + MadScience.Helpers.sanitiseString(dateTimePicker1.Value.ToString());

                ulong instanceId = MadScience.StringHelpers.HashFNV64(pName);
                ulong specInstanceId = MadScience.StringHelpers.HashFNV64(pName + "_spec");

                //uint groupId = 33554432;
                uint groupId = 0;

                int numChans = numChannels();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<complate category=\"" + cmbCategory.Text + "\" type=\"fabric\" name=\"" + pName + "\" typeConverter=\"Medator.ComplateConvertor,Medator\" surfaceMaterial=\"" + cmbSurfaceMat.Text + "\" reskey=\"key:0333406c:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "\">");
                sb.AppendLine("  <GUID>" + System.Guid.NewGuid().ToString() + "</GUID>");
                sb.AppendLine("  <variables>");

                // Convert color boxes into 0 to 1 style colours
                if (numChans >= 1) { sb.AppendLine("    <param type=\"color\" name=\"Color 0\" default=\"" + MadScience.Helpers.convertColour(lblPalette1.BackColor) + "\" uiEditor=\"Medator.Color4TypeEditor, Medator\" uiCategory=\"Colors\" />"); }
                if (numChans >= 2) { sb.AppendLine("    <param type=\"color\" name=\"Color 1\" default=\"" + MadScience.Helpers.convertColour(lblPalette2.BackColor) + "\" uiEditor=\"Medator.Color4TypeEditor, Medator\" uiCategory=\"Colors\" />"); }
                if (numChans >= 3) { sb.AppendLine("    <param type=\"color\" name=\"Color 2\" default=\"" + MadScience.Helpers.convertColour(lblPalette3.BackColor) + "\" uiEditor=\"Medator.Color4TypeEditor, Medator\" uiCategory=\"Colors\" />"); }
                if (numChans == 4) { sb.AppendLine("    <param type=\"color\" name=\"Color 3\" default=\"" + MadScience.Helpers.convertColour(lblPalette4.BackColor) + "\" uiEditor=\"Medator.Color4TypeEditor, Medator\" uiCategory=\"Colors\" />"); }

                sb.AppendLine("    <param type=\"texture\" name=\"rgbmask\" uiVisible=\"false\" default=\"key:00b2d882:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "\" />");
                if (chkUseDefaultSpecular.Checked)
                {
                    sb.AppendLine("    <param type=\"texture\" name=\"specmap\" uiVisible=\"false\" default=\"($assetRoot)\\InGame\\GlobalTextures\\Objects\\defaultBlackSpecular.tga\" />");
                }
                else
                {
                    sb.AppendLine("    <param type=\"texture\" name=\"specmap\" uiVisible=\"false\" default=\"key:00b2d882:" + groupId.ToString("X8").ToLower() + ":" + specInstanceId.ToString("X16").ToLower() + "\" />");
                }
                sb.AppendLine("  </variables>");

                sb.AppendLine("  <texturePart>");
                sb.AppendLine("    <destination>");
                if (!chkAllowDecal.Checked)
                {
                    sb.AppendLine("      <step type=\"ColorFill\" color=\"" + MadScience.Helpers.convertColour(lblBackgroundColour.BackColor) + "\" />");
                }
                // 2 lines per channel
                if (numChans >= 1)
                {
                    sb.AppendLine("      <step type=\"ChannelSelect\" texture=\"($rgbmask)\" select=\"1.0000,0.0000,0.0000,0.0000\" colorWrite=\"Alpha\" />");
                    sb.AppendLine("      <step type=\"ColorFill\" color=\"($Color 0)\" enableBlending=\"" + chkPalette1Blend.Checked.ToString().ToLower() + "\" srcBlend=\"DestAlpha\" dstBlend=\"InvDestAlpha\" sourceRect=\"0,0,1,1\" />");
                }
                if (numChans >= 2)
                {
                    sb.AppendLine("      <step type=\"ChannelSelect\" texture=\"($rgbmask)\" select=\"0.0000,1.0000,0.0000,0.0000\" colorWrite=\"Alpha\" />");
                    sb.AppendLine("      <step type=\"ColorFill\" color=\"($Color 1)\" enableBlending=\"" + chkPalette1Blend.Checked.ToString().ToLower() + "\" srcBlend=\"DestAlpha\" dstBlend=\"InvDestAlpha\" sourceRect=\"0,0,1,1\" />");
                }
                if (numChans >= 3)
                {
                    sb.AppendLine("      <step type=\"ChannelSelect\" texture=\"($rgbmask)\" select=\"0.0000,0.0000,1.0000,0.0000\" colorWrite=\"Alpha\" />");
                    sb.AppendLine("      <step type=\"ColorFill\" color=\"($Color 2)\" enableBlending=\"" + chkPalette1Blend.Checked.ToString().ToLower() + "\" srcBlend=\"DestAlpha\" dstBlend=\"InvDestAlpha\" sourceRect=\"0,0,1,1\" />");
                }
                if (numChans >= 4)
                {
                    sb.AppendLine("      <step type=\"ChannelSelect\" texture=\"($rgbmask)\" select=\"0.0000,0.0000,0.0000,1.0000\" colorWrite=\"Alpha\" />");
                    sb.AppendLine("      <step type=\"ColorFill\" color=\"($Color 3)\" enableBlending=\"" + chkPalette1Blend.Checked.ToString().ToLower() + "\" srcBlend=\"DestAlpha\" dstBlend=\"InvDestAlpha\" sourceRect=\"0,0,1,1\" />");
                }

                sb.AppendLine("      <step type=\"ChannelSelect\" texture=\"($specmap)\" select=\"1.0,0.0,0.0,0.0\" colorWrite=\"Alpha\" />");
                sb.AppendLine("    </destination>");
                sb.AppendLine("  </texturePart>");

                sb.AppendLine("  <localizedName key=\"Name:" + pName + "\" />");
                sb.AppendLine("  <contentType type=\"kCustomContent\" />");
                sb.AppendLine("  <localizedDescription key=\"Name:" + pName + "\" />");
                sb.AppendLine("</complate>");

                string xml033406c = sb.ToString();

                sb = null;
                sb = new StringBuilder();

                // Now for the d4d9fbe5 patternlist file
                sb.AppendLine("<patternlist>");
                sb.AppendLine("  <category name=\"" + cmbCategory.Text + "\">");
                sb.AppendLine("    <pattern reskey=\"key:0333406c:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "\" name=\"" + pName + "\" complexity=\"\" />");
                sb.AppendLine("  </category>");
                sb.AppendLine("</patternlist>");

                string xmld4d9fbe = sb.ToString();

                sb = null;
                sb = new StringBuilder();

                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<manifest version=\"3\" packagetype=\"pattern\" packagesubtype=\"0x00000000\" paidcontent=\"false\">");
                sb.Append("<gameversion>0.0.0.11195</gameversion>");
                sb.Append("<packagedate>" + dateTimePicker1.Value.ToString() + "</packagedate>");
                sb.Append("<packageid>" + pName + "</packageid>");
                sb.Append("<packagetitle><![CDATA[" + txtPatternTitle.Text + "]]></packagetitle>");
                sb.Append("<packagedesc><![CDATA[" + txtPatternDesc.Text + "]]></packagedesc>");
                sb.Append("<assetversion>0</assetversion>");
                sb.Append("<mingamever>1.0.0.0</mingamever>");
                sb.Append("<thumbnail>2e75c765:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "</thumbnail>");
                sb.Append("<localizednames>");
                sb.Append("<localizedname language=\"en-US\"><![CDATA[" + txtPatternTitle.Text + "]]></localizedname>");
                sb.Append("</localizednames>");
                sb.Append("<localizeddescriptions>");
                sb.Append("<localizeddescription language=\"en-US\"><![CDATA[" + txtPatternDesc.Text + "]]></localizeddescription>");
                sb.Append("</localizeddescriptions>");
                sb.Append("<handler />");
                sb.Append("<dependencylist>");
                sb.Append("<packageid>0x050cffe800000000050cffe800000000</packageid>");
                sb.Append("<packageid>" + pName + "</packageid>");
                sb.Append("</dependencylist>");
                sb.Append("<keylist>");
                sb.Append("<reskey>1:0333406c:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "</reskey>");
                sb.Append("<reskey>1:00b2d882:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "</reskey>");
                sb.Append("<reskey>1:d4d9fbe5:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "</reskey>");
                sb.Append("<reskey>1:2e75c765:" + groupId.ToString("X8").ToLower() + ":" + instanceId.ToString("X16").ToLower() + "</reskey>");
                if (chkUseDefaultSpecular.Checked == false) { sb.Append("<reskey>1:00b2d882:" + groupId.ToString("X8").ToLower() + ":" + specInstanceId.ToString("X16").ToLower() + "</reskey>"); }
                sb.Append("</keylist>");
                sb.Append("<metatags>");
                sb.Append("<numofthumbs>1</numofthumbs>");
                sb.Append("<matcategory>" + cmbCategory.Text + "</matcategory>");
                sb.Append("</metatags>");
                sb.Append("</manifest>");

                string xml73e93eeb = sb.ToString();
                sb = null;

                sb = new StringBuilder();

                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<communityCustomContentManifest>");
                sb.Append("<installTo>{CCROOT}/installTo>");
                sb.Append("<contentType>Pattern</contentType>");
                sb.Append("<creatorName><![CDATA[" + MadScience.Helpers.sanitiseString(txtCreatorName.Text.Trim()) + "]]></creatorName>");
                sb.Append("<homePage><![CDATA[" + txtCreatorHomepage.Text.Trim() + "]]></homePage>");
                sb.Append("<toolName>DelphysPatternCreator</toolName>");
                sb.Append("</communityCustomContentManifest>");

                string xmlCommunityCustomContentPackage = sb.ToString();
                sb = null;

                Stream output;
                if (f.Extension.ToLower() == ".package")
                {
                    output = File.Open(savePath + saveName + ".package", FileMode.Create, FileAccess.ReadWrite);
                }
                else
                {
                    output = new MemoryStream();
                }

                Database db = new Database(output, false);
                ResourceKey rkey;

                rkey = new ResourceKey((uint)0xDEADBEEF, (uint)0xDEADBEEF, (ulong)0xcafebabeb000b135);
                db.SetResource(rkey, Encoding.UTF8.GetBytes(xmlCommunityCustomContentPackage));

                // 0x73e93eeb
                rkey = new ResourceKey(1944665835, 0, (ulong)0);
                db.SetResource(rkey, Encoding.UTF8.GetBytes(xml73e93eeb));

                // 0x0333406
                rkey = new ResourceKey(53690476, groupId, instanceId);
                db.SetResource(rkey, Encoding.UTF8.GetBytes(xml033406c));

                // 0xd4d9fbe5
                rkey = new ResourceKey(3571055589, groupId, instanceId);
                db.SetResource(rkey, Encoding.UTF8.GetBytes(xmld4d9fbe));

                // 0x00B2D882
                Stream ddsImage = File.Open(txtSourceDDS.Text, FileMode.Open);
                rkey = new ResourceKey(11720834, groupId, instanceId);
                db.SetResourceStream(rkey, ddsImage);
                ddsImage.Close();

                if (chkUseDefaultSpecular.Checked == false)
                {
                    // 0x00B2D882
                    Stream ddsImageSpec = File.Open(lblSpecularCustom.Text, FileMode.Open);
                    rkey = new ResourceKey(11720834, groupId, specInstanceId);
                    db.SetResourceStream(rkey, ddsImageSpec);
                    ddsImageSpec.Close();

                }

                // PNG file
                MemoryStream pngFile = new MemoryStream();
                if (radioButton1.Checked)
                {
                    toggleColourPalettes(true, true, false, false, false);
                }
                if (radioButton2.Checked)
                {
                    toggleColourPalettes(true, true, true, false, false);
                }
                if (radioButton3.Checked)
                {
                    toggleColourPalettes(true, true, true, true, false);
                }
                if (radioButton4.Checked)
                {
                    toggleColourPalettes(true, true, true, true, true);
                }

                pictureBox1.Image.Save(pngFile, System.Drawing.Imaging.ImageFormat.Png);

                pngFile.Seek(0, SeekOrigin.Begin);
                rkey = new ResourceKey(instanceId, 0x2E75C765, groupId);
                db.SetResourceStream(rkey, pngFile);

                db.Commit(true);

                //output.Close();

                //FileInfo fi = new FileInfo(saveFileDialog1.FileName);

                if (f.Extension.ToLower() == ".sims3pack")
                {

                    sb = new StringBuilder();
                    sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sb.Append("<Sims3Package Type=\"pattern\" SubType=\"0x00000000\">");
                    sb.Append("<ArchiveVersion>1.4</ArchiveVersion>");
                    sb.Append("<CodeVersion>0.0.0.34</CodeVersion>");
                    sb.Append("<GameVersion>0.0.0.0</GameVersion>");
                    sb.Append("<PackageId>" + pName + "</PackageId>");
                    sb.Append("<Date>" + dateTimePicker1.Value.ToString() + "</Date>");
                    sb.Append("<AssetVersion>0</AssetVersion>");
                    sb.Append("<MinReqVersion>1.0.0.0</MinReqVersion>");
                    sb.Append("<DisplayName><![CDATA[" + txtPatternTitle.Text + "]]></DisplayName>");
                    sb.Append("<Description><![CDATA[" + txtPatternDesc.Text + "]]></Description>");
                    //sb.Append("<DisplayName>" + txtPatternTitle.Text + "</DisplayName>");
                    //sb.Append("<Description>" + txtPatternDesc.Text + "</Description>");
                    sb.Append("<Dependencies>");
                    sb.Append("<Dependency>0x050cffe800000000050cffe800000000</Dependency>");
                    //sb.Append("<Dependency>" + pName + "</Dependency>");
                    sb.Append("</Dependencies>");
                    sb.Append("<LocalizedNames>");
                    sb.Append("<LocalizedName Language=\"en-US\"><![CDATA[" + txtPatternTitle.Text + "]]></LocalizedName>");
                    sb.Append("</LocalizedNames>");
                    sb.Append("<LocalizedDescriptions>");
                    sb.Append("<LocalizedDescription Language=\"en-US\"><![CDATA[" + txtPatternDesc.Text + "]]></LocalizedDescription>");
                    sb.Append("</LocalizedDescriptions>");
                    sb.Append("<PackagedFile>");
                    sb.Append("<Name>" + pName + ".package</Name>");
                    sb.Append("<Length>" + output.Length + "</Length>");
                    sb.Append("<Offset>0</Offset>");
                    //sb.Append("    <Crc>317211BAD0B3E0F5</Crc>");
                    sb.Append("<Guid>" + pName + "</Guid>");
                    sb.Append("<ContentType>pattern</ContentType>");
                    sb.Append("<MetaTags />");
                    sb.Append("</PackagedFile>");
                    /*
                    sb.Append("    <metatags>");
                    sb.Append("      <numOfThumbs>1</numOfThumbs>");
                    sb.Append("      <matCategory>" + cmbCategory.Text + "</matCategory>");
                    sb.Append("    </metatags>");
                    
                    */
                    /*
                    sb.Append("  <PackagedFile>");
                    sb.Append("    <Name>" + pName + ".png</Name>");
                    sb.Append("    <Length>" + pngFile.Length + "</Length>");
                    sb.Append("    <Offset>" + output.Length + "</Offset>");
                    //sb.Append("    <Crc>317211BAD0B3E0F5</Crc>");
                    sb.Append("    <Guid>" + pName + "png</Guid>");
                    sb.Append("    <ContentType>unknown</ContentType>");
                    sb.Append("    <metatags />");
                    sb.Append("  </PackagedFile>");
                    */
                    sb.Append("</Sims3Package>");

                    string s3p_xml = sb.ToString();
                    sb = null;

                    Stream sims3pack = File.Open(savePath + saveName + ".Sims3Pack", FileMode.Create, FileAccess.ReadWrite);

                    MadScience.StreamHelpers.WriteValueU32(sims3pack, 7);
                    MadScience.StreamHelpers.WriteStringASCII(sims3pack, "TS3Pack");
                    MadScience.StreamHelpers.WriteValueU16(sims3pack, 257);
                    MadScience.StreamHelpers.WriteValueU32(sims3pack, (uint)s3p_xml.Length);
                    MadScience.StreamHelpers.WriteStringUTF8(sims3pack, s3p_xml);

                    MadScience.Helpers.CopyStream(output, sims3pack, true);
                    //ReadWriteStream(pngFile, sims3pack, true);

                    sims3pack.Close();


                }
                pngFile.Close();
                output.Close();

                toggleEverything(true);
                label1.Visible = false;

            }

        }

        /*
        private void ReadWriteStream(Stream readStream, Stream writeStream, bool fromStart)
        {
            if (fromStart)
            {
                readStream.Seek(0, SeekOrigin.Begin);
            }

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
        */

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();

            frm.ShowDialog();

            frm.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            newPatternToolStripMenuItem_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isPreviewImage = true;
            showDDSChannels();
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            isPreviewImage = false;
            showDDSChannels();
        }

        private void chkAllowDecal_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "DDS files|*.dds";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                lblSpecularCustom.Text = openFileDialog1.FileName;
            }
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

    }
}
