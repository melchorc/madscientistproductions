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
            Bitmap _Multiplier;
            Bitmap _PartMask;
            Bitmap output;

            Color color1 = Color.Empty;
            Color color2 = Color.Empty;
            Color color3 = Color.Empty;
            Color color4 = Color.Empty;

            if (tinta.enabled.ToLower() == "true") color1 = MadScience.Colours.convertColour(tinta.color);
            if (tintb.enabled.ToLower() == "true") color2 = MadScience.Colours.convertColour(tintb.color);
            if (tintc.enabled.ToLower() == "true") color3 = MadScience.Colours.convertColour(tintc.color);
            if (tintd.enabled.ToLower() == "true") color4 = MadScience.Colours.convertColour(tintd.color);

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
                                if (color4.IsEmpty) // if color4 is disabled we ignore the alpha channel of the mask
                                {
                                    if (useMask)
                                    {
                                        outputColor = ProcessMakeUpPixelRGB(multiplierColor, maskColor, color1, color2, color3);
                                    }
                                    else
                                    {
                                        outputColor = ProcessMakeUpPixelRGB(multiplierColor, Color.Red, color1, color2, color3);
                                    }
                                }
                                else
                                {
                                    outputColor = ProcessMakeUpPixelRGBA(multiplierColor, maskColor, color1, color2, color3, color4);
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

            return output;
        }

        public static Bitmap ProcessTexture(
                List<Stream> textures, 
                Bitmap[] Patterns,
                PointF[] Tilings
            )
        {
            int numPatterns = Patterns.Length;
            Bitmap _Multiplier;
            Bitmap _PartMask;
            Bitmap _Overlay = null;
            Bitmap output;

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
                if (_Overlay.Width != 1024 || _Overlay.Height != 1024)
                {
                    ResizeBitmap(ref _Overlay, 1024, 1024);
                }
            }

            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("Overlay generation time: " + duration.TotalMilliseconds);

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

            int[] PatternsWidth = new int[numPatterns];
            int[] PatternsHeight = new int[numPatterns];

            for (int i = 0; i < numPatterns; i++)
                if (Patterns[i] != null)
                {
                    PatternsWidth[i] = (int)(1024 / Tilings[i].X);
                    PatternsHeight[i] = (int)(1024 / Tilings[i].Y);
                    if (PatternsWidth[i] != Patterns[i].Width || PatternsHeight[i] != Patterns[i].Height) 
                        ResizeBitmap(ref Patterns[i], PatternsWidth[i], PatternsHeight[i]);
                }

            startTime = DateTime.Now;
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData multiplierData = _Multiplier.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData maskData = _PartMask.LockBits(new Rectangle(0, 0, 1024, 1024), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData[] patternData = new BitmapData[numPatterns];

            for (int i=0; i < numPatterns; i++)
                if (Patterns[i] != null)
                    patternData[i] = Patterns[i].LockBits(new Rectangle(0, 0, PatternsWidth[i], PatternsHeight[i]), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            
            stopTime = DateTime.Now;
            duration = stopTime - startTime;
            Console.WriteLine("LockBits time: " + duration.TotalMilliseconds);
            const int pixelSize = 4;

            byte[] byteMask = new byte[4];
            byteMask[0] = 2;
            byteMask[1] = 1;
            byteMask[2] = 0;
            byteMask[3] = 3;

            //process every pixel
            unsafe
            {
                startTime = DateTime.Now;
                    for (int y = 0; y < 1024; y++)
                    {
                        byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                        byte* multiplierRow = (byte*)multiplierData.Scan0 + (y * multiplierData.Stride);
                        byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);

                        byte*[] patternRows = new byte*[numPatterns];
                        for (int i=0; i < numPatterns; i++)
                            if (Patterns[i] != null)
                                patternRows[i] = (byte*)patternData[i].Scan0 + ((y % PatternsHeight[i]) * patternData[i].Stride);

                        for (int x = 0; x < 1024; x++)
                        {

                            int pixelLocation = x * pixelSize;

                            Color multiplierColor = Color.FromArgb(multiplierRow[pixelLocation + 3], multiplierRow[pixelLocation + 2], multiplierRow[pixelLocation + 1], multiplierRow[pixelLocation]);
                            if (multiplierColor.A != 0)
                            {
                                //Color maskColor = Color.FromArgb(maskRow[pixelLocation + 3], maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);

                                Color outColor = multiplierColor;
                                Color patternColor;
                                for (int i = 0; i < numPatterns; i++)
                                    if (Patterns[i] != null)
                                    {
                                        int xs = x % PatternsWidth[i];
                                        int pixelLocation2 = xs * pixelSize;
                                        patternColor = Color.FromArgb(patternRows[i][pixelLocation2 + 3], patternRows[i][pixelLocation2 + 2], patternRows[i][pixelLocation2 + 1], patternRows[i][pixelLocation2]);
                                        outColor = ColorOverlay(maskRow[pixelLocation + byteMask[i]], outColor, ColorMultiply(multiplierColor, patternColor));
                                    }
                                outputRow[pixelLocation] = (byte)outColor.B;
                                outputRow[pixelLocation + 1] = (byte)outColor.G;
                                outputRow[pixelLocation + 2] = (byte)outColor.R;
                                outputRow[pixelLocation + 3] = (byte)outColor.A;
                            }
                        }
                    }
                
                stopTime = DateTime.Now;
                duration = stopTime - startTime;
                Console.WriteLine("Loop time: " + duration.TotalMilliseconds);

            }
            output.UnlockBits(outputData);

            //apply overlay 
            if (_Overlay != null)
            {
                Graphics g = Graphics.FromImage(output);
                g.DrawImage(_Overlay, 0, 0);
                g.Dispose();
            }
            return output;
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
                            ResizeBitmap(ref _Stencil, 1024, 1024);
                        }
                        g.DrawImage(_Stencil, 0, 0);
                    }
                }
            }
            g.Dispose();
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

        private static Color ProcessPixelRGB(Color multiplier, Color mask, Color pattern1color, Color pattern2color, Color pattern3color)
        {
            Color output;
            output = multiplier;
            if (!pattern1color.IsEmpty) output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, pattern1color));
            if (!pattern2color.IsEmpty) output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, pattern2color));
            if (!pattern3color.IsEmpty) output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, pattern3color));
            return output;
        }

        private static Color ProcessPixelRGBA(Color multiplier, Color mask, Color pattern1color, Color pattern2color, Color pattern3color, Color pattern4color)
        {
            Color output = multiplier;
            if (!pattern1color.IsEmpty) output = ColorOverlay(mask.R, output, ColorMultiply(multiplier, pattern1color));
            if (!pattern2color.IsEmpty) output = ColorOverlay(mask.G, output, ColorMultiply(multiplier, pattern2color));
            if (!pattern3color.IsEmpty) output = ColorOverlay(mask.B, output, ColorMultiply(multiplier, pattern3color));
            if (!pattern4color.IsEmpty) output = ColorOverlay(mask.A, output, ColorMultiply(multiplier, pattern4color));
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
