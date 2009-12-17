//----------------------------------------------------------------------------------
// <copyright file="ColorDetector.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL.Colors
{
    using System.Collections.Generic;

    public class ColorDetector
    {
        public static double percentSquare = 20;
        public static double percentRange = 50;
        public static double percentImportantSquareForSection = 40;

        public static List<ColorDetails> GetImportantColors(List<ColorDetails> possibleImportant,int imageWidthPlusHeight)
        {
            double minRange = (imageWidthPlusHeight) * 0.5 * percentRange;
            List<ColorDetails> important = new List<ColorDetails>();
            foreach (var det in possibleImportant)
            {                
                double importance = 100 * det.PercentOnImage / (det.RangeSum / det.PixelsOnImage);               
                if (importance > 100 * percentSquare / minRange)
                {
                    important.Add(det);
                }
            }
            return important;
        }

        public static List<ColorDetails> GetImportantColors(List<ColorDetails> possibleImportant)
        {
            List<ColorDetails> important = new List<ColorDetails>();
            foreach (var det in possibleImportant)
            {
                double importance = 100 * det.PercentOnImage;
                if (importance > percentImportantSquareForSection )
                {
                    important.Add(det);
                }
            }
            return important;
        }
    }
}