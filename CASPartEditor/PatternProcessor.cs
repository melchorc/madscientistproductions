using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Collections.Generic;

namespace CASPartEditor
{
    public static class PatternProcessor
    {
        public static Bitmap ProcessMakeupTexture(
                List<Stream> textures,
                uint clothingType,
                MadScience.tintDetail tinta,
                MadScience.tintDetail tintb,
                MadScience.tintDetail tintc,
                MadScience.tintDetail tintd, 
                bool useMask
            )
        {
            Bitmap _FaceTexture;
            Bitmap _Multiplier;
            Bitmap _PartMask;
            Bitmap _Overlay;
            Bitmap output;

            Color color1 = Color.Empty;
            Color color2 = Color.Empty;
            Color color3 = Color.Empty;
            Color color4 = Color.Empty;

            if (tinta.enabled.ToLower() == "true") color1 = MadScience.Helpers.convertColour(tinta.color);
            if (tintb.enabled.ToLower() == "true") color2 = MadScience.Helpers.convertColour(tintb.color);
            if (tintc.enabled.ToLower() == "true") color3 = MadScience.Helpers.convertColour(tintc.color);
            if (tintd.enabled.ToLower() == "true") color4 = MadScience.Helpers.convertColour(tintd.color);

            var d = new DdsFileTypePlugin.DdsFile();

            Stream Multiplier = textures[0];
            Console.WriteLine("Multiplier length: " + Multiplier.Length.ToString());
            //If there is no multiplier return empty image
            DateTime startTime = DateTime.Now;
            if (Multiplier.Length == 0)
            {
                _Multiplier = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }
            else
            {

                //Load multiplier
                d.Load(Multiplier);
                _Multiplier = (Bitmap)d.Image(true, true, true, true);
                Multiplier.Close();
            }
            Stream PartMask = textures[1];
            Console.WriteLine("PartMask length: " + PartMask.Length.ToString());
            //Load partmask
            if ((PartMask.Length != 0))
            {
                d.Load(PartMask);
                _PartMask = (Bitmap)d.Image(true, true, true, true);
                PartMask.Close();
            }
            else
            {
                _PartMask = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }

            Stream Overlay = textures[2];


            //Load overlay
            if ((Overlay.Length != 0))
            {
                d.Load(Overlay);
                _Overlay = (Bitmap)d.Image(true, true, true, true);
                Overlay.Close();
            }
            else
            {
                _Overlay = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }

            //create empty output bitmap
            output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);

            //some error handling
            if (_PartMask.Width != 1024 || _PartMask.Height != 1024)
            {
                ResizeBitmap(ref _PartMask, 1024, 1024);
            }

            if (_Overlay.Width != 1024 || _Overlay.Height != 1024)
            {
                ResizeBitmap(ref _Overlay, 1024, 1024);
            }

            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData multiplierData = _Multiplier.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData maskData = _PartMask.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Color outputColor;
            const int pixelSize = 4;
            //process every pixel
            unsafe
            {
                for (int y = 0; y < 1024; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    byte* multiplierRow = (byte*)multiplierData.Scan0 + (y * multiplierData.Stride);
                    byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);
                    
                    for (int x = 0; x < 1024; x++)
                    {

                        int pixelLocation = x * pixelSize;

                            Color maskColor = Color.FromArgb(maskRow[pixelLocation + 3], maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);
                            Color multiplierColor = Color.FromArgb(multiplierRow[pixelLocation + 3], multiplierRow[pixelLocation + 2], multiplierRow[pixelLocation + 1], multiplierRow[pixelLocation]);
                            if (multiplierColor.A != 0)
                            {
                                if (useMask)
                                {
                                    outputColor = ProcessMakeUpPixelRGBA(multiplierColor, maskColor, color1, color2, color3, color4);
                                }
                                else
                                {
                                    outputColor = ProcessMakeUpPixelRGB(multiplierColor, Color.White, color1, color2, color3);
                                }
                                outputRow[pixelLocation] = (byte)outputColor.B;
                                outputRow[pixelLocation + 1] = (byte)outputColor.G;
                                outputRow[pixelLocation + 2] = (byte)outputColor.R;
                                outputRow[pixelLocation + 3] = (byte)outputColor.A;
                            }
                    }
                }


            }
            output.UnlockBits(outputData);

            //apply overlay and stencils
            Graphics g = Graphics.FromImage(output);
            g.DrawImage(output, 0, 0);
            g.DrawImage(_Overlay, 0, 0);
            //if ((textures[2] != null))
            //{
            //    d.Load(textures[2]);
            //    var _Stencil = d.Image(true, true, true, true);
            //    if (_Stencil.Width == 1024 || _Stencil.Height == 1024)
            //    {
            //        g.DrawImage(_Stencil, 0, 0);
            //    }
            //}


            return output;
        }

        public static Bitmap ProcessTexture(
                List<Stream> textures, 
                Color[] Pattern1Colors, 
                Color[] Pattern2Colors, 
                Color[] Pattern3Colors,
                Color[] Pattern4Colors, 
                List<Stream> Stencils,
                bool RGBA
            )
        {
            Bitmap _Multiplier;
            Bitmap _PartMask;
            Bitmap _Overlay;
            Bitmap _Pattern1;
            Bitmap _Pattern2;
            Bitmap _Pattern3;
            Bitmap _Pattern4;
            Bitmap output;

            // Pattern Colors check
            bool Pattern1Enabled = true;
            bool Pattern2Enabled = true;
            bool Pattern3Enabled = true;
            bool Pattern4Enabled = true;
            if (Pattern1Colors.Length == 1 && Pattern1Colors[0].IsEmpty)
            {
                Pattern1Enabled = false;
            }
            if (Pattern2Colors.Length == 1 && Pattern2Colors[0].IsEmpty)
            {
                Pattern2Enabled = false;
            }
            if (Pattern3Colors.Length == 1 && Pattern3Colors[0].IsEmpty)
            {
                Pattern3Enabled = false;
            }
            if (Pattern4Colors.Length == 1 && Pattern4Colors[0].IsEmpty)
            {
                Pattern4Enabled = false;
            }
            
            var d = new DdsFileTypePlugin.DdsFile();

            Stream Multiplier = textures[0];
            Console.WriteLine("Multiplier length: " + Multiplier.Length.ToString());
            //If there is no multiplier return empty image
            DateTime startTime = DateTime.Now;
            if (Multiplier.Length == 0)
            {
                _Multiplier = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }
            else
            {

                //Load multiplier
                d.Load(Multiplier);
                _Multiplier = (Bitmap)d.Image(true, true, true, true);
                Multiplier.Close();
            }
            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("Multiplier generation time: " + duration.TotalMilliseconds);

            startTime = DateTime.Now;
            Stream PartMask = textures[1];
            Console.WriteLine("Mask length: " + PartMask.Length.ToString());
            //Load partmask
            if ((PartMask.Length != 0))
            {
                d.Load(PartMask);
                _PartMask = (Bitmap)d.Image(true, true, true, true);
                PartMask.Close();
            }
            else
            {
                _PartMask = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Mask generation time: " + duration.TotalMilliseconds);

            startTime = DateTime.Now;
            Stream Overlay = textures[2];
            Console.WriteLine("Overlay length: " + Overlay.Length.ToString());

            //Load overlay
            if ((Overlay.Length != 0))
            {
                d.Load(Overlay);
                _Overlay = (Bitmap)d.Image(true, true, true, true);
                Overlay.Close();
            }
            else
            {
                _Overlay = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Overlay generation time: " + duration.TotalMilliseconds);


            startTime = DateTime.Now;
            Stream Pattern1 = textures[3];
            Console.WriteLine("Pattern1 length: " + Pattern1.Length.ToString());

            //Load patterns
            if ((Pattern1.Length != 0))
            {
                d.Load(Pattern1);
                _Pattern1 = (Bitmap)d.Image(true, true, true, true);
                Pattern1.Close();
            }
            else
            {
                _Pattern1 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Pattern1 generation time: " + duration.TotalMilliseconds);

            startTime = DateTime.Now;
            Stream Pattern2 = textures[4];
            Console.WriteLine("Pattern2 length: " + Pattern2.Length.ToString());

            if ((Pattern2.Length != 0))
            {
                d.Load(Pattern2);
                _Pattern2 = (Bitmap)d.Image(true, true, true, true);
                Pattern2.Close();
            }

            else
            {
                _Pattern2 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Pattern2 generation time: " + duration.TotalMilliseconds);

            startTime = DateTime.Now;
            Stream Pattern3 = textures[5];
            Console.WriteLine("Pattern3 length: " + Pattern3.Length.ToString());

            if ((Pattern3.Length != 0))
            {
                d.Load(Pattern3);
                _Pattern3 = (Bitmap)d.Image(true, true, true, true);
                Pattern3.Close();
            }
            else
            {
                _Pattern3 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Pattern3 generation time: " + duration.TotalMilliseconds);

            startTime = DateTime.Now;
            Stream Pattern4 = textures[6];
            Console.WriteLine("Pattern4 length: " + Pattern4.Length.ToString());

            if ((Pattern4.Length != 0))
            {
                d.Load(Pattern4);
                _Pattern4 = (Bitmap)d.Image(true, true, true, true);
                Pattern4.Close();
            }
            else
            {
                _Pattern4 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            }
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Pattern4 generation time: " + duration.TotalMilliseconds);

            //create empty output bitmap
            output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);

            //some error handling
            if (_Multiplier.Width != 1024 || _Multiplier.Height != 1024)
            {
                ResizeBitmap(ref _Multiplier, 1024, 1024);
            }

            if (_PartMask.Width != 1024 || _PartMask.Height != 1024)
            {
                ResizeBitmap(ref _PartMask, 1024, 1024);
            }

            if (_Overlay.Width != 1024 || _Overlay.Height != 1024)
            {
                ResizeBitmap(ref _Overlay, 1024, 1024);
            }

            if (_Pattern1.Width != 256 || _Pattern1.Height != 256)
            {
                ResizeBitmap(ref _Pattern1, 256, 256);
            }

            if (_Pattern2.Width != 256 || _Pattern2.Height != 256)
            {
                ResizeBitmap(ref _Pattern2, 256, 256);
            }

            if (_Pattern3.Width != 256 || _Pattern3.Height != 256)
            {
                ResizeBitmap(ref _Pattern3, 256, 256);
            }

            if (_Pattern4.Width != 256 || _Pattern4.Height != 256)
            {
                ResizeBitmap(ref _Pattern4, 256, 256);
            }

            startTime = DateTime.Now;
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData multiplierData = _Multiplier.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData maskData = _PartMask.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData pattern1Data = _Pattern1.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData pattern2Data = _Pattern2.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData pattern3Data = _Pattern3.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData pattern4Data = _Pattern4.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("LockBits time: " + duration.TotalMilliseconds);
            const int pixelSize = 4;
            //process every pixel
            unsafe
            {
                startTime = DateTime.Now;
                    for (int y = 0; y < 1024; y++)
                    {
                        byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                        byte* multiplierRow = (byte*)multiplierData.Scan0 + (y * multiplierData.Stride);
                        byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);

                        byte* pattern1Row = null;
                        byte* pattern2Row = null;
                        byte* pattern3Row = null;
                        byte* pattern4Row = null;

                        if (Pattern1Enabled)
                        {
                            pattern1Row = (byte*)pattern1Data.Scan0 + ((y % 256) * pattern1Data.Stride);
                        }
                        if (Pattern2Enabled)
                        {
                            pattern2Row = (byte*)pattern2Data.Scan0 + ((y % 256) * pattern2Data.Stride);
                        }
                        if (Pattern3Enabled)
                        {
                            pattern3Row = (byte*)pattern3Data.Scan0 + ((y % 256) * pattern3Data.Stride);
                        }
                        if (Pattern4Enabled)
                        {
                            pattern4Row = (byte*)pattern4Data.Scan0 + ((y % 256) * pattern4Data.Stride);
                        }

                        for (int x = 0; x < 1024; x++)
                        {

                            int pixelLocation = x * pixelSize;

                            Color multiplierColor = Color.FromArgb(multiplierRow[pixelLocation + 3], multiplierRow[pixelLocation + 2], multiplierRow[pixelLocation + 1], multiplierRow[pixelLocation]);
                            if (multiplierColor.A != 0)
                            {
                                Color maskColor = Color.FromArgb(maskRow[pixelLocation + 3], maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);

                                int xs = x % 256;
                                int pixelLocation2 = xs * pixelSize;

                                Color pattern1Color = Color.Empty;
                                Color pattern2Color = Color.Empty;
                                Color pattern3Color = Color.Empty;
                                Color pattern4Color = Color.Empty;

                                if (Pattern1Enabled) pattern1Color = Color.FromArgb(pattern1Row[pixelLocation2 + 3], pattern1Row[pixelLocation2 + 2], pattern1Row[pixelLocation2 + 1], pattern1Row[pixelLocation2]);
                                if (Pattern2Enabled) pattern2Color = Color.FromArgb(pattern2Row[pixelLocation2 + 3], pattern2Row[pixelLocation2 + 2], pattern2Row[pixelLocation2 + 1], pattern2Row[pixelLocation2]);
                                if (Pattern3Enabled) pattern3Color = Color.FromArgb(pattern3Row[pixelLocation2 + 3], pattern3Row[pixelLocation2 + 2], pattern3Row[pixelLocation2 + 1], pattern3Row[pixelLocation2]);
                                if (RGBA && Pattern4Enabled) pattern4Color = Color.FromArgb(pattern4Row[pixelLocation2 + 3], pattern4Row[pixelLocation2 + 2], pattern4Row[pixelLocation2 + 1], pattern4Row[pixelLocation2]);

                                if (RGBA)
                                {
                                    multiplierColor = ProcessPixelRGBA(multiplierColor, maskColor, pattern1Color, pattern2Color, pattern3Color, pattern4Color, Pattern1Colors, Pattern2Colors, Pattern3Colors, Pattern4Colors);
                                }
                                else
                                {
                                    multiplierColor = ProcessPixelRGB(multiplierColor, maskColor, pattern1Color, pattern2Color, pattern3Color, Pattern1Colors, Pattern2Colors, Pattern3Colors);
                                }
                                outputRow[pixelLocation] = (byte)multiplierColor.B;
                                outputRow[pixelLocation + 1] = (byte)multiplierColor.G;
                                outputRow[pixelLocation + 2] = (byte)multiplierColor.R;
                                outputRow[pixelLocation + 3] = (byte)multiplierColor.A;
                            }
                        }
                    }
                
                stopTime = DateTime.Now;
                duration = stopTime - startTime;
                Console.WriteLine("Loop time: " + duration.TotalMilliseconds);

            }
            output.UnlockBits(outputData);

            //apply overlay and stencils
            Graphics g = Graphics.FromImage(output);
            g.DrawImage(_Overlay, 0, 0);
            if ((Stencils != null))
            {
                for (int i = 0; i < Stencils.Count; i++)
                {
                    if (Stencils[i].Length != 0)
                    {
                        d.Load(Stencils[i]);
                        var _Stencil = d.Image(true, true, true, true);
                        Stencils[i].Close();
                        if (_Stencil.Width == 1024 || _Stencil.Height == 1024)
                        {
                            g.DrawImage(_Stencil, 0, 0);
                        }
                    }
                }
                 
            }
            return output;
        }


        private static void ResizeBitmap(ref Bitmap bitmap, int width, int height)
        {

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, 0, 0, width, height);

            bitmap = result;

        }

        private static Color ProcessMakeUpPixelRGB(Color multiplier, Color mask, Color color1, Color color2, Color color3)
        {
            Color output = multiplier;
            output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, color1));
            output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, color2));
            output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, color3));
            return output;
        }

        private static Color ProcessMakeUpPixelRGBA(Color multiplier, Color mask, Color color1, Color color2, Color color3, Color color4)
        {
            Color output = multiplier;
            output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, color1));
            output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, color2));
            output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, color3));
            output = ColorOverlay(mask.A, output, ColorMultiply(multiplier, color4));

            return output;
        }

        private static Color ProcessPixelRGB(Color multiplier, Color mask, Color pattern1mask, Color pattern2mask, Color pattern3mask, Color[] colors1, Color[] colors2, Color[] colors3)
        {
            Color output;
            output = multiplier;
            if (!pattern1mask.IsEmpty) output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, ColorFromPattern(pattern1mask, colors1)));
            if (!pattern2mask.IsEmpty) output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, ColorFromPattern(pattern2mask, colors2)));
            if (!pattern3mask.IsEmpty) output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, ColorFromPattern(pattern3mask, colors3)));
            return output;
        }

        private static Color ProcessPixelRGBA(Color multiplier, Color mask, Color pattern1mask, Color pattern2mask, Color pattern3mask, Color pattern4mask, Color[] colors1, Color[] colors2, Color[] colors3, Color[] colors4)
        {
            Color output = multiplier;
            if (!pattern1mask.IsEmpty) output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, ColorFromPattern(pattern1mask, colors1)));
            if (!pattern2mask.IsEmpty) output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, ColorFromPattern(pattern2mask, colors2)));
            if (!pattern3mask.IsEmpty) output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, ColorFromPattern(pattern3mask, colors3)));
            if (!pattern4mask.IsEmpty) output = ColorOverlay(mask.A, output, ColorMultiply(multiplier, ColorFromPattern(pattern4mask, colors4)));
            return output;
        }

        private static Color ColorMultiply(Color colorA, Color colorB)
        {
            return Color.FromArgb(colorA.A, ((int)colorA.R * colorB.R) / 255, ((int)colorA.G * colorB.G) / 255, ((int)colorA.B * colorB.B) / 255);
        }

        private static Color ColorOverlay(byte factor, Color colorA, Color colorB)
        {
            int cFactor = 255 - factor;
            return Color.FromArgb(colorA.A, ((int)colorA.R * (cFactor) + (int)colorB.R * factor) / 255, ((int)colorA.G * (cFactor) + (int)colorB.G * factor) / 255, ((int)colorA.B * (cFactor) + (int)colorB.B * factor) / 255);
        }

        private static Color ColorPaint(byte factor, Color colorA, Color colorB)
        {
            if (factor == 0)
                return colorA;
            float fFactor = (float)factor / 255;
            float fIFactor = 1-(fFactor);
            float a = factor + colorA.A * fIFactor;
            float r = (((fIFactor * colorA.R * colorA.A)) / 255 + (fFactor * colorB.R)) / (a / 255);
            float g = (((fIFactor * colorA.B * colorA.A)) / 255 + (fFactor * colorB.G)) / (a / 255);
            float b = (((fIFactor * colorA.G * colorA.A)) / 255 + (fFactor * colorB.B)) / (a / 255);

            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }

        private static Color ColorFromPattern(Color mask, Color[] colors)
        {
            Color output = colors[0];
            if (colors.Length > 1)
            {
                if (colors[1] != Color.Empty) { output = ColorOverlay(mask.R, output, colors[1]); }
                if (colors[2] != Color.Empty) { output = ColorOverlay(mask.G, output, colors[2]); }
                if (colors[3] != Color.Empty) { output = ColorOverlay(mask.B, output, colors[3]); }
                if (colors[4] != Color.Empty) { output = ColorOverlay(mask.A, output, colors[4]); }
                /*
                if (colors.Length > 2)
                {
                    output = ColorOverlay(mask.G, output, colors[2]);
                    if (colors.Length > 3)
                    {
                        output = ColorOverlay(mask.B, output, colors[3]);
                        if (colors.Length > 4)
                            output = ColorOverlay(mask.A, output, colors[4]);
                    }
                }
                */
            }
            return output;
        }
    }
}
