using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Collections.Generic;
using System.Xml;


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

        //public static Image makePatternThumb(patternsFile pattern, bool saveImage, patternsFile pOverride)
        public static Image makePatternThumb(patternDetails pattern, Wrappers.Database db)
        {
            //keyName patternXML = new keyName(resKey);

            //Stream patternThumb = Stream.Null;

            //if (pattern.isCustom)
            //{

            //    if (File.Exists(pattern.customFilename))
            //    {
            //        patternThumb = KeyUtils.searchForKey("key:00B2D882:00000000:" + StringHelpers.HashFNV64(pattern.name.Substring(pattern.name.LastIndexOf("\\") +1)).ToString("X16"), pattern.customFilename);
            //        if (!Helpers.isValidStream(patternThumb))
            //        {
            //            patternThumb = KeyUtils.searchForKey(pattern.BackgroundImage, pattern.customFilename);
            //        }
            //    }
            //}
            //else
            //{
            //    Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(0x00B2D882, 0x0, StringHelpers.HashFNV64(pattern.name.Substring(pattern.name.LastIndexOf("\\") +1)));
            //    if (db != null) patternThumb = KeyUtils.findKey(rKey, 0, db);
            //    else patternThumb = KeyUtils.findKey(rKey);
            //    if (!Helpers.isValidStream(patternThumb))
            //    {
            //        rKey = new MadScience.Wrappers.ResourceKey(pattern.BackgroundImage);
            //        if (db != null) patternThumb = KeyUtils.findKey(rKey, 0, db);
            //        else patternThumb = KeyUtils.findKey(rKey);
            //    }
            //}

            Image temp = null;


                
                if (pattern.type == "HSV")
                {
                    Colours.HSVColor C0 = new Colours.HSVColor();
                    Colours.HSVColor C1 = new Colours.HSVColor();
                    Colours.HSVColor C2 = new Colours.HSVColor();
                    DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
                    Colours.HSVColor bg = new Colours.HSVColor(double.Parse(pattern.HBg) * 360, double.Parse(pattern.SBg), double.Parse(pattern.VBg));
                    Colours.HSVColor basebg = new Colours.HSVColor(double.Parse(pattern.HBg) * 360, double.Parse(pattern.SBg), double.Parse(pattern.VBg));
                    Colours.HSVColor shift = new Colours.HSVColor(double.Parse(pattern.HBg) * 360, double.Parse(pattern.SBg), double.Parse(pattern.VBg));

                    bool[] ce = new bool[3];

                    if (pattern.ChannelEnabled[0] != null && pattern.ChannelEnabled[0].ToLower() == "true")
                    {
                        C0 = new Colours.HSVColor(double.Parse(pattern.H[0]) * 360, double.Parse(pattern.S[0]), double.Parse(pattern.V[0]));
                        ce[0] = true;
                    }
                    if (pattern.ChannelEnabled[1] != null && pattern.ChannelEnabled[1].ToLower() == "true")
                    {
                        C1 = new Colours.HSVColor(double.Parse(pattern.H[1]) * 360, double.Parse(pattern.S[1]), double.Parse(pattern.V[1]));
                        ce[1] = true;
                    }
                    if (pattern.ChannelEnabled[2] != null && pattern.ChannelEnabled[2].ToLower() == "true")
                    {
                        C2 = new Colours.HSVColor(double.Parse(pattern.H[2]) * 360, double.Parse(pattern.S[2]), double.Parse(pattern.V[2]));
                        ce[2] = true;
                    }
                    if (db != null)
                    {
                        temp = Patterns.createHSVPattern(KeyUtils.findKey(new Wrappers.ResourceKey(pattern.BackgroundImage), 2, db), KeyUtils.findKey(new Wrappers.ResourceKey(pattern.rgbmask), 2, db), bg, KeyUtils.findKey(new MadScience.Wrappers.ResourceKey(makeKey(pattern.Channel[0])), 0, db), KeyUtils.findKey(new Wrappers.ResourceKey(makeKey(pattern.Channel[1])), 0, db), KeyUtils.findKey(new Wrappers.ResourceKey(makeKey(pattern.Channel[2])), 0, db), C0, C1, C2, ce);
                    }
                    else
                    {
                        temp = Patterns.createHSVPattern(KeyUtils.findKey(pattern.BackgroundImage), KeyUtils.findKey(pattern.rgbmask), bg, KeyUtils.findKey(makeKey(pattern.Channel[0])), KeyUtils.findKey(makeKey(pattern.Channel[1])), KeyUtils.findKey(makeKey(pattern.Channel[2])), C0, C1, C2, ce);
                    }
                }
                if (pattern.type == "Coloured")
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
                if (pattern.type == "solidColor")
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

        public static Bitmap createHSVPattern(Stream texture, Stream rgbmask, MadScience.Colours.HSVColor bg, Stream textureChannel1, Stream textureChannel2, Stream textureChannel3, MadScience.Colours.HSVColor Channel1bg, MadScience.Colours.HSVColor Channel2bg, MadScience.Colours.HSVColor Channel3bg, bool[] ChannelEnabled)
        {
            Bitmap input;
            Bitmap mask;
            Bitmap output;

            Bitmap channel1;
            Bitmap channel2;
            Bitmap channel3;

            var d = new DdsFileTypePlugin.DdsFile();
            if ((texture.Length != 0))
            {
                d.Load(texture);
                input = (Bitmap)d.Image(true, true, true, true);
                texture.Close();
            }
            else
            {
                input = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            if (input.Width != 256 || input.Height != 256)
            {
                ResizeBitmap(ref input, 256, 256);
            }

            if ((rgbmask.Length != 0))
            {
                d.Load(rgbmask);
                mask = (Bitmap)d.Image(true, true, true, true);
                rgbmask.Close();
            }
            else
            {
                mask = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            if (mask.Width != 256 || mask.Height != 256)
            {
                ResizeBitmap(ref mask, 256, 256);
            }

            if ((textureChannel1 != null) && (textureChannel1.Length != 0))
            {
                d.Load(textureChannel1);
                channel1 = (Bitmap)d.Image(true, true, true, true);
                textureChannel1.Close();
            }
            else
            {
                channel1 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            if (channel1.Width != 256 || channel1.Height != 256)
            {
                ResizeBitmap(ref channel1, 256, 256);
            }

            if ((textureChannel2 != null) && (textureChannel2.Length != 0))
            {
                d.Load(textureChannel2);
                channel2 = (Bitmap)d.Image(true, true, true, true);
                textureChannel2.Close();
            }
            else
            {
                channel2 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            if (channel2.Width != 256 || channel2.Height != 256)
            {
                ResizeBitmap(ref channel2, 256, 256);
            }

            if ((textureChannel3 != null) && (textureChannel3.Length != 0))
            {
                d.Load(textureChannel3);
                channel3 = (Bitmap)d.Image(true, true, true, true);
                textureChannel3.Close();
            }
            else
            {
                channel3 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            if (channel3.Width != 256 || channel3.Height != 256)
            {
                ResizeBitmap(ref channel3, 256, 256);
            }

            output = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData inputData = input.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
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
                    byte* inputRow = (byte*)inputData.Scan0 + (y * inputData.Stride);
                    byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);
                    byte* channel1Row = (byte*)channel1Data.Scan0 + (y * channel1Data.Stride);
                    byte* channel2Row = (byte*)channel2Data.Scan0 + (y * channel2Data.Stride);
                    byte* channel3Row = (byte*)channel3Data.Scan0 + (y * channel3Data.Stride);

                    for (int x = 0; x < 256; x++)
                    {

                        int pixelLocation = x * pixelSize;

                        Color inputcolor = Color.FromArgb(1, inputRow[pixelLocation + 2], inputRow[pixelLocation + 1], inputRow[pixelLocation]);
                        Color maskcolor = Color.FromArgb(1, maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);

                        MadScience.Colours.HSVColor bgcolor = new MadScience.Colours.HSVColor(inputcolor);
                        bgcolor.Hue += bg.Hue;
                        bgcolor.Saturation += bg.Saturation;
                        bgcolor.Value += bg.Value;
                        outputColor = bgcolor.Color;

                        if (maskcolor.G > 0 && ChannelEnabled[0])
                        {
                            Color channelcolor = Color.FromArgb(1, channel1Row[pixelLocation + 2], channel1Row[pixelLocation + 1], channel1Row[pixelLocation]);
                            bgcolor = new MadScience.Colours.HSVColor(channelcolor);
                            bgcolor.Hue += Channel1bg.Hue;
                            bgcolor.Saturation += Channel1bg.Saturation;
                            bgcolor.Value += Channel1bg.Value;
                            outputColor = ColorOverlay(maskcolor.G, outputColor, bgcolor.Color);
                        }

                        if (maskcolor.B > 0 && ChannelEnabled[1])
                        {
                            Color channelcolor = Color.FromArgb(1, channel2Row[pixelLocation + 2], channel2Row[pixelLocation + 1], channel2Row[pixelLocation]);
                            bgcolor = new MadScience.Colours.HSVColor(channelcolor);
                            bgcolor.Hue += Channel2bg.Hue;
                            bgcolor.Saturation += Channel2bg.Saturation;
                            bgcolor.Value += Channel2bg.Value;
                            outputColor = ColorOverlay(maskcolor.B, outputColor, bgcolor.Color);
                        }

                        if (maskcolor.A > 0 && ChannelEnabled[2])
                        {
                            Color channelcolor = Color.FromArgb(1, channel3Row[pixelLocation + 2], channel3Row[pixelLocation + 1], channel3Row[pixelLocation]);
                            bgcolor = new MadScience.Colours.HSVColor(channelcolor);
                            bgcolor.Hue += Channel3bg.Hue;
                            bgcolor.Saturation += Channel3bg.Saturation;
                            bgcolor.Value += Channel3bg.Value;
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

            return output;

        }

        private static Color ColorOverlay(byte factor, Color colorA, Color colorB)
        {
            int cFactor = 255 - factor;
            return Color.FromArgb(colorA.A, ((int)colorA.R * (cFactor) + (int)colorB.R * factor) / 255, ((int)colorA.G * (cFactor) + (int)colorB.G * factor) / 255, ((int)colorA.B * (cFactor) + (int)colorB.B * factor) / 255);
        }

        private static void ResizeBitmap(ref Bitmap bitmap, int width, int height)
        {

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, 0, 0, width, height);

            bitmap = result;

        }

    }

}
