//----------------------------------------------------------------------------------
// <copyright file="ColorRange.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL.Colors
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    class ColorRange
    {
        public static Color WhiteColor = Color.White;
        public static Color BlackColor= Color.Black;
        public static float saturs = 0;
        public static string BlackColorName = "Black";
        public static string WhiteColorName = "White";

        private static float[] DefaultHue = new float[] 
                                                { 
                                                    42,//red
                                                    70,//yellow
                                                    168,//green
                                                    276,//blue
                                                    329,//violet
                                                    361//red
                                                };
        private static List<string> DefaultColorNames = new List<string>()
                                                            {
                                                                "Red",
                                                                "Yellow",
                                                                "Green",
                                                                "Blue",
                                                                "Violet",
                                                                "Red"
                                                            };
        private static List<Color> defaultTrueColors = new List<Color>()
                                                           {
                                                               Color.Red,
                                                               Color.Yellow,
                                                               Color.Green,
                                                               Color.Blue,
                                                               Color.Violet,
                                                               Color.Red
                                                           };

        public static List<float> Hues = new List<float>();
        public static List<string> ColorNames = new List<string>();
        public static List<Color> TrueColors = new List<Color>();

        public static void AddColorRange(float hue, string colorName, Color trueColor)
        {
            Hues.Add(hue);
            ColorNames.Add(colorName);
            TrueColors.Add(trueColor);
        }

        public static void ClearColors()
        {
            Hues.Clear();
            ColorNames.Clear();
            TrueColors.Clear();
        }

        public static void SetDefaultColors()
        {
            Hues.AddRange(DefaultHue.ToList());
            ColorNames.AddRange(DefaultColorNames.ToList());
            TrueColors.AddRange(defaultTrueColors.ToList());
        }

        private static void CheckInitialization()
        {
            if (Hues.Count == 0)
            {
                SetDefaultColors();
            }
        }

        public static string GetColorNameByTrueColor(Color trueColor)
        {
            if (CheckIfBlack(trueColor))
                return BlackColorName;
            if (CheckIfWhite(trueColor))
                return WhiteColorName;
            int index=GetIndexOfTrueColorByHue(trueColor.GetHue());
            if (index != -1)
                return ColorNames[index];

            return "Color not found";
        }

        public static string GetColorNameByColor(Color color)
        {            
            Color trueColor = GetTrueColorByColor(color);            
            return GetColorNameByTrueColor(trueColor);           
        }

        public static Color GetTrueColorByColor(Color color)
        {
            CheckInitialization();
            Color blackWhiteOrColored=GetBlackWhiteOrColored(color);
            //if colored
            if (blackWhiteOrColored != Color.Gray)
            {
                return blackWhiteOrColored;
            }
            return GetTrueColorByHue(color.GetHue());
        }

        private static Color GetTrueColorByHue(float hue)
        {
            int index = GetIndexOfTrueColorByHue(hue);
            if (index != -1)
                return TrueColors[index];

            return Color.FromArgb(0, Color.Black);
        }

        private static int GetIndexOfTrueColorByHue(float hue)
        {
            if (Hues[0] > hue)
                return 0;
            for (int i = 0; i < Hues.Count - 1; i++)
            {
                if (Hues[i + 1] > hue && Hues[i] <= hue)
                    return i + 1;
            }
            return -1;
        }

        private static bool CheckIfWhite(Color trueColor)
        {
            if (trueColor.R == WhiteColor.R && trueColor.G == WhiteColor.G && trueColor.B == WhiteColor.B)
                return true;
            return false;           
        }
        private static bool CheckIfBlack(Color trueColor)
        {
            if (trueColor.R == BlackColor.R && trueColor.G == BlackColor.G && trueColor.B == BlackColor.B)
                return true;
            return false;
        }

        private static Color GetBlackWhiteOrColored(Color color)
        {
            float hue = color.GetHue();
            float sat =100* color.GetSaturation();
            
            float bri =100* color.GetBrightness();
            byte satPorog = 240;//less to increase white
            if (sat >= 99&&color.R>satPorog&&color.G>satPorog&&color.B>satPorog)
                sat = 0;            

            if (bri < 30)
            {
                return BlackColor;
            }

            if ((bri - 30) * 2 + sat < 80)//more to increase black
            {
                return BlackColor;
            }

            if (((100 - bri) + sat)<= 100)//more to increase white
            {
                return WhiteColor;
            }

            
            return Color.Gray;            
        }
    }
}