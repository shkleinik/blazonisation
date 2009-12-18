


namespace Blazonisation.BLL.Figures
{
    using System;
    using Colors;
    using System.Collections.Generic;
    using System.Drawing;

    class FigureDefiner
    {
        private const int blackPixelTolerance = 5;
        private const int averagePixelTolerance = 2;
        private Bitmap inputBMP;

        public string resultOutputString;
        public int[] resultOutputInt;
        public double[] resultArray;

        private List<Bitmap> Models;
        private readonly int count;

        public FigureDefiner(Bitmap bmp, int count)
        {
            this.count = count;
            InitializeInputBMP(bmp);
            InitializeModels();
            InitializeResultOutput();
        }

        public FigureDefiner(Bitmap bmp, List<Bitmap> bitmaps)
        {
            count = bitmaps.Count;
            InitializeInputBMP(bmp);
            Models = bitmaps;
            InitializeResultOutput();
        }

        #region input-output

        private void InitializeInputBMP(Bitmap bmp)
        {
            
            inputBMP = (Bitmap) bmp.GetThumbnailImage(200, 240, null, IntPtr.Zero);
            /// todo: decolorize thumbnail image after scalling (=
            //inputBMP.Save("THIS.BMP");
        }

        private void InitializeModels() // создаём коллекцию эталонов
        {
            Models = new List<Bitmap>();
            for (var i = 0; i < count; i++) // figure quantity
            {
                var bmp = new Bitmap("models\\figure" + (i + 1) + ".bmp");
                Models.Add(bmp);
            }
        }

        private void InitializeResultOutput() // форматируем строку вывода с трёмя лучшими значениями
        {
            //var result = Find3Min(resultArray);
            //resultOutputString = (result[0] + 1).ToString(".#") + " " +
            //               (result[1] + 1).ToString(".#") + " " +
            //               (result[2] + 1).ToString(".#");
            //resultOutputInt = FindMin(resultArray);
            resultOutputInt = FindModelColorCoinsidence();
        }

        private string ShowExtendedResult() // формируем строку вывода со значениями массива результатов
        {
            var result = (FindMax(resultArray) + 1).ToString(".#") + "\n";
            for (var i = 0; i < resultArray.Length; i++)
            {
                result += "[" + (i + 1) + "]=" + resultArray[i].ToString(".#") + "\n";
            }
            return result;
        }

        #endregion

        #region max-min methods

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

        private static int FindMax(double[] array) // находим ИНДЕКС минимальное значение из массива результатов
        {
            double max = -10000000;
            var index = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                    index = i;
                }
            }
            return index;
        }

        private static int[] Find3Max(double[] array)// находим ИНДЕКСЫ три МАКСимальных значения из массива результатов
        {
            var tmp = (double[])array.Clone();

            var index1 = FindMax(tmp);
            tmp[index1] = -10000000;

            var index2 = FindMax(tmp);
            tmp[index2] = -10000000;

            var index3 = FindMax(tmp);
            tmp[index3] = -10000000;

            var result = new int[3];
            result[0] = index1;
            result[1] = index2;
            result[2] = index3;

            return result;
        }

        private static int[] Find3Min(double[] array) // находим ИНДЕКСЫ три МИНИмальных значения из массива результатов
        {
            var tmp = (double[])array.Clone();

            var index1 = FindMin(tmp);
            tmp[index1] = 10000000;

            var index2 = FindMin(tmp);
            tmp[index2] = 10000000;

            var index3 = FindMin(tmp);
            tmp[index3] = 10000000;

            var result = new int[3];
            result[0] = index1;
            result[1] = index2;
            result[2] = index3;

            return result;
        }

        #endregion

        #region Algorythm 3b

        private int[] FindModelColorCoinsidence() // находим ИНДЕКС эталона, с которым какой-то цвет совпал лучше всего
        {
            var colorDetails = ColorDetails.GetColorDetailsFull(inputBMP);

            double bestValue = 0;
            int[] bestIndex = { 0, 0, 0 };
            for (var i = 0; i < colorDetails.Count; i++)
            {
                if ((colorDetails[i].PercentOnImage > 0.1) && (colorDetails[i].PercentOnImage < 0.56))
                {
                    var tmpArray = CreateCoinsidePxPercArray(colorDetails[i].Color, colorDetails[i].PixelsOnImage);
                    var tmpBest = Find3Min(tmpArray);
                    if (tmpArray[tmpBest[0]] > bestValue)
                    {
                        bestValue = tmpArray[tmpBest[0]];
                        bestIndex = tmpBest;
                    }
                }
            }
            return bestIndex;
        }

        private double[] CreateCoinsidePxPercArray(Color color, int number) // ищем процент пикселей опред. цвета в пикселях ВСЕХ эталонов
        {
            var result = new double[Models.Count];
            for (var i = 0; i < Models.Count; i++)
            {
                //result[i] = Math.Abs(FindCoinsidePxPercent(Models[i], number) * FindCoinsidePxPercent(Models[i], color));
                result[i] = FindCoinsidePxPercent(Models[i], color);
            }
            return result;
        }

        private double FindCoinsidePxPercent(Bitmap model, int number)
        {
            double modelPxCount = 0;
            for (var y = 0; y < model.Height; y++)
                for (var x = 0; x < model.Width; x++)
                {
                    if ((y < inputBMP.Height) && (x < inputBMP.Width))
                        if ((GetAveragePixel(model, x, y) < blackPixelTolerance))
                            modelPxCount++;
                }

            return Math.Abs(number - modelPxCount);
        }

        private double FindCoinsidePxPercent(Bitmap model, Color color) // ищем процент пикселей опред. цвета в пикселях эталона
        {
            double modelPxCount = 0;
            //double impPxCount = 0;
            double var = 0;
            for (var y = 0; y < model.Height; y++) // считаем среднее значение для всех пискелей, что подпадают под эталон
                for (var x = 0; x < model.Width; x++)
                {
                    if ((y < inputBMP.Height) && (x < inputBMP.Width))
                        if (((CompareColors(GetColor(inputBMP, x, y), color)) && (GetAveragePixel(model, x, y) > blackPixelTolerance)) ||
                            ((!CompareColors(GetColor(inputBMP, x, y), color)) && (GetAveragePixel(model, x, y) < blackPixelTolerance)))
                            var++;
                    if ((GetAveragePixel(model, x, y) < blackPixelTolerance))
                        modelPxCount++;

                }
            return var;//modelPxCount;
        }

        #endregion

        #region additional methods

        private static Color GetColor(Bitmap bmp, int x, int y)
        {
            return Color.FromArgb(
                                 bmp.GetPixel(x, y).A,
                                 bmp.GetPixel(x, y).R,
                                 bmp.GetPixel(x, y).G,
                                 bmp.GetPixel(x, y).B);
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

        #endregion
    }
}
