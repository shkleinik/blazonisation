//----------------------------------------------------------------------------------
// <copyright file="TemplatesProvider.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL.ShieldForm
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Colors;

    class ShieldFormDefiner
    {
        private Bitmap inputBMP;
        private readonly List<Bitmap> models;
        private List<ColorDetails> colorDetails;
        private double[] zondValues;

        public ShieldFormDefiner(Bitmap bmp, List<Bitmap> models)
        {
            InitializeInputBMP(bmp);
            this.models = models;
            InitializeZondValues();
        }

        private void InitializeZondValues()
        {
            var c = models.Count;
            zondValues = new double[c];
            for (var i = 0; i < c; i++)
                zondValues[i] = GetZondsValues(models[i]);
        }

        private void InitializeInputBMP(Bitmap bmp)
        {
            //inputBMP = (Bitmap)bmp.GetThumbnailImage(199, 237, null, IntPtr.Zero);
            inputBMP = bmp;
            /// todo: decolorize thumbnail image after scalling (=
            colorDetails = ColorDetails.GetColorDetailsFull(inputBMP);
        }

        private bool IsOfMajorColor(int x, int y)
        {
            var c = inputBMP.GetPixel(x, y);
            foreach (var detail in colorDetails)
            {
                if (CompareColors(detail.Color, c) && (detail.PercentOnImage > 0.2))
                    return true;
            }
            return false;
        }

        public int FindBestPercentage()
        {
            var result = new double[models.Count];
            for (var i = 0; i < models.Count; i++)
                result[i] = FindPercentage(models[i]);
            return FindMin(result);
        }

/*
        private double FindPercentage2(Bitmap model) // ищем процент пикселей опред. цвета в пикселях эталона
        {
            inputBMP = (Bitmap)inputBMP.GetThumbnailImage(model.Width, model.Height, null, IntPtr.Zero); // вот когда размеры темплейтов будут одинаковыми, эту строчку СТЕРЕТЬ, Паша!
            double count = 0;
            var whitecount1 = 0;
            var whitecount2 = 0;
            for (var y = 0; y < model.Height; y++) // считаем среднее значение для всех пискелей, что подпадают под эталон
                for (var x = 0; x < model.Width; x++)
                {
                    count++;
                    if (GetAveragePixel(model, x, y) > 5)
                        whitecount1++;
                    if (!IsOfMajorColor(x, y))
                        whitecount2++;
                    //if ((y < inputBMP.Height) && (x < inputBMP.Width))
                    //    if ((IsOfMajorColor(x, y) && (GetAveragePixel(model, x, y) > 5)) ||
                    //        (!IsOfMajorColor(x, y) && (GetAveragePixel(model, x, y) < 5)))
                    //        var++;
                }
            return (whitecount1 - whitecount2) / count;
        }
*/

        private double FindPercentage(Bitmap model)
        {
            double modelPxCount = 0;
            double inputBmpPxCount = 0;

            inputBMP = (Bitmap)inputBMP.GetThumbnailImage(model.Width, model.Height, null, IntPtr.Zero); // вот когда размеры темплейтов будут одинаковыми, эту строчку СТЕРЕТЬ, Паша!
            for (var y = 0; y < model.Height; y++)
            {
                for (var x = 0; x < model.Width; x++) //(int)(0.6 * model.Height)k
                {
                    if ((GetAveragePixel(model, x, y) < 5))
                        modelPxCount++;
                    if (IsOfMajorColor(x, y))
                        inputBmpPxCount++;
                }
            }
            return modelPxCount / inputBmpPxCount;//modelPxCount;
        }

        public int GetRes()
        {
            int index;
            var res = GetZond();
            //if (res > 0.8) index = 2;
            //else index = res > 0.7 ? 1 : 0;
            if (res > zondValues[2]) index = 2;
            else index = res > zondValues[1] ? 1 : 0;
            return index;
        }

        private double GetZond()
        {
            var x = inputBMP.Width / 10;
            var y = inputBMP.Height;
            do
            {
                y--;
            } while (y > 0 && !isYRepeating(x, y));
            var res = (double)y / inputBMP.Height;
            return res;
        }

        private static double GetZondsValues(Bitmap input)
        {
            var x = input.Width / 10;
            var y = input.Height;
            do
            {
                y--;
            } while (CompareColors(input.GetPixel(x, y), Color.White));
            var res = (double)y / input.Height;
            return res - 0.09;
        }

        private bool isYRepeating(int x, int y)
        {
            var tolerance = 70;
            for (var ty = y; ty > y - tolerance; ty--)
            {
                if (CompareColors(inputBMP.GetPixel(x, ty), inputBMP.GetPixel(x, ty - 1)) &&
                    !CompareColors(inputBMP.GetPixel(x, ty), Color.White))
                    continue;
                return false;
            }
            return true;
        }

        private static int FindMin(double[] array) // находим ИНДЕКС минимальное значение из массива результатов
        {
            double min = 10000000;
            var index = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                    index = i;
                }
            }
            return index;
        }

        private static int GetAveragePixel(Bitmap bmp, int x, int y) // средний пиксель из расчёта по К, З, С
        {
            return (bmp.GetPixel(x, y).R + bmp.GetPixel(x, y).G + bmp.GetPixel(x, y).B) / 3;
        }

        private static bool CompareColors(Color color1, Color color2)
        {
            return
                ((color1.R == color2.R) &&
                 (color1.G == color2.G) &&
                 (color1.B == color2.B))
                    ? true
                    : false;
        }
    }
}


