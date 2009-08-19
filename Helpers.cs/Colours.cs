using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;

namespace MadScience
{
    public class Colours
    {

        #region HSVColor
        public struct HSVColor
        {
            double m_hue;
            double m_saturation;
            double m_value;

            public double Hue
            {
                get { return m_hue; }
                set
                {
                    m_hue = value;
                    while (m_hue > 360) m_hue -= 360;
                    while (m_hue < 0) m_hue += 360;
                }
            }
            public double Saturation
            {
                get { return m_saturation; }
                set
                {
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
                m_hue = hue;
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
                return HsvToRgb(m_hue, m_saturation, m_value);
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
                return new HSVColor(left.Hue + right.Hue, left.Saturation + right.Saturation, left.Value + right.Value);
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
        #endregion

        #region Colour Conversions and functions
        public static string convertColour(Color color)
        {
            // Converts a colour dialog box colour into a 0 to 1 style colour
            float newR = color.R / 255f;
            float newG = color.G / 255f;
            float newB = color.B / 255f;
            float newA = color.A / 255f;

            //CultureInfo englishCulture = new CultureInfo("");
            string red = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newR);
            string green = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newG);
            string blue = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newB);
            string alpha = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newA);

            return red + "," + green + "," + blue + "," + alpha;

        }

        public static String ColorToHex(Color actColor)
        {
            return "#" + actColor.R.ToString("X2") + actColor.G.ToString("X2") + actColor.B.ToString("X2");
        }

        public static Color HexToColor(string hex)
        {
            return ColorFromHex(hex);
        }

        public static Color ColorFromHex(string hex)
        {
            if (hex.StartsWith("#")) { hex = hex.Remove(0, 1); }

            if (hex.Length != 6)
            {
                throw new Exception("Invalid hex");
            }
            int red = Convert.ToByte(hex.Substring(0, 2), 0x10);
            int green = Convert.ToByte(hex.Substring(2, 2), 0x10);
            int blue = Convert.ToByte(hex.Substring(4, 2), 0x10);
            return Color.FromArgb(0xff, red, green, blue);
        }

        public static Color convertColour(string colourString)
        {
            return convertColour(colourString, false);
        }
        public static Color convertColour(string colourString, bool returnNull)
        {
            Color newC;
            if (colourString != "" && colourString != null)
            {

                // Split the colour string using the ,
                string[] colours = colourString.Split(",".ToCharArray());
                float red = Convert.ToSingle(colours[0], CultureInfo.InvariantCulture);
                float green = Convert.ToSingle(colours[1], CultureInfo.InvariantCulture);
                float blue = Convert.ToSingle(colours[2], CultureInfo.InvariantCulture);
                float alpha = 255;
                if (colours[3] != null)
                {
                    alpha = Convert.ToSingle(colours[3], CultureInfo.InvariantCulture);
                }
                newC = Color.FromArgb((int)(alpha * 255), (int)(red * 255), (int)(green * 255), (int)(blue * 255));
            }
            else
            {
                if (!returnNull)
                {
                    newC = Color.Black;
                }
                else
                {
                    newC = Color.Empty;
                }
            }
            return newC;
        }

        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        public static Color HsvToRgb(double h, double S, double V)
        {
            Color temp = new Color();

            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            temp = Color.FromArgb(Clamp((int)(R * 255.0)), Clamp((int)(G * 255.0)), Clamp((int)(B * 255.0)));

            return temp;
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        #endregion

    }
    
}
