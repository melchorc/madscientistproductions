using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Collections.Generic;

namespace PatternBrowser
{
    static class HSVPatternProcessor
    {
        public static Bitmap createHSVPattern(Stream texture, HSVColor bg, HSVColor basebg, HSVColor shift)
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

                        HSVColor a = new HSVColor(inputcolor);
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

    public struct HSVColor
    {
        double m_hue;
        double m_saturation;
        double m_value;

        public double Hue
        {
            get { return m_hue; }
            set { 
                m_hue = value; 
                while (m_hue > 360) m_hue -= 360;
                while (m_hue < 0) m_hue += 360;
            }
        }
        public double Saturation
        {
            get { return m_saturation; }
            set { 
                m_saturation = value;
            }
        }
        public double Value
        {
            get { return m_value; }
            set
            {
                m_value = value;
            }
        }
        public HSVColor(double hue, double saturation, double value)
        {
            m_hue =hue;
                            while (m_hue > 360) m_hue -= 360;
                while (m_hue < 0) m_hue += 360;
            m_saturation = Math.Min(1, saturation);
            m_value = Math.Min(1, value);
        }
        public HSVColor(Color color)
        {
            m_hue = 0;
            m_saturation = 1;
            m_value = 1;
            FromRGB(color);
        }
        public Color Color
        {
            get { return ToRGB(); }
            set { FromRGB(value); }
        }
        void FromRGB(Color cc)
        {
            double r = (double)cc.R / 255d;
            double g = (double)cc.G / 255d;
            double b = (double)cc.B / 255d;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            // calulate hue according formula given in
            // "Conversion from RGB to HSL or HSV"
            m_hue = 0;
            if (min != max)
            {
                if (r == max && g >= b)
                {
                    m_hue = 60 * ((g - b) / (max - min)) + 0;
                }
                else
                    if (r == max && g < b)
                    {
                        m_hue = 60 * ((g - b) / (max - min)) + 360;
                    }
                    else
                        if (g == max)
                        {
                            m_hue = 60 * ((b - r) / (max - min)) + 120;
                        }
                        else
                            if (b == max)
                            {
                                m_hue = 60 * ((r - g) / (max - min)) + 240;
                            }
            }
            // find value
            m_value = max;

            // find saturation
            if (max == 0)
                m_saturation = 0;
            else
                m_saturation = (max - min) / max;

        }
        Color ToRGB()
        {
            return MadScience.Helpers.HsvToRgb(m_hue, m_saturation, m_value);
        }
        public static bool operator !=(HSVColor left, HSVColor right)
        {
            return !(left == right);
        }
        public static bool operator ==(HSVColor left, HSVColor right)
        {
            return (left.Hue == right.Hue &&
                    left.Value == right.Value &&
                    left.Saturation == right.Saturation);
        }
        public static HSVColor operator +(HSVColor left, HSVColor right)
        {
            return new HSVColor(left.Hue + right.Hue,left.Saturation + right.Saturation, left.Value + right.Value);
        }
        public static HSVColor operator -(HSVColor left, HSVColor right)
        {
            return new HSVColor(left.Hue - right.Hue, left.Saturation - right.Saturation, left.Value - right.Value);
        }
        public override string ToString()
        {
            string s = string.Format("HSV({0:f2}, {1:f2}, {2:f2})", Hue, Saturation, Value);
            return s;
        }
    }
}
