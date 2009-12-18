//----------------------------------------------------------------------------------
// <copyright file="ColorDetails.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL.Colors
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ColorDetails : IComparable
    {
        public Color Color;
        public string ColorName;
        public int PixelsOnImage = -1;
        public double PercentOnImage;
        public int RangeSum;

        private static void InitPercentage(double imageSquare, ColorDetails details)
        {
            details.PercentOnImage = details.PixelsOnImage / imageSquare;
        }

        private static List<ColorDetails> InitPixelsAndRangeSum(Bitmap bitmap)
        {
            Point imageCenter = new Point(bitmap.Width / 2, bitmap.Height / 2);
            List<ColorDetails> colorDetails = new List<ColorDetails>();
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    ColorDetails details = new ColorDetails();
                    Color c = bitmap.GetPixel(x, y);
                    c = ColorRange.GetTrueColorByColor(c);
                    List<ColorDetails> hasColor = colorDetails.Where(findC => (findC.Color.R == c.R
                                                                               && findC.Color.G == c.G
                                                                               && findC.Color.B == c.B)).ToList();
                    if (hasColor.Count > 0)
                    {
                        //increase pixel number
                        hasColor[0].PixelsOnImage++;
                        details = hasColor[0];
                    }
                    else
                    {
                        //create new colorDetails                        
                        details.PixelsOnImage = 1;
                        details.Color = c;
                        //ColorRange.SetDefaultColors();
                        details.ColorName = ColorRange.GetColorNameByTrueColor(c);
                        colorDetails.Add(details);
                    }

                    //init color RangeSum
                    int xRange = Math.Abs(imageCenter.X - x);
                    int yRange = Math.Abs(imageCenter.Y - y);
                    details.RangeSum += xRange + yRange;
                }
            }
            return colorDetails;
        }

        private static List<ColorDetails> InitPixels(List<Color> array)
        {
            List<ColorDetails> colorDetails = new List<ColorDetails>();
            for (int x = 0; x < array.Count; x++)
            {
                ColorDetails details = new ColorDetails();
                Color c = array[x];
                c = ColorRange.GetTrueColorByColor(c);
                List<ColorDetails> hasColor = colorDetails.Where(findC => (findC.Color.R == c.R
                                                                           && findC.Color.G == c.G
                                                                           && findC.Color.B == c.B)).ToList();
                if (hasColor.Count > 0)
                {
                    //increase pixel number
                    hasColor[0].PixelsOnImage++;
                    details = hasColor[0];
                }
                else
                {
                    //create new colorDetails                        
                    details.PixelsOnImage = 1;
                    details.Color = c;
                    //ColorRange.SetDefaultColors();
                    details.ColorName = ColorRange.GetColorNameByTrueColor(c);
                    colorDetails.Add(details);
                }

            }
            return colorDetails;
        }

        public static List<ColorDetails> GetColorDetailsPixelPercentage(List<Color> colors)
        {
            List<ColorDetails> colorDetailsList = InitPixels(colors);
            foreach (var cd in colorDetailsList)
            {
                InitPercentage(colors.Count, cd);
            }
            return colorDetailsList;
        }

        public static List<ColorDetails> GetColorDetailsFull(Bitmap bitmap)
        {
            List<ColorDetails> colorDetailsList = InitPixelsAndRangeSum(bitmap);
            foreach (var cd in colorDetailsList)
            {
                InitPercentage(bitmap.Width * bitmap.Height, cd);
            }
            return colorDetailsList;
        }

        #region Implementation of IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is ColorDetails))

                throw new ArgumentException("object is not a Temperature");

                var otherDetails = (ColorDetails)obj;
                return PercentOnImage.CompareTo(otherDetails.PercentOnImage);
        }

        #endregion
    }
}