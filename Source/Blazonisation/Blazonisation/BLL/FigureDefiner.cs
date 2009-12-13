//----------------------------------------------------------------------------------
// <copyright file="FigureDefiner.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL
{
    using System.Drawing;

    public class FigureDefiner
    {
        #region Fields
        public Bitmap inputBMP;
        public string resultOutput;
        #endregion

        #region Constructors
        public FigureDefiner(Bitmap bmp)
        {
            inputBMP = bmp;
            resultOutput = DefineVerticalCrossings() + "_" + DefineHorizontalCrossings();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Находим точки смены цвета ВЕРТИКАЛЬНЫХ зондов
        /// </summary>
        /// <returns></returns>
        private string DefineVerticalCrossings()
        {
            var line1 = new ZondLine(true, inputBMP.Width / 20, inputBMP);
            var line2 = new ZondLine(true, inputBMP.Width / 2, inputBMP);
            var line3 = new ZondLine(true, inputBMP.Width * 19 / 20, inputBMP);
            var line4 = new ZondLine(true, inputBMP.Width * 11 / 20, inputBMP);
            var line5 = new ZondLine(true, inputBMP.Width * 9 / 20, inputBMP);
            var lineH = new ZondLine(false, inputBMP.Height / 2, inputBMP);

            var result1 = line1.FindColorChangeNumber(); // находим точки и их кол-во
            var result2 = line2.FindColorChangeNumber();
            var result3 = line3.FindColorChangeNumber();
            var result4 = line4.FindColorChangeNumber();
            var result5 = line5.FindColorChangeNumber();
            //var resultH = lineH.FindColorChangeNumber();

            var result1pts = line1.colorChangeArray; // загоняем найденные точки в массивы
            var result2pts = line2.colorChangeArray;
            var result3pts = line3.colorChangeArray;
            //var result4pts = line4.colorChangeArray;
            //var result5pts = line5.colorChangeArray;
            var resultHpts = lineH.colorChangeArray;

            var dif1 = result1pts[0].Y > inputBMP.Height / 2 ? 0 : 1; // выше ли точка половины щита?
            var dif2 = result2pts[0].Y > inputBMP.Height / 2 ? 0 : 1;
            var dif3 = result3pts[0].Y > inputBMP.Height / 2 ? 0 : 1;

            int dif4; // растянут по горизонтали?
            int dif5; // растянут по вертикали?    

            if ((result2pts[1].Y - result2pts[0].Y) * (resultHpts[1].X - resultHpts[0].X) != 0)
            {
                dif4 = (result2pts[1].Y - result2pts[0].Y) / (resultHpts[1].X - resultHpts[0].X) < 1 ? 1 : 0;
                dif5 = (result2pts[1].Y - result2pts[0].Y) / (resultHpts[1].X - resultHpts[0].X) > 1 ? 1 : 0;
            }
            else
            {
                dif4 = (result2pts[0].Y - inputBMP.Height / 2) / (resultHpts[0].X - inputBMP.Width / 2) > 1 ? 1 : 0;
                dif5 = (result2pts[0].Y - inputBMP.Height / 2) / (resultHpts[0].X - inputBMP.Width / 2) > 1 ? 1 : 0;
            }

            return result1 + "" + result2 + "" + result3 + "" + result4 + "" + result5 + "" +
                   dif1 + "" + dif2 + "" + dif3 + "" + dif4;// +"" + dif5;// +"" + dif6;
        }

        /// <summary>
        /// Находим точки смены цвета ГОРИЗОНТАЛЬНЫХ зондов
        /// </summary>
        /// <returns></returns>
        private string DefineHorizontalCrossings() 
        {
            var line1 = new ZondLine(false, inputBMP.Height / 22, inputBMP);
            var line2 = new ZondLine(false, inputBMP.Height / 2, inputBMP);
            var line3 = new ZondLine(false, inputBMP.Height * 10 / 11, inputBMP);
            var line4 = new ZondLine(false, inputBMP.Height * 10 / 22, inputBMP);
            var line5 = new ZondLine(false, inputBMP.Height * 14 / 22, inputBMP);

            var result1 = line1.FindColorChangeNumber();
            var result2 = line2.FindColorChangeNumber();
            var result3 = line3.FindColorChangeNumber();
            var result4 = line4.FindColorChangeNumber();
            var result5 = line5.FindColorChangeNumber();

            var result1pts = line1.colorChangeArray;
            var result2pts = line2.colorChangeArray;
            var result3pts = line3.colorChangeArray;
            var result4pts = line4.colorChangeArray;
            //var result5pts = line5.colorChangeArray;

            var dif1 = result1pts[0].X > inputBMP.Width / 2 ? 0 : 1;
            var dif2 = result2pts[0].X > inputBMP.Width / 2 ? 0 : 1;
            var dif3 = result3pts[0].X > inputBMP.Width / 2 ? 0 : 1;
            var dif4 = result4pts[0].X > result2pts[0].X ? 1 : 0; // похож ли объект на ромбик? (=
            var dif5 = result2pts[0].X < inputBMP.Width / 4 ? 0 : 1;

            return result1 + "" + result2 + "" + result3 + "" + result4 + "" + result5 + "" +
                   dif1 + "" + dif2 + "" + dif3 + "" + dif4 + "" + dif5;
        }
        #endregion
    }
}