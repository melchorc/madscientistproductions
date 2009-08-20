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
                    DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
                    Colours.HSVColor bg = new Colours.HSVColor(double.Parse(pattern.HBg) * 360, double.Parse(pattern.SBg), double.Parse(pattern.VBg));
                    Colours.HSVColor basebg = new Colours.HSVColor(double.Parse(pattern.HBg) * 360, double.Parse(pattern.SBg), double.Parse(pattern.VBg));
                    Colours.HSVColor shift = new Colours.HSVColor(double.Parse(pattern.HBg) * 360, double.Parse(pattern.SBg), double.Parse(pattern.VBg));
                    temp = Patterns.createHSVPattern(KeyUtils.findKey(pattern.BackgroundImage), bg, basebg, shift);
                }
                if (pattern.type == "Coloured")
                {
                    DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
                    Color bgColor = Colours.convertColour(pattern.ColorP[0], true);
                    if (bgColor == Color.Empty)
                    {
                        bgColor = Color.Black;
                    }
                    ddsP.Load(KeyUtils.findKey(pattern.rgbmask));
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

                            xtr.MoveToAttribute("name");
                            pDetail.name = xtr.Value;
                            xtr.MoveToAttribute("category");
                            pDetail.category = xtr.Value;

                            xtr.MoveToAttribute("reskey");
                            pDetail.key = xtr.Value;

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
                                        xtr.MoveToAttribute("default");
                                        pDetail.assetRoot = xtr.Value;
                                        break;
                                    case "Color":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Color = xtr.Value;
                                        break;

                                    case "Color 0":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[0] = xtr.Value;
                                        break;
                                    case "Color 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[1] = xtr.Value;
                                        break;
                                    case "Color 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[2] = xtr.Value;
                                        break;
                                    case "Color 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[3] = xtr.Value;
                                        break;
                                    case "Color 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[4] = xtr.Value;
                                        break;

                                    case "Channel 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[0] = xtr.Value;
                                        break;
                                    case "Channel 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[1] = xtr.Value;
                                        break;
                                    case "Channel 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[2] = xtr.Value;
                                        break;
                                    case "Channel 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[3] = xtr.Value;
                                        break;

                                    case "Channel 1 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[0] = xtr.Value;
                                        break;
                                    case "Channel 2 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[1] = xtr.Value;
                                        break;
                                    case "Channel 3 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[2] = xtr.Value;
                                        break;
                                    case "Channel 4 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[3] = xtr.Value;
                                        break;


                                    case "Background Image":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BackgroundImage = xtr.Value;
                                        if (patternTexture == "") patternTexture = xtr.Value;
                                        break;

                                    case "H Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HBg = xtr.Value;
                                        break;
                                    case "H 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[0] = xtr.Value;
                                        break;
                                    case "H 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[1] = xtr.Value;
                                        break;
                                    case "H 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[2] = xtr.Value;
                                        break;
                                    case "H 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[3] = xtr.Value;
                                        break;

                                    case "S Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.SBg = xtr.Value;
                                        break;
                                    case "S 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[0] = xtr.Value;
                                        break;
                                    case "S 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[1] = xtr.Value;
                                        break;
                                    case "S 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[2] = xtr.Value;
                                        break;
                                    case "S 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[3] = xtr.Value;
                                        break;

                                    case "V Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.VBg = xtr.Value;
                                        break;
                                    case "V 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[0] = xtr.Value;
                                        break;
                                    case "V 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[1] = xtr.Value;
                                        break;
                                    case "V 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[2] = xtr.Value;
                                        break;
                                    case "V 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[3] = xtr.Value;
                                        break;


                                    case "Base H Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseHBg = xtr.Value;
                                        break;
                                    case "Base H 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[0] = xtr.Value;
                                        break;
                                    case "Base H 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[1] = xtr.Value;
                                        break;
                                    case "Base H 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[2] = xtr.Value;
                                        break;
                                    case "Base H 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[3] = xtr.Value;
                                        break;

                                    case "Base S Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseSBg = xtr.Value;
                                        break;
                                    case "Base S 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[0] = xtr.Value;
                                        break;
                                    case "Base S 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[1] = xtr.Value;
                                        break;
                                    case "Base S 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[2] = xtr.Value;
                                        break;
                                    case "Base S 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[3] = xtr.Value;
                                        break;

                                    case "Base V Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseVBg = xtr.Value;
                                        break;
                                    case "Base V 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[0] = xtr.Value;
                                        break;
                                    case "Base V 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[1] = xtr.Value;
                                        break;
                                    case "Base V 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[2] = xtr.Value;
                                        break;
                                    case "Base V 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[3] = xtr.Value;
                                        break;

                                    case "HSVShift Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShiftBg = xtr.Value;
                                        break;
                                    case "HSVShift 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[0] = xtr.Value;
                                        break;
                                    case "HSVShift 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[1] = xtr.Value;
                                        break;
                                    case "HSVShift 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[2] = xtr.Value;
                                        break;
                                    case "HSVShift 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[3] = xtr.Value;
                                        break;

                                    case "rgbmask":
                                        xtr.MoveToAttribute("default");
                                        pDetail.rgbmask = xtr.Value;
                                        if (patternTexture == "") patternTexture = xtr.Value;
                                        break;
                                    case "specmap":
                                        xtr.MoveToAttribute("default");
                                        pDetail.specmap = xtr.Value;
                                        break;
                                    case "filename":
                                        xtr.MoveToAttribute("default");
                                        pDetail.filename = xtr.Value;
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

        public static Bitmap createHSVPattern(Stream texture, MadScience.Colours.HSVColor bg, MadScience.Colours.HSVColor basebg, MadScience.Colours.HSVColor shift)
        {
            Bitmap input;
            Bitmap output;
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
            output = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData inputData = input.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Color outputColor;
            const int pixelSize = 4;
            //process every pixel
            unsafe
            {
                for (int y = 0; y < 256; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    byte* inputRow = (byte*)inputData.Scan0 + (y * inputData.Stride);

                    for (int x = 0; x < 256; x++)
                    {

                        int pixelLocation = x * pixelSize;

                        Color inputcolor = Color.FromArgb(1, inputRow[pixelLocation + 2], inputRow[pixelLocation + 1], inputRow[pixelLocation]);

                        MadScience.Colours.HSVColor a = new MadScience.Colours.HSVColor(inputcolor);
                        a.Hue += bg.Hue;
                        a.Saturation += bg.Saturation;
                        a.Value += bg.Value;

                        outputColor = a.Color;
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

        private static void ResizeBitmap(ref Bitmap bitmap, int width, int height)
        {

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, 0, 0, width, height);

            bitmap = result;

        }

    }

}
