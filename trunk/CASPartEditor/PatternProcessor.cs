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

    #region Makeup
        public static Bitmap ProcessMakeupTexture(
                List<Stream> textures,
                MadScience.tintDetail tinta,
                MadScience.tintDetail tintb,
                MadScience.tintDetail tintc,
                MadScience.tintDetail tintd
            )
        {
            Bitmap baseTexture = MadScience.Patterns.LoadBitmapFromStream(textures[0]);
            Bitmap output;

            if (baseTexture != null)
            {
                output = new Bitmap(baseTexture.Width, baseTexture.Height, PixelFormat.Format32bppArgb);

                Bitmap mask = MadScience.Patterns.LoadBitmapFromStream(textures[1], baseTexture.Width, baseTexture.Height);

                Color color1 = Color.Empty;
                Color color2 = Color.Empty;
                Color color3 = Color.Empty;
                Color color4 = Color.Empty;

                if (tinta.enabled.ToLower() == "true") color1 = MadScience.Colours.convertColour(tinta.color);
                if (tintb.enabled.ToLower() == "true") color2 = MadScience.Colours.convertColour(tintb.color);
                if (tintc.enabled.ToLower() == "true") color3 = MadScience.Colours.convertColour(tintc.color);
                if (tintd.enabled.ToLower() == "true") color4 = MadScience.Colours.convertColour(tintd.color);

                BitmapData outputData = output.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData baseTextureData = baseTexture.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData maskData = mask.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                Color outputColor;
                const int pixelSize = 4;
                //process every pixel
                unsafe
                {
                    for (int y = 0; y < baseTexture.Height; y++)
                    {
                        byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                        byte* baseTextureRow = (byte*)baseTextureData.Scan0 + (y * baseTextureData.Stride);
                        byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);

                        for (int x = 0; x < baseTexture.Width; x++)
                        {

                            int pixelLocation = x * pixelSize;

                            Color maskColor = Color.FromArgb(maskRow[pixelLocation + 3], maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);
                            Color baseColor = Color.FromArgb(baseTextureRow[pixelLocation + 3], baseTextureRow[pixelLocation + 2], baseTextureRow[pixelLocation + 1], baseTextureRow[pixelLocation]);
                            if (baseColor.A != 0)
                            {
                                if (color4.IsEmpty) // if color4 is disabled we ignore the alpha channel of the mask
                                {
                                    outputColor = MadScience.Patterns.ProcessMakeUpPixelRGB(baseColor, maskColor, color1, color2, color3);
                                }
                                else
                                {
                                    outputColor = MadScience.Patterns.ProcessMakeUpPixelRGBA(baseColor, maskColor, color1, color2, color3, color4);
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
                mask.UnlockBits(maskData);
                baseTexture.UnlockBits(baseTextureData);
            }
            else
            {
                output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }

            //apply overlay 
            Bitmap overlay = MadScience.Patterns.LoadBitmapFromStream(textures[2], output.Width, output.Height);
            if (overlay != null)
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.DrawImageUnscaled(overlay, 0, 0);
                }
            }
            return output;
        }

        public static Bitmap ProcessMakeupTexture(
        List<Stream> textures, Color tintColor)
        {
            Bitmap baseTexture = MadScience.Patterns.LoadBitmapFromStream(textures[0]);
            Bitmap output;

            if (baseTexture != null)
            {
                output = new Bitmap(baseTexture.Width, baseTexture.Height, PixelFormat.Format32bppArgb);

                BitmapData outputData = output.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData baseTextureData = baseTexture.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                Color outputColor;
                const int pixelSize = 4;
                //process every pixel
                unsafe
                {
                    for (int y = 0; y < baseTexture.Height; y++)
                    {
                        byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                        byte* baseTextureRow = (byte*)baseTextureData.Scan0 + (y * baseTextureData.Stride);

                        for (int x = 0; x < baseTexture.Width; x++)
                        {

                            int pixelLocation = x * pixelSize;

                            Color baseColor = Color.FromArgb(baseTextureRow[pixelLocation + 3], baseTextureRow[pixelLocation + 2], baseTextureRow[pixelLocation + 1], baseTextureRow[pixelLocation]);
                            if (baseColor.A != 0)
                            {
                                outputColor = MadScience.Patterns.ColorMultiply(baseColor, tintColor);
                                outputRow[pixelLocation] = (byte)outputColor.B;
                                outputRow[pixelLocation + 1] = (byte)outputColor.G;
                                outputRow[pixelLocation + 2] = (byte)outputColor.R;
                                outputRow[pixelLocation + 3] = (byte)outputColor.A;
                            }
                        }
                    }
                }
                output.UnlockBits(outputData);
                baseTexture.UnlockBits(baseTextureData);
            }
            else
            {
                output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }
            //apply overlay 
            Bitmap overlay = MadScience.Patterns.LoadBitmapFromStream(textures[2], output.Width, output.Height);
            if (overlay != null)
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.DrawImageUnscaled(overlay, 0, 0);
                }
            }
            return output;
        }


    #endregion
    #region Hair
        public static Bitmap ProcessHairTexture(
                List<Stream> textures,
                Color color1,
                Color color2,
                Color color3,
                Color color4)
        {
            Bitmap baseTexture = MadScience.Patterns.LoadBitmapFromStream(textures[0]);
            Bitmap output;

            if (baseTexture != null)
            {
                output = new Bitmap(baseTexture.Width, baseTexture.Height, PixelFormat.Format32bppArgb);

                Bitmap mask = MadScience.Patterns.LoadBitmapFromStream(textures[1], baseTexture.Width, baseTexture.Height);

                BitmapData outputData = output.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData baseTextureData = baseTexture.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData maskData = mask.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                Color outputColor;
                const int pixelSize = 4;
                //process every pixel
                unsafe
                {
                    for (int y = 0; y < baseTexture.Height; y++)
                    {
                        byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                        byte* baseTextureRow = (byte*)baseTextureData.Scan0 + (y * baseTextureData.Stride);
                        byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);

                        for (int x = 0; x < baseTexture.Width; x++)
                        {

                            int pixelLocation = x * pixelSize;

                            Color maskColor = Color.FromArgb(maskRow[pixelLocation + 3], maskRow[pixelLocation + 2], maskRow[pixelLocation + 1], maskRow[pixelLocation]);
                            Color baseColor = Color.FromArgb(baseTextureRow[pixelLocation + 3], baseTextureRow[pixelLocation + 2], baseTextureRow[pixelLocation + 1], baseTextureRow[pixelLocation]);
                            if (baseColor.A != 0)
                            {
                                outputColor = MadScience.Patterns.ProcessHairPixelRGB(baseColor, maskColor, color1, color2, color3, color4);

                                outputRow[pixelLocation] = (byte)outputColor.B;
                                outputRow[pixelLocation + 1] = (byte)outputColor.G;
                                outputRow[pixelLocation + 2] = (byte)outputColor.R;
                                outputRow[pixelLocation + 3] = (byte)outputColor.A;
                            }
                        }
                    }
                }
                output.UnlockBits(outputData);
                mask.UnlockBits(maskData);
                baseTexture.UnlockBits(baseTextureData);
            }
            else
            {
                output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }
            //apply overlay 
            Bitmap overlay = MadScience.Patterns.LoadBitmapFromStream(textures[2], output.Width, output.Height);
            if (overlay != null)
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.DrawImageUnscaled(overlay, 0, 0);
                }
            }
            return output;
        }

    #endregion
    #region Clothing

        public static Bitmap ProcessClothingTexture(
                List<Stream> textures, 
                Bitmap[] Patterns,
                PointF[] Tilings)
        {
            int numPatterns = Patterns.Length;
            Bitmap baseTexture = MadScience.Patterns.LoadBitmapFromStream(textures[0]);
            Bitmap output;

            if (baseTexture != null)
            {
                output = new Bitmap(baseTexture.Width, baseTexture.Height, PixelFormat.Format32bppArgb);

                Bitmap mask = MadScience.Patterns.LoadBitmapFromStream(textures[1], baseTexture.Width, baseTexture.Height);

                var d = new DdsFileTypePlugin.DdsFile();

                int[] PatternsWidth = new int[numPatterns];
                int[] PatternsHeight = new int[numPatterns];

                for (int i = 0; i < numPatterns; i++)
                    if (Patterns[i] != null)
                    {
                        PatternsWidth[i] = (int)(baseTexture.Width / Tilings[i].X);
                        PatternsHeight[i] = (int)(baseTexture.Height / Tilings[i].Y);
                        if (PatternsWidth[i] != Patterns[i].Width || PatternsHeight[i] != Patterns[i].Height)
                            MadScience.Patterns.ResizeBitmapFast(ref Patterns[i], PatternsWidth[i], PatternsHeight[i]);
                    }

                BitmapData outputData = output.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData baseTextureData = baseTexture.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData maskData = mask.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData[] patternData = new BitmapData[numPatterns];

                for (int i = 0; i < numPatterns; i++)
                    if (Patterns[i] != null)
                        patternData[i] = Patterns[i].LockBits(new Rectangle(0, 0, PatternsWidth[i], PatternsHeight[i]), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                const int pixelSize = 4;

                byte[] byteMask = new byte[4];
                byteMask[0] = 2;
                byteMask[1] = 1;
                byteMask[2] = 0;
                byteMask[3] = 3;

                //process every pixel
                unsafe
                {
                    for (int y = 0; y < baseTexture.Height; y++)
                    {
                        byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                        byte* baseTextureRow = (byte*)baseTextureData.Scan0 + (y * baseTextureData.Stride);
                        byte* maskRow = (byte*)maskData.Scan0 + (y * maskData.Stride);

                        byte*[] patternRows = new byte*[numPatterns];
                        for (int i = 0; i < numPatterns; i++)
                            if (Patterns[i] != null)
                                patternRows[i] = (byte*)patternData[i].Scan0 + ((y % PatternsHeight[i]) * patternData[i].Stride);

                        for (int x = 0; x < baseTexture.Width; x++)
                        {
                            int pixelLocation = x * pixelSize;

                            Color baseColor = Color.FromArgb(baseTextureRow[pixelLocation + 3], baseTextureRow[pixelLocation + 2], baseTextureRow[pixelLocation + 1], baseTextureRow[pixelLocation]);
                            if (baseColor.A != 0)
                            {
                                Color outColor = baseColor;

                                for (int i = 0; i < numPatterns; i++)
                                    if (Patterns[i] != null)
                                    {
                                        int xs = x % PatternsWidth[i];
                                        int pixelLocation2 = xs * pixelSize;
                                        Color patternColor = Color.FromArgb(patternRows[i][pixelLocation2 + 3], patternRows[i][pixelLocation2 + 2], patternRows[i][pixelLocation2 + 1], patternRows[i][pixelLocation2]);
                                        outColor = MadScience.Patterns.ColorOverlay(maskRow[pixelLocation + byteMask[i]], outColor, MadScience.Patterns.ColorMultiply(baseColor, patternColor));
                                    }
                                outputRow[pixelLocation] = (byte)outColor.B;
                                outputRow[pixelLocation + 1] = (byte)outColor.G;
                                outputRow[pixelLocation + 2] = (byte)outColor.R;
                                outputRow[pixelLocation + 3] = (byte)outColor.A;
                            }
                        }
                    }
                }
                output.UnlockBits(outputData);
                mask.UnlockBits(maskData);
                baseTexture.UnlockBits(baseTextureData);
            }
            else
            {
                output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }

            //apply overlay 
            Bitmap overlay = MadScience.Patterns.LoadBitmapFromStream(textures[2], baseTexture.Width, baseTexture.Height);
            if (overlay != null)
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.DrawImageUnscaled(overlay, 0, 0);
                }
            }
            return output;
        }

        public static Bitmap ProcessSingleChannelTexture(
                List<Stream> textures,
                Bitmap pattern,
                PointF tiling)
        {
            Bitmap baseTexture = MadScience.Patterns.LoadBitmapFromStream(textures[0]);
            Bitmap output;

            if (baseTexture != null)
            {
                output = new Bitmap(baseTexture.Width, baseTexture.Height, PixelFormat.Format32bppArgb);

                var d = new DdsFileTypePlugin.DdsFile();
                int patternWidth = (int)(baseTexture.Width / tiling.X);
                int patternHeight = (int)(baseTexture.Height / tiling.Y);

                if (pattern != null)
                {
                    BitmapData outputData = output.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    BitmapData baseTextureData = baseTexture.LockBits(new Rectangle(0, 0, baseTexture.Width, baseTexture.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    BitmapData patternData = pattern.LockBits(new Rectangle(0, 0, patternWidth, patternHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    const int pixelSize = 4;

                    byte[] byteMask = new byte[4];
                    byteMask[0] = 2;
                    byteMask[1] = 1;
                    byteMask[2] = 0;
                    byteMask[3] = 3;

                    //process every pixel
                    unsafe
                    {
                        for (int y = 0; y < baseTexture.Height; y++)
                        {
                            byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                            byte* baseTextureRow = (byte*)baseTextureData.Scan0 + (y * baseTextureData.Stride);
                            byte* patternRow = (byte*)patternData.Scan0 + ((y % patternHeight) * patternData.Stride);

                            for (int x = 0; x < baseTexture.Width; x++)
                            {
                                int pixelLocation = x * pixelSize;

                                Color baseColor = Color.FromArgb(baseTextureRow[pixelLocation + 3], baseTextureRow[pixelLocation + 2], baseTextureRow[pixelLocation + 1], baseTextureRow[pixelLocation]);
                                if (baseColor.A != 0)
                                {
                                    Color outColor = baseColor;
                                    Color patternColor;
                                    if (pattern != null)
                                    {
                                        int xs = x % patternWidth;
                                        int pixelLocation2 = xs * pixelSize;
                                        patternColor = Color.FromArgb(patternRow[pixelLocation2 + 3], patternRow[pixelLocation2 + 2], patternRow[pixelLocation2 + 1], patternRow[pixelLocation2]);
                                        outColor = MadScience.Patterns.ColorMultiply(baseColor, patternColor);
                                    }
                                    outputRow[pixelLocation] = (byte)outColor.B;
                                    outputRow[pixelLocation + 1] = (byte)outColor.G;
                                    outputRow[pixelLocation + 2] = (byte)outColor.R;
                                    outputRow[pixelLocation + 3] = (byte)outColor.A;
                                }
                            }
                        }
                    }
                    output.UnlockBits(outputData);
                    baseTexture.UnlockBits(baseTextureData);
                }
                else
                {
                    output = baseTexture;
                }
            }
            else
            {
                output = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            }

            //apply overlay 
            Bitmap overlay = MadScience.Patterns.LoadBitmapFromStream(textures[2], output.Width, output.Height);
            if (overlay != null)
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.DrawImageUnscaled(overlay, 0, 0);
                }
            }
            return output;
        }


        #endregion


    }


}
