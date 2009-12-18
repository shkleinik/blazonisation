//----------------------------------------------------------------------------------
// <copyright file="DivisionDefiner.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL.Devisions
{
    using System.Collections.Generic;
    using System.Drawing;
    using Colors;
    using C = Common;

    public class DivisionDefiner
    {
        private readonly IList<List<ColorDetails>> sectionColorDetails;

        public string DevisionCode
        {
            get
            {
                return GetDevisionCode(sectionColorDetails);
            }
        }

        public DivisionDefiner(Bitmap shield)
        {
            var colors = SectionCreator.GetSectionsArray(shield);
            sectionColorDetails = new List<List<ColorDetails>>();
            for (var i = 0; i < colors.Length; i++)
            {
                var details = ColorDetails.GetColorDetailsPixelPercentage(colors[i]);
                sectionColorDetails.Add(details);
            }
        }

        private static string GetDevisionCode(IList<List<ColorDetails>> colorDetailsList)
        {
            var sectorsColors = new List<Color>();

            for (var i = 0; i < colorDetailsList.Count; i++)
            {
                var importantColors = ColorDetector.GetImportantColors(colorDetailsList[i]);

                importantColors.Sort();
                sectorsColors.Add(importantColors[0].Color);
            }

            var result = "";
            var init = sectorsColors[0];

            foreach (var color in sectorsColors)
            {
                if (C.CompareColors(init, color))
                    result += 0;
                else
                    result += 1;
            }

            return result;

        }
    }
}