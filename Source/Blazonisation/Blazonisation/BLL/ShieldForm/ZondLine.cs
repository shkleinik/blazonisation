//----------------------------------------------------------------------------------
// <copyright file="ZondLine.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

using System;
using System.Drawing;

namespace Blazonisation.BLL.ShieldForm
{
    public class ZondLine
    {
        #region Private fields
        private readonly bool gVertical;
        private readonly int gX;
        private readonly int gY;
        private readonly Bitmap gBMP;
        private readonly int tolerance;
        #endregion

        #region Properties
        public Point[] colorChangeArray;
        #endregion

        #region Constructors
        public ZondLine(bool vertical, int coord, Bitmap bmp)
        {
            colorChangeArray = new Point[10];
            gVertical = vertical;
            gBMP = bmp;
            tolerance = 10;
            if (gVertical)
                gX = coord;
            else
                gY = coord;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Возвращает количество смены цвета в пределах одной линии-зонда
        /// </summary>
        /// <returns>Возвращает количество смены цвета в пределах одной линии-зонда</returns>
        public int FindColorChangeNumber()
        {
            var count = 0;
            if (gVertical)
            {
                var limit = gBMP.Height;
                for (var i = tolerance; i < limit - 2 * tolerance; i++)
                {
                    if (Math.Abs((GetAveregePixel(gBMP, gX, i) - (GetAveregePixel(gBMP, gX, i + 1)))) > tolerance)
                    {
                        if (count > 9)
                            continue;

                        colorChangeArray[count] = new Point(gX, i + 1);
                        count++;
                        i += tolerance;
                    }
                }
            }
            else
            {
                var limit = gBMP.Width;
                for (var i = tolerance; i < limit - 2 * tolerance; i++)
                {
                    if (Math.Abs((GetAveregePixel(gBMP, i, gY) - (GetAveregePixel(gBMP, i + 1, gY)))) > tolerance)
                    {
                        if (count > 9)
                            continue;

                        colorChangeArray[count] = new Point(i + 1, gY);
                        count++;
                        i += tolerance;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Средний пиксель из расчёта по К, З, С
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int GetAveregePixel(Bitmap bmp, int x, int y)
        {
            return (bmp.GetPixel(x, y).R + bmp.GetPixel(x, y).G + bmp.GetPixel(x, y).B) / 3;
        }
        #endregion
    }
}