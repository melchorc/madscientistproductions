using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using MadScience;

namespace CASPartEditor
{
    public partial class Form1 : Form
    {
        //private int patternBrowserCategory = 0;
        PatternBrowser.PatternBrowser pBrowser = new PatternBrowser.PatternBrowser();


        private Color showColourDialog(Color input)
        {

            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = input;
            if (cpd.ShowDialog() == DialogResult.OK)
            {
                return cpd.Color;
            }
            else
            {
                return input;
            }

        }


        private void viewPatternChannelInfo()
        {
            if (listView1.SelectedItems.Count == 1)
            {

                int patternNo = cmbPatternSelect.SelectedIndex;
                int channelNo = cmbChannelSelect.SelectedIndex;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                if (chunk.pattern[patternNo].ChannelEnabled[channelNo] == "True")
                {
                    chkPatternChannelEnabled.Checked = true;
                }
                else
                {
                    chkPatternChannelEnabled.Checked = false;
                }

                txtPatternChannelTexture.Text = chunk.pattern[patternNo].Channel[channelNo];
                txtPatternChannelH.Text = chunk.pattern[patternNo].H[channelNo];
                txtPatternChannelS.Text = chunk.pattern[patternNo].S[channelNo];
                txtPatternChannelV.Text = chunk.pattern[patternNo].V[channelNo];
                txtPatternChannelBaseH.Text = chunk.pattern[patternNo].BaseH[channelNo];
                txtPatternChannelBaseS.Text = chunk.pattern[patternNo].BaseS[channelNo];
                txtPatternChannelBaseV.Text = chunk.pattern[patternNo].BaseV[channelNo];
                txtPatternChannelBaseHSVShift.Text = chunk.pattern[patternNo].HSVShift[channelNo];

            }
        }

        private void makePatternPreviewThumb()
        {
            if (listView1.SelectedItems.Count == 1)
            {

                patternDetails temp = commitPatternDetails();

                picPatternThumb.Image = this.pBrowser.makePatternThumb(pBrowser.findPattern(temp.key), false, pBrowser.pDetailsTopFile(temp));
                picPatternThumb.Visible = true;
            }

        }

        private void showPatternDetails(int designNo)
        {
            xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[designNo];

            int i = cmbPatternSelect.SelectedIndex;
            if (chunk.filename == "CasRgbMask" && i == 3)
            {
                grpPatternA.Enabled = false;
            }
            else
            {
                grpPatternA.Enabled = true;
                showPatternDetails(chunk.pattern[i]);
            }
        }

        private void showPatternDetails(patternDetails pDetails)
        {
            showPatternDetails(pDetails, true);
        }

        private void showPatternDetails(patternDetails pDetails, bool doEnable)
        {

            switch (pDetails.type)
            {
                case "solidColor":
                    cmbPatternAType.SelectedIndex = 0;
                    break;
                case "HSV":
                    cmbPatternAType.SelectedIndex = 2;
                    break;
                case "Coloured":
                    cmbPatternAType.SelectedIndex = 1;
                    break;
            }

            if (doEnable)
            {
                if (pDetails.Enabled.ToLower() == "true") { chkPatternAEnabled.Checked = true; }
                else { chkPatternAEnabled.Checked = false; }
            }
            txtPatternABaseHBg.Text = pDetails.BaseHBg;
            txtPatternABaseSBg.Text = pDetails.BaseSBg;
            txtPatternABaseVBg.Text = pDetails.BaseVBg;
            txtPatternBGImage.Text = pDetails.BackgroundImage;
            txtPatternAFilename.Text = pDetails.filename;
            txtPatternAHBg.Text = pDetails.HBg;
            txtPatternASBg.Text = pDetails.SBg;
            txtPatternAVBg.Text = pDetails.VBg;
            txtPatternAHSVShiftBG.Text = pDetails.HSVShiftBg;
            txtPatternAKey.Text = pDetails.key;
            txtPatternAName.Text = pDetails.name;
            if (pDetails.Linked.ToLower() == "true") { chkPatternALinked.Checked = true; }
            else { chkPatternALinked.Checked = false; }
            txtPatternARGBMask.Text = pDetails.rgbmask;
            txtPatternASpecular.Text = pDetails.specmap;
            txtPatternATiling.Text = pDetails.Tiling;
            picPatternSolidColour.BackColor = MadScience.Helpers.convertColour(pDetails.Color);

            grpPatternA.Enabled = true;

            if (pDetails.type == "Coloured")
            {
                PatternBrowser.patternsFile pFile = pBrowser.pDetailsTopFile(pDetails);
                PatternBrowser.patternsFile pattern = pBrowser.findPattern(pDetails.key);

                // Check if pFile has the correct colours
                if (String.IsNullOrEmpty(pattern.color0)) { pFile.color0 = ""; pDetails.ColorP[0] = null; }
                if (String.IsNullOrEmpty(pattern.color1)) { pFile.color1 = ""; pDetails.ColorP[1] = null; }
                if (String.IsNullOrEmpty(pattern.color2)) { pFile.color2 = ""; pDetails.ColorP[2] = null; }
                if (String.IsNullOrEmpty(pattern.color3)) { pFile.color3 = ""; pDetails.ColorP[3] = null; }
                if (String.IsNullOrEmpty(pattern.color4)) { pFile.color4 = ""; pDetails.ColorP[4] = null; }

                picPatternThumb.Image = this.pBrowser.makePatternThumb(pattern, false, pFile);
                picPatternThumb.Visible = true;
            }

            if (pDetails.type == "HSV")
            {
                if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pDetails.name + ".png"))))
                {
                    Stream pngThumb = File.OpenRead(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pDetails.name + ".png")));
                    picPatternThumb.Image = Image.FromStream(pngThumb);
                    pngThumb.Close();
                    picPatternThumb.Visible = true;
                }
                else
                {
                    picPatternThumb.Image = this.pBrowser.makePatternThumb(pDetails.key);
                    picPatternThumb.Visible = true;
                }
            }

            Color temp;
            temp = MadScience.Helpers.convertColour(pDetails.ColorP[0], true);
            picPatternColourBg.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColourBg.Tag = "empty";
                picPatternColourBg.Visible = false;
            }
            else
            {
                picPatternColourBg.Tag = "color";
                picPatternColourBg.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[1], true);
            picPatternColour1.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour1.Tag = "empty";
                picPatternColour1.Visible = false;
            }
            else
            {
                picPatternColour1.Tag = "color";
                picPatternColour1.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[2], true);
            picPatternColour2.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour2.Tag = "empty";
                picPatternColour2.Visible = false;
            }
            else
            {
                picPatternColour2.Tag = "color";
                picPatternColour2.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[3], true);
            picPatternColour3.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour3.Tag = "empty";
                picPatternColour3.Visible = false;
            }
            else
            {
                picPatternColour3.Tag = "color";
                picPatternColour3.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[4], true);
            picPatternColour4.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour4.Tag = "empty";
                picPatternColour4.Visible = false;
            }
            else
            {
                picPatternColour4.Tag = "color";
                picPatternColour4.Visible = true;
            }

        }

        private patternDetails commitPatternDetails()
        {

            patternDetails pDetails = new patternDetails();

            if (chkPatternAEnabled.Checked == true)
            {
                //grpPatternA.Enabled = true;
                pDetails.Enabled = "True";
            }
            else
            {
                //grpPatternA.Enabled = false;
                pDetails.Enabled = "False";
            }

            if (txtPatternATiling.Text.Trim() != "") { pDetails.Tiling = txtPatternATiling.Text; }
            if (chkPatternALinked.Checked == true) { pDetails.Linked = "true"; } else { pDetails.Linked = "false"; }
            if (txtPatternAName.Text.Trim() != "") { pDetails.name = txtPatternAName.Text; }
            if (txtPatternAFilename.Text.Trim() != "") { pDetails.filename = txtPatternAFilename.Text; }
            if (txtPatternAKey.Text.Trim() != "") { pDetails.key = txtPatternAKey.Text; }
            pDetails.rgbmask = txtPatternARGBMask.Text;
            pDetails.specmap = txtPatternASpecular.Text;

            switch (cmbPatternAType.SelectedIndex)
            {
                case 0:
                    pDetails.type = "solidColor";
                    break;
                case 2:
                    pDetails.type = "HSV";
                    break;
                case 1:
                    pDetails.type = "Coloured";
                    break;
            }

            if (pDetails.type == "solidColor")
            {
                pDetails.nameHigh = pDetails.filename;
                pDetails.Color = MadScience.Helpers.convertColour(picPatternSolidColour.BackColor);
            }
            if (pDetails.type == "HSV")
            {
                pDetails.nameHigh = pDetails.filename;
                if (txtPatternBGImage.Text.Trim() != "") { pDetails.BackgroundImage = txtPatternBGImage.Text; }
                if (txtPatternAHBg.Text.Trim() != "") { pDetails.HBg = txtPatternAHBg.Text; }
                if (txtPatternASBg.Text.Trim() != "") { pDetails.SBg = txtPatternASBg.Text; }
                if (txtPatternAVBg.Text.Trim() != "") { pDetails.VBg = txtPatternAVBg.Text; }
                if (txtPatternABaseHBg.Text.Trim() != "") { pDetails.BaseHBg = txtPatternABaseHBg.Text; }
                if (txtPatternABaseSBg.Text.Trim() != "") { pDetails.BaseSBg = txtPatternABaseSBg.Text; }
                if (txtPatternABaseVBg.Text.Trim() != "") { pDetails.BaseVBg = txtPatternABaseVBg.Text; }
                if (txtPatternAHSVShiftBG.Text.Trim() != "") { pDetails.HSVShiftBg = txtPatternAHSVShiftBG.Text; }
            }
            if (pDetails.type == "Coloured")
            {
                pDetails.nameHigh = @"Materials\Miscellaneous\solidColor_1";
                if (picPatternColourBg.Visible) { pDetails.ColorP[0] = MadScience.Helpers.convertColour(picPatternColourBg.BackColor); }
                else { pDetails.ColorP[0] = null; }
                if (picPatternColour1.Visible) { pDetails.ColorP[1] = MadScience.Helpers.convertColour(picPatternColour1.BackColor); }
                else { pDetails.ColorP[1] = null; }
                if (picPatternColour2.Visible) { pDetails.ColorP[2] = MadScience.Helpers.convertColour(picPatternColour2.BackColor); }
                else { pDetails.ColorP[2] = null; }
                if (picPatternColour3.Visible) { pDetails.ColorP[3] = MadScience.Helpers.convertColour(picPatternColour3.BackColor); }
                else { pDetails.ColorP[3] = null; }
                if (picPatternColour4.Visible) { pDetails.ColorP[4] = MadScience.Helpers.convertColour(picPatternColour4.BackColor); }
                else { pDetails.ColorP[4] = null; }
            }

            return pDetails;
        }


    }
}