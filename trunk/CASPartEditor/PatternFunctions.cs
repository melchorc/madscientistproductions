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


        private void viewPatternChannelInfo(patternDetails pDetail)
        {
            int channelNo = cmbChannelSelect.SelectedIndex;
            if (pDetail.ChannelEnabled[channelNo] != null)
            {
                //if (chunk.pattern[patternNo].ChannelEnabled[channelNo] == "True")
                if (pDetail.ChannelEnabled[channelNo].ToLower() == "true")
                {
                    chkPatternChannelEnabled.Checked = true;
                }
                else
                {
                    chkPatternChannelEnabled.Checked = false;
                }
            }
            else
            {
                chkPatternChannelEnabled.Checked = false;
            }
            if (pDetail.Channel[channelNo] != null) txtPatternChannelTexture.Text = pDetail.Channel[channelNo];
            else txtPatternChannelTexture.Text = "";
            if (pDetail.H[channelNo] != null) txtPatternChannelH.Text = pDetail.H[channelNo];
            else txtPatternChannelH.Text = "";
            if (pDetail.S[channelNo] != null) txtPatternChannelS.Text = pDetail.S[channelNo];
            else txtPatternChannelS.Text = "";
            if (pDetail.V[channelNo] != null) txtPatternChannelV.Text = pDetail.V[channelNo];
            else txtPatternChannelV.Text = "";
            if (pDetail.BaseH[channelNo] != null) txtPatternChannelBaseH.Text = pDetail.BaseH[channelNo];
            else txtPatternChannelBaseH.Text = "";
            if (pDetail.BaseS[channelNo] != null) txtPatternChannelBaseS.Text = pDetail.BaseS[channelNo];
            else txtPatternChannelBaseS.Text = "";
            if (pDetail.BaseV[channelNo] != null) txtPatternChannelBaseV.Text = pDetail.BaseV[channelNo];
            else txtPatternChannelBaseV.Text = "";
            if (pDetail.HSVShift[channelNo] != null) txtPatternChannelBaseHSVShift.Text = pDetail.HSVShift[channelNo];
            else txtPatternChannelBaseHSVShift.Text = "";

        }

        private void makePatternPreviewThumb()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int patternNo = cmbPatternSelect.SelectedIndex;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                //patternDetails temp = commitPatternDetails();

                //picPatternThumb.Image = Patterns.makePatternThumb(pBrowser.findPattern(temp.key), pBrowser.pDetailsTopFile(temp));
                picPatternThumb.Image = Patterns.makePatternThumb(chunk.pattern[patternNo]);
                picPatternThumb.Visible = true;

                refreshDisplay();
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

            picPatternThumb.Visible = false;

            txtPatternAFilename.Text = pDetails.filename;
            txtPatternAKey.Text = pDetails.key;
            txtPatternAName.Text = pDetails.name;
            if (pDetails.Linked.ToLower() == "true") { chkPatternALinked.Checked = true; }
            else { chkPatternALinked.Checked = false; }

            txtPatternARGBMask.Text = pDetails.rgbmask;
            txtPatternASpecular.Text = pDetails.specmap;
            txtPatternATiling.Text = pDetails.Tiling;

            switch (pDetails.type)
            {
                case "solidColor":
                    cmbPatternAType.SelectedIndex = 0;
                    picPatternSolidColour.BackColor = MadScience.Colours.convertColour(pDetails.Color);
                    break;
                case "HSV":
                    cmbPatternAType.SelectedIndex = 2;

                    txtPatternABaseHBg.Text = pDetails.BaseHBg;
                    txtPatternABaseSBg.Text = pDetails.BaseSBg;
                    txtPatternABaseVBg.Text = pDetails.BaseVBg;
                    txtPatternBGImage.Text = pDetails.BackgroundImage;
                    txtPatternAFilename.Text = pDetails.filename;
                    txtPatternAHBg.Text = pDetails.HBg;
                    txtPatternASBg.Text = pDetails.SBg;
                    txtPatternAVBg.Text = pDetails.VBg;
                    txtPatternAHSVShiftBG.Text = pDetails.HSVShiftBg;

                    viewPatternChannelInfo(pDetails);
                    Colours.HSVColor hsv = new Colours.HSVColor();
                    hsv.Hue = double.Parse(pDetails.HBg) * 360 + double.Parse(pDetails.BaseHBg) * 360;
                    hsv.Saturation = double.Parse(pDetails.SBg) + double.Parse(pDetails.BaseSBg);
                    hsv.Value = double.Parse(pDetails.VBg) + double.Parse(pDetails.BaseVBg);
                    picHSVColorBG.BackColor = hsv.Color;
                    makePatternPreviewThumb();

                    break;
                case "Coloured":
                    cmbPatternAType.SelectedIndex = 1;

                    Color temp;
                    temp = MadScience.Colours.convertColour(pDetails.ColorP[0], true);
                    picPatternColourBg.BackColor = temp;
                    if (temp == Color.Empty)
                    {
                        picPatternColourBg.Tag = "empty";
                        picPatternColourBg.Visible = false;
                        label65.Visible = false;
                    }
                    else
                    {
                        picPatternColourBg.Tag = "color";
                        picPatternColourBg.Visible = true;
                        label65.Visible = true;
                    }

                    temp = MadScience.Colours.convertColour(pDetails.ColorP[1], true);
                    picPatternColour1.BackColor = temp;
                    if (temp == Color.Empty)
                    {
                        picPatternColour1.Tag = "empty";
                        picPatternColour1.Visible = false;
                        label64.Visible = false;
                    }
                    else
                    {
                        picPatternColour1.Tag = "color";
                        picPatternColour1.Visible = true;
                        label64.Visible = true;
                    }

                    temp = MadScience.Colours.convertColour(pDetails.ColorP[2], true);
                    picPatternColour2.BackColor = temp;
                    if (temp == Color.Empty)
                    {
                        picPatternColour2.Tag = "empty";
                        picPatternColour2.Visible = false;
                        label67.Visible = false;
                    }
                    else
                    {
                        picPatternColour2.Tag = "color";
                        picPatternColour2.Visible = true;
                        label67.Visible = true;
                    }

                    temp = MadScience.Colours.convertColour(pDetails.ColorP[3], true);
                    picPatternColour3.BackColor = temp;
                    if (temp == Color.Empty)
                    {
                        picPatternColour3.Tag = "empty";
                        picPatternColour3.Visible = false;
                        label66.Visible = false;
                    }
                    else
                    {
                        picPatternColour3.Tag = "color";
                        picPatternColour3.Visible = true;
                        label66.Visible = true;
                    }

                    temp = MadScience.Colours.convertColour(pDetails.ColorP[4], true);
                    picPatternColour4.BackColor = temp;
                    if (temp == Color.Empty)
                    {
                        picPatternColour4.Tag = "empty";
                        picPatternColour4.Visible = false;
                        label68.Visible = false;
                    }
                    else
                    {
                        picPatternColour4.Tag = "color";
                        picPatternColour4.Visible = true;
                        label68.Visible = true;
                    }

                    picPatternThumb.Image = Patterns.makePatternThumb(pDetails);
                    picPatternThumb.Visible = true;

                    break;
            }

            if (doEnable)
            {
                if (pDetails.Enabled.ToLower() == "true") { chkPatternAEnabled.Checked = true; }
                else { chkPatternAEnabled.Checked = false; }
            }

            grpPatternA.Enabled = true;


        }

        private void commitPatternDetails(string param, bool value)
        {
            if (value)
            {
                commitPatternDetails(param, "True");
            }
            else
            {
                commitPatternDetails(param, "False");
            }
        }

        private void commitPatternDetails(string param, string value)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int patternNo = cmbPatternSelect.SelectedIndex;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                patternDetails pDetail = chunk.pattern[patternNo];
                switch (param)
                {
                    case "enabled":
                        pDetail.Enabled = value;
                        break;
                    case "linked":
                        pDetail.Linked = value;
                        break;
                    case "key":
                        pDetail.key = value;
                        break;
                    case "type":
                        pDetail.type = value;
                        break;
                    case "name":
                        pDetail.name = value;
                        break;
                    case "filename":
                        pDetail.filename = value;
                        break;
                    case "tiling":
                        pDetail.Tiling = value;
                        break;
                    case "rgbmask":
                        pDetail.rgbmask = value;
                        break;
                    case "bgimage":
                        pDetail.BackgroundImage = value;
                        break;
                    case "specular":
                        pDetail.specmap = value;
                        break;
                    case "Color":
                        pDetail.Color = value;
                        break;
                    case "ColorP0":
                        pDetail.ColorP[0] = value;
                        break;
                    case "ColorP1":
                        pDetail.ColorP[1] = value;
                        break;
                    case "ColorP2":
                        pDetail.ColorP[2] = value;
                        break;
                    case "ColorP3":
                        pDetail.ColorP[3] = value;
                        break;
                    case "ColorP4":
                        pDetail.ColorP[4] = value;
                        break;

                }

            }
        }

        /*
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
                pDetails.Color = MadScience.Colours.convertColour(picPatternSolidColour.BackColor);
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
                if (picPatternColourBg.Visible) { pDetails.ColorP[0] = MadScience.Colours.convertColour(picPatternColourBg.BackColor); }
                else { pDetails.ColorP[0] = null; }
                if (picPatternColour1.Visible) { pDetails.ColorP[1] = MadScience.Colours.convertColour(picPatternColour1.BackColor); }
                else { pDetails.ColorP[1] = null; }
                if (picPatternColour2.Visible) { pDetails.ColorP[2] = MadScience.Colours.convertColour(picPatternColour2.BackColor); }
                else { pDetails.ColorP[2] = null; }
                if (picPatternColour3.Visible) { pDetails.ColorP[3] = MadScience.Colours.convertColour(picPatternColour3.BackColor); }
                else { pDetails.ColorP[3] = null; }
                if (picPatternColour4.Visible) { pDetails.ColorP[4] = MadScience.Colours.convertColour(picPatternColour4.BackColor); }
                else { pDetails.ColorP[4] = null; }
            }

            return pDetails;
        }
        */

    }
}