using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace MadScience
{
    public static class Patterns
    {

        public static Stream findPattern(patternDetails pattern)
        {
            Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(0x00B2D882, 0x0, StringHelpers.HashFNV64(pattern.name));

            Stream patternThumb = KeyUtils.findKey(rKey);

            return patternThumb;
        }

        public static Image makePatternThumb(patternDetails pattern)
        {
            return makePatternThumb(pattern, null);
        }

        public static Image makePatternThumb(patternDetails pattern, Wrappers.Database db)
        {
            Image temp = null;

                if (pattern.type == "HSV")
                {
                    DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
                    
                    Colours.HSVColor channel1Color = new Colours.HSVColor();
                    Colours.HSVColor channel2Color = new Colours.HSVColor();
                    Colours.HSVColor channel3Color = new Colours.HSVColor();

                    Colours.HSVColor backColor = new Colours.HSVColor(Convert.ToDouble(pattern.HBg, CultureInfo.InvariantCulture) * 360, Convert.ToDouble(pattern.SBg, CultureInfo.InvariantCulture), Convert.ToDouble(pattern.VBg, CultureInfo.InvariantCulture));
                    bool[] channelsEnabled = new bool[3];

                    if (pattern.Channel[0] != null && pattern.ChannelEnabled[0] != null && pattern.ChannelEnabled[0].ToLower() == "true")
                    {
                        channel1Color = new Colours.HSVColor(Convert.ToDouble(pattern.H[0], CultureInfo.InvariantCulture) * 360, Convert.ToDouble(pattern.S[0], CultureInfo.InvariantCulture), Convert.ToDouble(pattern.V[0], CultureInfo.InvariantCulture));
                        channelsEnabled[0] = true;
                    }
                    if (pattern.Channel[1] != null && pattern.ChannelEnabled[1] != null && pattern.ChannelEnabled[1].ToLower() == "true")
                    {
                        channel2Color = new Colours.HSVColor(Convert.ToDouble(pattern.H[1], CultureInfo.InvariantCulture) * 360, Convert.ToDouble(pattern.S[1], CultureInfo.InvariantCulture), Convert.ToDouble(pattern.V[1], CultureInfo.InvariantCulture));
                        channelsEnabled[1] = true;
                    }
                    if (pattern.Channel[2] != null && pattern.ChannelEnabled[2] != null && pattern.ChannelEnabled[2].ToLower() == "true")
                    {
                        channel3Color = new Colours.HSVColor(Convert.ToDouble(pattern.H[2], CultureInfo.InvariantCulture) * 360, Convert.ToDouble(pattern.S[2], CultureInfo.InvariantCulture), Convert.ToDouble(pattern.V[2], CultureInfo.InvariantCulture));
                        channelsEnabled[2] = true;
                    }
                    if (isEmptyMask(pattern.rgbmask))
                    {
                        if (db != null)
                        {
                            temp = Patterns.createHSVPattern(KeyUtils.findKey(new Wrappers.ResourceKey(pattern.BackgroundImage), 2, db), backColor);
                        }
                        else
                        {
                            temp = Patterns.createHSVPattern(KeyUtils.findKey(pattern.BackgroundImage), backColor);
                        }
                    }
                    else
                    {
                        if (db != null)
                        {
                            temp = Patterns.createHSVPattern(KeyUtils.findKey(new Wrappers.ResourceKey(pattern.BackgroundImage), 2, db), KeyUtils.findKey(new Wrappers.ResourceKey(pattern.rgbmask), 2, db), backColor, KeyUtils.findKey(new MadScience.Wrappers.ResourceKey(makeKey(pattern.Channel[0])), 0, db), KeyUtils.findKey(new Wrappers.ResourceKey(makeKey(pattern.Channel[1])), 0, db), KeyUtils.findKey(new Wrappers.ResourceKey(makeKey(pattern.Channel[2])), 0, db), channel1Color, channel2Color, channel3Color, channelsEnabled);
                        }
                        else
                        {
                            temp = Patterns.createHSVPattern(KeyUtils.findKey(pattern.BackgroundImage), KeyUtils.findKey(pattern.rgbmask), backColor, KeyUtils.findKey(makeKey(pattern.Channel[0])), KeyUtils.findKey(makeKey(pattern.Channel[1])), KeyUtils.findKey(makeKey(pattern.Channel[2])), channel1Color, channel2Color, channel3Color, channelsEnabled);
                        }
                    }
                }
                else if (pattern.type == "Coloured")
                {
                    DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
                    Color bgColor = Colours.convertColour(pattern.ColorP[0], true);
                    if (bgColor == Color.Empty)
                    {
                        bgColor = Color.Black;
                    }
                    if (pattern.isCustom)
                    {
                        // We need this in here becuase findKey only searches the game files and any local DDS files - it
                        // doesn't search custom packages
                        if (File.Exists(pattern.customFilename))
                        {
                            Stream patternThumb = KeyUtils.searchForKey(pattern.rgbmask, pattern.customFilename);
                            if (!Helpers.isValidStream(patternThumb))
                            {
                                patternThumb = KeyUtils.searchForKey(pattern.BackgroundImage, pattern.customFilename);
                            }
                            if (Helpers.isValidStream(patternThumb))
                            {
                                ddsP.Load(patternThumb);
                            }
                            patternThumb.Close();
                        }
                    }
                    else
                    {
                        if (db != null)
                        {
                            ddsP.Load(KeyUtils.findKey(new Wrappers.ResourceKey(pattern.rgbmask), 2, db));
                        }
                        else
                        {
                            ddsP.Load(KeyUtils.findKey(pattern.rgbmask));
                        }
                    }
                    temp = ddsP.Image(bgColor, Colours.convertColour(pattern.ColorP[1], true), Colours.convertColour(pattern.ColorP[2], true), Colours.convertColour(pattern.ColorP[3], true), Colours.convertColour(pattern.ColorP[4], true));

                }
                else if (pattern.type == "solidColor")
                {
                    temp = new Bitmap(256,256);
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.FillRectangle(new SolidBrush(Colours.convertColour(pattern.Color)),0,0,256,256);
                    }

                }


            return temp;
        }

        private static String makeKey(String text)
        {
            if (text == null)
                return "";
            string patternTexture = text.Replace(@"($assetRoot)\InGame\Complates\Materials\", "");
            patternTexture = patternTexture.Replace(@".tga", "");
            patternTexture = patternTexture.Replace(@".dds", "");
            string fullName = patternTexture.Substring(patternTexture.LastIndexOf("\\") + 1);
            return "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(fullName).ToString("X16");
        }

        public static bool isEmptyMask(String key)
        {
            //only textures, that consist entirely of RGBA[255,0,0,0]
            switch (key)
            {
                case "key:00B2D882:00000000:646B487723D17864": return true;
                case "key:00B2D882:00000000:75F8F21E0F143CAC": return true;
                case "key:00B2D882:00000000:6F04C03483C744EC": return true;
                default: return false;
            }
        }

        public static bool isEmptyTexture(String key)
        {
            //only textures, that have the alpha channel == 0

            //Empty maks have by definition the alpha channel == 0
            if (isEmptyMask(key))
                return true;
            
            //other textures
            switch (key)
            {
                case "key:00B2D882:00000000:34CC2DBA7857B5CB": return true;
                default: return false;
            }
        }

        public static patternDetails parsePatternComplate(Stream xmlStream)
        {
            int level = 0;
            string curPattern = "";

            patternDetails pDetail = new patternDetails();

            if (xmlStream == null) return pDetail;
            string patternTexture = "";

            XmlTextReader xtr = new XmlTextReader(xmlStream);

            while (xtr.Read())
            {
                if (xtr.NodeType == XmlNodeType.Element)
                {
                    switch (xtr.Name)
                    {
                        case "complate":
                            level++;

                            pDetail.name = xtr.GetAttribute("name");
                            pDetail.category = xtr.GetAttribute("category");
                            pDetail.key = xtr.GetAttribute("reskey");

                            break;
                        case "variables":
                            level++;
                            break;
                        case "texturePart":
                            level++;
                            break;
                        case "destination":
                            level++;
                            break;
                        case "param":
                            if (level == 1)
                            {
                                // Normal complate stuff
                                xtr.MoveToNextAttribute();
                                //Console.WriteLine(xtr.Value);
                            }
                            if (level == 2)
                            {
                                // Inside pattern
                                xtr.MoveToAttribute("name");

                                //Console.WriteLine(xtr.Value);
                                switch (xtr.Value)
                                {
                                    case "assetRoot":
                                        pDetail.assetRoot = xtr.GetAttribute("default");
                                        break;
                                    case "Color":
                                        pDetail.Color = xtr.GetAttribute("default");
                                        break;

                                    case "Color 0":
                                        pDetail.ColorP[0] = xtr.GetAttribute("default");
                                        break;
                                    case "Color 1":
                                        pDetail.ColorP[1] = xtr.GetAttribute("default");
                                        break;
                                    case "Color 2":
                                        pDetail.ColorP[2] = xtr.GetAttribute("default");
                                        break;
                                    case "Color 3":
                                        pDetail.ColorP[3] = xtr.GetAttribute("default");
                                        break;
                                    case "Color 4":
                                        pDetail.ColorP[4] = xtr.GetAttribute("default");
                                        break;

                                    case "Channel 1":
                                        pDetail.Channel[0] = xtr.GetAttribute("default");
                                        break;
                                    case "Channel 2":
                                        pDetail.Channel[1] = xtr.GetAttribute("default");
                                        break;
                                    case "Channel 3":
                                        pDetail.Channel[2] = xtr.GetAttribute("default");
                                        break;
                                    case "Channel 4":
                                        pDetail.Channel[3] = xtr.GetAttribute("default");
                                        break;

                                    case "Channel 1 Enabled":
                                        pDetail.ChannelEnabled[0] = xtr.GetAttribute("default");
                                        break;
                                    case "Channel 2 Enabled":
                                        pDetail.ChannelEnabled[1] = xtr.GetAttribute("default");
                                        break;
                                    case "Channel 3 Enabled":
                                        pDetail.ChannelEnabled[2] = xtr.GetAttribute("default");
                                        break;
                                    case "Channel 4 Enabled":
                                        pDetail.ChannelEnabled[3] = xtr.GetAttribute("default");
                                        break;


                                    case "Background Image":
                                        pDetail.BackgroundImage = xtr.GetAttribute("default");
                                        if (patternTexture == "") patternTexture = pDetail.BackgroundImage;
                                        break;

                                    case "H Bg":
                                        pDetail.HBg = xtr.GetAttribute("default");
                                        break;
                                    case "H 1":
                                        pDetail.H[0] = xtr.GetAttribute("default");
                                        break;
                                    case "H 2":
                                        pDetail.H[1] = xtr.GetAttribute("default");
                                        break;
                                    case "H 3":
                                        pDetail.H[2] = xtr.GetAttribute("default");
                                        break;
                                    case "H 4":
                                        pDetail.H[3] = xtr.GetAttribute("default");
                                        break;

                                    case "S Bg":
                                        pDetail.SBg = xtr.GetAttribute("default");
                                        break;
                                    case "S 1":
                                        pDetail.S[0] = xtr.GetAttribute("default");
                                        break;
                                    case "S 2":
                                        pDetail.S[1] = xtr.GetAttribute("default");
                                        break;
                                    case "S 3":
                                        pDetail.S[2] = xtr.GetAttribute("default");
                                        break;
                                    case "S 4":
                                        pDetail.S[3] = xtr.GetAttribute("default");
                                        break;

                                    case "V Bg":
                                        pDetail.VBg = xtr.GetAttribute("default");
                                        break;
                                    case "V 1":
                                        pDetail.V[0] = xtr.GetAttribute("default");
                                        break;
                                    case "V 2":
                                        pDetail.V[1] = xtr.GetAttribute("default");
                                        break;
                                    case "V 3":
                                        pDetail.V[2] = xtr.GetAttribute("default"); 
                                        break;
                                    case "V 4":
                                        pDetail.V[3] = xtr.GetAttribute("default");
                                        break;


                                    case "Base H Bg":
                                        pDetail.BaseHBg = xtr.GetAttribute("default");
                                        break;
                                    case "Base H 1":
                                        pDetail.BaseH[0] = xtr.GetAttribute("default");
                                        break;
                                    case "Base H 2":
                                        pDetail.BaseH[1] = xtr.GetAttribute("default");
                                        break;
                                    case "Base H 3":
                                        pDetail.BaseH[2] = xtr.GetAttribute("default");
                                        break;
                                    case "Base H 4":
                                        pDetail.BaseH[3] = xtr.GetAttribute("default");
                                        break;

                                    case "Base S Bg":
                                        pDetail.BaseSBg = xtr.GetAttribute("default");
                                        break;
                                    case "Base S 1":
                                        pDetail.BaseS[0] = xtr.GetAttribute("default");
                                        break;
                                    case "Base S 2":
                                        pDetail.BaseS[1] = xtr.GetAttribute("default");
                                        break;
                                    case "Base S 3":
                                        pDetail.BaseS[2] = xtr.GetAttribute("default");
                                        break;
                                    case "Base S 4":
                                        pDetail.BaseS[3] = xtr.GetAttribute("default");
                                        break;

                                    case "Base V Bg":
                                        pDetail.BaseVBg = xtr.GetAttribute("default");
                                        break;
                                    case "Base V 1":
                                        pDetail.BaseV[0] = xtr.GetAttribute("default");
                                        break;
                                    case "Base V 2":
                                        pDetail.BaseV[1] = xtr.GetAttribute("default");
                                        break;
                                    case "Base V 3":
                                        pDetail.BaseV[2] = xtr.GetAttribute("default");
                                        break;
                                    case "Base V 4":
                                        pDetail.BaseV[3] = xtr.GetAttribute("default");
                                        break;

                                    case "HSVShift Bg":
                                        pDetail.HSVShiftBg = xtr.GetAttribute("default");
                                        break;
                                    case "HSVShift 1":
                                        pDetail.HSVShift[0] = xtr.GetAttribute("default");
                                        break;
                                    case "HSVShift 2":
                                        pDetail.HSVShift[1] = xtr.GetAttribute("default");
                                        break;
                                    case "HSVShift 3":
                                        pDetail.HSVShift[2] = xtr.GetAttribute("default");
                                        break;
                                    case "HSVShift 4":
                                        pDetail.HSVShift[3] = xtr.GetAttribute("default");
                                        break;

                                    case "rgbmask":
                                        pDetail.rgbmask = xtr.GetAttribute("default");
                                        if (patternTexture == "") patternTexture = pDetail.rgbmask;
                                        break;
                                    case "specmap":
                                        pDetail.specmap = xtr.GetAttribute("default");
                                        break;
                                    case "filename":
                                        pDetail.filename = xtr.GetAttribute("default");
                                        break;

                                }
                            }
                            break;
                        case "pattern":
                            level++;
                            string a1 = "";
                            string a2 = "";
                            string a3 = "";

                            xtr.MoveToNextAttribute();
                            a1 = xtr.Value;
                            xtr.MoveToNextAttribute();
                            a2 = xtr.Value;
                            xtr.MoveToNextAttribute();
                            a3 = xtr.Value;
                            curPattern = a3;

                            break;
                        case "step":
                            if (level == 3)
                            {
                                // Inside pattern
                                xtr.MoveToAttribute("type");
                                switch (xtr.Value)
                                {
                                    case "ColorFill":
                                        if (xtr.AttributeCount == 2)
                                        {
                                            // Okay here we have to get creative.  In-Game patterns often have the params as Color 1 through 4, but custom patterns have them as Color 0 through 3.
                                            // What we need to do is to try and detect which this pattern is - in-game or custom, and to shift things accordingly so we can load the background colour in.

                                            if (pDetail.ColorP[0] != null)
                                            {
                                                // We already have a background color... so check to see if we are already filled up
                                                if (pDetail.ColorP[4] == null)
                                                {
                                                    pDetail.ColorP[4] = pDetail.ColorP[3];
                                                    pDetail.ColorP[3] = pDetail.ColorP[2];
                                                    pDetail.ColorP[2] = pDetail.ColorP[1];
                                                    pDetail.ColorP[1] = pDetail.ColorP[0];
                                                }
                                            }

                                            xtr.MoveToAttribute("color");
                                            pDetail.ColorP[0] = xtr.Value;
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
                if (xtr.NodeType == XmlNodeType.EndElement)
                {
                    switch (xtr.Name)
                    {
                        case "variables":
                            level--;
                            break;
                        case "complate":
                            level--;
                            break;
                        case "texturePart":
                            level--;
                            break;
                        case "destination":
                            level--;
                            break;
                    }
                }
            }

            if (patternTexture != "")
            {
                //Console.WriteLine(lookupList.Items[i].fullCasPartname);

                if (!MadScience.KeyUtils.validateKey(patternTexture, false))
                {
                    patternTexture = patternTexture.Replace(@"($assetRoot)\InGame\Complates\", "");
                    patternTexture = patternTexture.Replace(@".tga", "");
                    patternTexture = patternTexture.Replace(@".dds", "");
                }
                else
                {
                    // This is a custom pattern so we have to construct up the filename ourselves
                    patternTexture = "Materials\\" + pDetail.category + "\\" + pDetail.name;
                }

                pDetail.filename = patternTexture;
            }

            if (!String.IsNullOrEmpty(pDetail.rgbmask) && !MadScience.KeyUtils.validateKey(pDetail.rgbmask, false))
            {
                // Need to change rgbmask to key format
                pDetail.rgbmask = "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(pDetail.rgbmask.Substring(pDetail.rgbmask.LastIndexOf("\\") + 1).Replace(@".tga", "").Replace(@".dds", "")).ToString("X16");
            }
            if (!String.IsNullOrEmpty(pDetail.specmap) && !MadScience.KeyUtils.validateKey(pDetail.specmap, false))
            {
                // Need to change rgbmask to key format
                pDetail.specmap = "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(pDetail.specmap.Substring(pDetail.specmap.LastIndexOf("\\") + 1).Replace(@".tga", "").Replace(@".dds", "")).ToString("X16");
            }

            if (!String.IsNullOrEmpty(pDetail.BackgroundImage) && !MadScience.KeyUtils.validateKey(pDetail.BackgroundImage, false))
            {
                pDetail.BackgroundImage = "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(pDetail.BackgroundImage.Substring(pDetail.BackgroundImage.LastIndexOf("\\") + 1).Replace(@".tga", "").Replace(@".dds", "")).ToString("X16");
            }

            if (pDetail.name.StartsWith("solidColor") || pDetail.name.StartsWith("Flat Color"))
            {
                pDetail.type = "solidColor";
            }
            if (!String.IsNullOrEmpty(pDetail.HSVShiftBg) || !String.IsNullOrEmpty(pDetail.HSVShift[0]) || !String.IsNullOrEmpty(pDetail.HBg) || !String.IsNullOrEmpty(pDetail.BaseHBg) || !String.IsNullOrEmpty(pDetail.H[0]))
            {
                pDetail.type = "HSV";
            }
            if (pDetail.ColorP[1] != null)
            {
                pDetail.type = "Coloured";
            }
            // If we have no type, default to Coloured to be on the safe side
            if (String.IsNullOrEmpty(pDetail.type)) pDetail.type = "Coloured";
            //Console.WriteLine("Pattern " + i + ": " + xcd.pattern[i].name + " - " + xcd.pattern[i].type);

            //Console.WriteLine(Environment.NewLine);
            xtr.Close();

            return pDetail;
        }

        public static Bitmap createHSVPattern(Stream backgroundStream, Stream maskStream, MadScience.Colours.HSVColor backgroundColor, Stream channel1Stream, Stream channel2Stream, Stream channel3Stream, MadScience.Colours.HSVColor channel1Color, MadScience.Colours.HSVColor channel2Color, MadScience.Colours.HSVColor channel3Color, bool[] channelEnabled)
        {
            Bitmap output = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            Bitmap background = LoadBitmapFromStream(backgroundStream, 256, 256);
            Bitmap mask = LoadBitmapFromStream(maskStream, 256, 256);

            if (background == null || mask == null)
                return output;

            Bitmap channel1 = LoadBitmapFromStreamOrEmpty(channel1Stream, 256, 256);
            Bitmap channel2 = LoadBitmapFromStreamOrEmpty(channel2Stream, 256, 256);
            Bitmap channel3 = LoadBitmapFromStreamOrEmpty(channel3Stream, 256, 256);

            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData backgroundData = background.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData maskData = mask.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData channel1Data = channel1.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData channel2Data = channel2.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData channel3Data = channel3.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Color outputColor;
            const int pixelSize = 4;
            //process every pixel
            unsafe
            {
                for (int y = 0; y < 256; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    byte* inputRow = (byte*)backgroundData.Scan0 + (y * backgroundData.Stride);
                    byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);
                    byte* channel1Row = (byte*)channel1Data.Scan0 + (y * channel1Data.Stride);
                    byte* channel2Row = (byte*)channel2Data.Scan0 + (y * channel2Data.Stride);
                    byte* channel3Row = (byte*)channel3Data.Scan0 + (y * channel3Data.Stride);

                    for (int x = 0; x < 256; x++)
                    {

                        int pixelLocation = x * pixelSize;

                        Color inputcolor = Color.FromArgb(255, inputRow[pixelLocation + 2], inputRow[pixelLocation + 1], inputRow[pixelLocation]);
                        Color maskcolor = Color.FromArgb(maskRow[pixelLocation + 3], maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);

                        MadScience.Colours.HSVColor bgcolor = new MadScience.Colours.HSVColor(inputcolor);
                        bgcolor.Hue += backgroundColor.Hue;
                        bgcolor.Saturation += backgroundColor.Saturation;
                        bgcolor.Value += backgroundColor.Value;
                        outputColor = bgcolor.Color;

                        if (maskcolor.G > 0 && channelEnabled[0])
                        {
                            Color channelcolor = Color.FromArgb(1, channel1Row[pixelLocation + 2], channel1Row[pixelLocation + 1], channel1Row[pixelLocation]);
                            bgcolor = new MadScience.Colours.HSVColor(channelcolor);
                            bgcolor.Hue += channel1Color.Hue;
                            bgcolor.Saturation += channel1Color.Saturation;
                            bgcolor.Value += channel1Color.Value;
                            outputColor = ColorOverlay(maskcolor.G, outputColor, bgcolor.Color);
                        }

                        if (maskcolor.B > 0 && channelEnabled[1])
                        {
                            Color channelcolor = Color.FromArgb(1, channel2Row[pixelLocation + 2], channel2Row[pixelLocation + 1], channel2Row[pixelLocation]);
                            bgcolor = new MadScience.Colours.HSVColor(channelcolor);
                            bgcolor.Hue += channel2Color.Hue;
                            bgcolor.Saturation += channel2Color.Saturation;
                            bgcolor.Value += channel2Color.Value;
                            outputColor = ColorOverlay(maskcolor.B, outputColor, bgcolor.Color);
                        }

                        if (maskcolor.A > 0 && channelEnabled[2])
                        {
                            Color channelcolor = Color.FromArgb(1, channel3Row[pixelLocation + 2], channel3Row[pixelLocation + 1], channel3Row[pixelLocation]);
                            bgcolor = new MadScience.Colours.HSVColor(channelcolor);
                            bgcolor.Hue += channel3Color.Hue;
                            bgcolor.Saturation += channel3Color.Saturation;
                            bgcolor.Value += channel3Color.Value;
                            outputColor = ColorOverlay(maskcolor.A, outputColor, bgcolor.Color);
                        }

                        outputRow[pixelLocation] = (byte)outputColor.B;
                        outputRow[pixelLocation + 1] = (byte)outputColor.G;
                        outputRow[pixelLocation + 2] = (byte)outputColor.R;
                        outputRow[pixelLocation + 3] = (byte)outputColor.A;
                    }
                }
            }
            output.UnlockBits(outputData);
            mask.UnlockBits(maskData);
            background.UnlockBits(backgroundData);
            channel1.UnlockBits(channel1Data);
            channel2.UnlockBits(channel2Data);
            channel3.UnlockBits(channel3Data);

            return output;
        }

        public static Bitmap createHSVPattern(Stream backgroundStream, MadScience.Colours.HSVColor backgroundColor)
        {
            Bitmap output = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            Bitmap background = LoadBitmapFromStream(backgroundStream, 256, 256);
            if (background == null)
                return output;

            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData backgroundData = background.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Color outputColor;
            const int pixelSize = 4;
            //process every pixel
            unsafe
            {
                for (int y = 0; y < 256; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    byte* inputRow = (byte*)backgroundData.Scan0 + (y * backgroundData.Stride);

                    for (int x = 0; x < 256; x++)
                    {

                        int pixelLocation = x * pixelSize;

                        Color inputcolor = Color.FromArgb(255, inputRow[pixelLocation + 2], inputRow[pixelLocation + 1], inputRow[pixelLocation]);

                        MadScience.Colours.HSVColor bgcolor = new MadScience.Colours.HSVColor(inputcolor);
                        bgcolor.Hue += backgroundColor.Hue;
                        bgcolor.Saturation += backgroundColor.Saturation;
                        bgcolor.Value += backgroundColor.Value;
                        outputColor = bgcolor.Color;

                        outputRow[pixelLocation] = (byte)outputColor.B;
                        outputRow[pixelLocation + 1] = (byte)outputColor.G;
                        outputRow[pixelLocation + 2] = (byte)outputColor.R;
                        outputRow[pixelLocation + 3] = (byte)outputColor.A;
                    }
                }
            }
            output.UnlockBits(outputData);
            background.UnlockBits(backgroundData);

            return output;
        }

        #region helperfunctions

        public static Bitmap LoadBitmapFromStreamOrEmpty(Stream source, int width, int height)
        {
            Bitmap output;
            DdsFileTypePlugin.DdsFile d = new DdsFileTypePlugin.DdsFile();
            if (source != null && source.Length != 0)
            {
                d.Load(source);
                if (isAlphaFormat(d.m_header.fileFormat))
                    output = (Bitmap)d.Image(true, true, true, true);
                else
                    output = (Bitmap)d.Image(true, true, true);
                source.Close();
                if (output.Width != width || output.Height != height)
                {
                    if (isAlphaFormat(d.m_header.fileFormat))
                        MadScience.Patterns.ResizeBitmapCorrect(ref output, width, height);
                    else
                        MadScience.Patterns.ResizeBitmapFast(ref output, width, height);
                }
            }
            else
            {
                output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            }
            return output;
        }

        public static Bitmap LoadBitmapFromStream(Stream source, int width, int height)
        {
            Bitmap output;
            DdsFileTypePlugin.DdsFile d = new DdsFileTypePlugin.DdsFile();
            if (source != null && source.Length != 0)
            {
                d.Load(source);
                if (isAlphaFormat(d.m_header.fileFormat))
                    output = (Bitmap)d.Image(true, true, true, true);
                else
                    output = (Bitmap)d.Image(true, true, true);
                source.Close();
                if (output.Width != width || output.Height != height)
                {
                    if (isAlphaFormat(d.m_header.fileFormat))
                        MadScience.Patterns.ResizeBitmapCorrect(ref output, width, height);
                    else
                        MadScience.Patterns.ResizeBitmapFast(ref output, width, height);
                }
            }
            else
            {
                return null;
            }
            return output;
        }

        public static Bitmap LoadBitmapFromStream(Stream source)
        {
            Bitmap output;
            DdsFileTypePlugin.DdsFile d = new DdsFileTypePlugin.DdsFile();
            if (source != null && source.Length != 0)
            {
                d.Load(source);
                if (isAlphaFormat(d.m_header.fileFormat))
                    output = (Bitmap)d.Image(true, true, true, true);
                else
                    output = (Bitmap)d.Image(true, true, true);
                source.Close();
            }
            else
            {
                output = null;
            }
            return output;
        }

        private static bool isAlphaFormat(String format)
        {
            switch (format)
            {
                case "DXT1":
                    return false;
            }
            return true;
        }

        public static Bitmap mergeStencils(List<Stream> Stencils)
        {
            Bitmap output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(output);
            var d = new DdsFileTypePlugin.DdsFile();
            if ((Stencils != null))
            {
                for (int i = 0; i < Stencils.Count; i++)
                {
                    if (Stencils[i].Length != 0)
                    {
                        d.Load(Stencils[i]);
                        Bitmap _Stencil = (Bitmap)d.Image(true, true, true, true);
                        Stencils[i].Close();
                        if (!(_Stencil.Width == 1024 || _Stencil.Height == 1024))
                        {
                            ResizeBitmapCorrect(ref _Stencil, 1024, 1024);
                        }
                        g.DrawImage(_Stencil, 0, 0);
                    }
                }
            }
            g.Dispose();
            return output;
        }

        public static void ResizeBitmapFast(ref Bitmap bitmap, int width, int height)
        {

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)result))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.DrawImage(bitmap, 0, 0, width, height);
            }
            bitmap = result;

        }

        public static void ResizeBitmapCorrect(ref Bitmap bitmap, int width, int height)
        {
            float[][] ptsArrayCopyRGB =
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 0, 0},
                new float[] {0, 0, 0, 1, 1}
            };
            // Create a ColorMatrix object
            ColorMatrix clrMatrixCopyRGB = new ColorMatrix(ptsArrayCopyRGB);
            // Create image attributes
            ImageAttributes imgAttribsCopyRGB = new ImageAttributes();
            // Set color matrix
            imgAttribsCopyRGB.SetColorMatrix(clrMatrixCopyRGB,
                ColorMatrixFlag.Default,
                ColorAdjustType.Default);

            float[][] ptsArrayCopyA =
            {
                new float[] {0, 0, 0, 0, 0},
                new float[] {0, 0, 0, 0, 0},
                new float[] {0, 0, 0, 0, 0},
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 0, 0, 1, 1}
            };
            // Create a ColorMatrix object
            ColorMatrix clrMatrixCopyA = new ColorMatrix(ptsArrayCopyA);
            // Create image attributes
            ImageAttributes imgAttribsCopyA = new ImageAttributes();
            // Set color matrix
            imgAttribsCopyA.SetColorMatrix(clrMatrixCopyA,
                ColorMatrixFlag.Default,
                ColorAdjustType.Default);

            Bitmap result = new Bitmap(width, height);
            Bitmap alphamask = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imgAttribsCopyRGB);

            using (Graphics g = Graphics.FromImage((Image)alphamask))
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imgAttribsCopyA);

            BitmapData bitmapData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData maskData = alphamask.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            const int pixelSize = 4;
            //process every pixel
            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* bitmapRow = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                    byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);

                    for (int x = 0; x < width; x++)
                    {

                        int pixelLocation = x * pixelSize;

                        bitmapRow[pixelLocation + 3] = (byte)maskRow[pixelLocation + 2];

                    }
                }
            }
            result.UnlockBits(bitmapData);
            alphamask.UnlockBits(maskData);


            bitmap = result;

        }

        public static Color ProcessHairPixelRGB(Color multiplier, Color mask, Color color1, Color color2, Color color3, Color color4)
        {
            Color output = ColorMultiply(multiplier, color1);
            output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, color2));
            output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, color3));
            output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, color4));
            return output;
        }

        public static Color ProcessMakeUpPixelRGB(Color multiplier, Color mask, Color color1, Color color2, Color color3)
        {
            Color output = multiplier;
            output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, color1));
            output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, color2));
            output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, color3));
            return output;
        }

        public static Color ProcessMakeUpPixelRGBA(Color multiplier, Color mask, Color color1, Color color2, Color color3, Color color4)
        {
            Color output = multiplier;
            output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, color1));
            output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, color2));
            output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, color3));
            output = ColorOverlay(mask.A, output, ColorMultiply(multiplier, color4));

            return output;
        }

        public static Color ProcessPixelRGB(Color multiplier, Color mask, Color pattern1color, Color pattern2color, Color pattern3color)
        {
            Color output;
            output = multiplier;
            if (!pattern1color.IsEmpty) output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, pattern1color));
            if (!pattern2color.IsEmpty) output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, pattern2color));
            if (!pattern3color.IsEmpty) output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, pattern3color));
            return output;
        }

        public static Color ProcessPixelRGBA(Color multiplier, Color mask, Color pattern1color, Color pattern2color, Color pattern3color, Color pattern4color)
        {
            Color output = multiplier;
            if (!pattern1color.IsEmpty) output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, pattern1color));
            if (!pattern2color.IsEmpty) output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, pattern2color));
            if (!pattern3color.IsEmpty) output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, pattern3color));
            if (!pattern4color.IsEmpty) output = ColorOverlay(mask.A, output, ColorMultiply(multiplier, pattern4color));
            return output;
        }

        public static Color ColorMultiply(Color colorA, Color colorB)
        {
            return Color.FromArgb(colorA.A, ((int)colorA.R * colorB.R) / 255, ((int)colorA.G * colorB.G) / 255, ((int)colorA.B * colorB.B) / 255);
        }

        public static Color ColorOverlay(byte factor, Color colorA, Color colorB)
        {
            int cFactor = 255 - factor;
            return Color.FromArgb(colorA.A, ((int)colorA.R * (cFactor) + (int)colorB.R * factor) / 255, ((int)colorA.G * (cFactor) + (int)colorB.G * factor) / 255, ((int)colorA.B * (cFactor) + (int)colorB.B * factor) / 255);
        }

        public static Color ColorPaint(byte factor, Color colorA, Color colorB)
        {
            if (factor == 0)
                return colorA;
            float fFactor = (float)factor / 255;
            float fIFactor = 1 - (fFactor);
            float a = factor + colorA.A * fIFactor;
            float r = (((fIFactor * colorA.R * colorA.A)) / 255 + (fFactor * colorB.R)) / (a / 255);
            float g = (((fIFactor * colorA.B * colorA.A)) / 255 + (fFactor * colorB.G)) / (a / 255);
            float b = (((fIFactor * colorA.G * colorA.A)) / 255 + (fFactor * colorB.B)) / (a / 255);

            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }

        #endregion



    }

}
