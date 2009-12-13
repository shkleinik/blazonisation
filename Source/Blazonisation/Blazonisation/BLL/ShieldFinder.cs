//----------------------------------------------------------------------------------
// <copyright file="ShieldFinder.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL
{
    using System;
    using System.Drawing;

    public class ShieldFinder
    {
        #region Constants
        private const int BACKGROUND_THRESHOLD = 240;
        #endregion

        #region Fields
        public Bitmap emblem;
        private Rectangle shieldRect;
        private Rectangle emblemRect;
        private int scan_step;
        private int colorTolerance;
        private int widthTolerance;
        #endregion

        #region Properties
        public Rectangle EmblemRect
        {
            get
            {
                return emblemRect;
            }
        }

        public Rectangle ShieldRect
        {
            get
            {
                return shieldRect;
            }
        }

        public Bitmap Shield
        {
            get { return GetImagePart(emblem, shieldRect); }
        }
        #endregion

        #region Constructors
        public ShieldFinder(Bitmap emblem)
        {
            this.emblem = emblem;
            InitFinder();
            PerfomShieldFind();
        }
        #endregion

        #region Public methods
        public static Bitmap GetImagePart(Bitmap bitmap, Rectangle rect)
        {
            if ((rect.X + rect.Width) > bitmap.Width)
                throw new ArgumentException("You are trying to cut more than allowed. Parameter - width.");

            if ((rect.Y + rect.Height) > bitmap.Height)
                throw new ArgumentException("You are trying to cut more than allowed. Parameter - height.");

            var result = new Bitmap(rect.Width, rect.Height);

            for (int y1 = rect.Y, y2 = 0; y1 < rect.Y + rect.Height; y1++, y2++)
            {
                for (int x1 = rect.X, x2 = 0; x1 < rect.X + rect.Width; x1++, x2++)
                {
                    result.SetPixel(x2, y2, bitmap.GetPixel(x1, y1));
                }
            }

            return result;
        }
        #endregion

        #region Private methods
        private void InitFinder()
        {
            colorTolerance = 10;
            widthTolerance = 20;

            shieldRect = new Rectangle();
            emblemRect = GetEmblemRectangle(emblem);
            emblem = GetImagePart(emblem, emblemRect);
            scan_step = Convert.ToInt32(emblemRect.Width * 0.13);
        }

        private void PerfomShieldFind()
        {
            FindLeftTop();
            shieldRect.Width = emblem.Width - 2 * shieldRect.X;//GetShieldWidth(shieldRect.X, shieldRect.Y);
            shieldRect.Height = (shieldRect.Width / 3) * 4;
        }

        public Rectangle GetEmblemRectangle(Bitmap bitmap)
        {
            var embRect = new Rectangle();
            var isFound = false;

            // find top border
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (IsBackGround(x, y))
                        continue;
                    embRect.Y = y;
                    isFound = true;
                }

                if (!isFound)
                    continue;

                isFound = false;
                break;
            }

            // find height
            for (var y = (bitmap.Height - 1); y > 0; y--)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (IsBackGround(x, y))
                        continue;
                    embRect.Height = y - embRect.Y;
                    isFound = true;
                }

                if (!isFound)
                    continue;

                isFound = false;
                break;
            }


            // find left border
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    if (IsBackGround(x, y))
                        continue;
                    embRect.X = x;
                    isFound = true;
                }

                if (!isFound)
                    continue;

                isFound = false;
                break;
            }

            // find width
            for (var x = (bitmap.Width - 1); x > 0; x--)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    if (IsBackGround(x, y))
                        continue;
                    embRect.Width = x - embRect.X;
                    isFound = true;
                }

                if (!isFound)
                    continue;

                isFound = false;
                break;
            }

            return embRect;
        }

        /// <summary>
        /// Defines coordinates of left-top shield corner.
        /// </summary>
        private void FindLeftTop()
        {
            var isShieldFound = false;

            //var x1 = emblemRect.X;
            //var x2 = emblemRect.X + emblemRect.Width / 2;
            //var y1 = emblemRect.Y;
            //var y2 = emblemRect.Height;
            var x1 = 0;
            var x2 = emblem.Width / 2;
            var y1 = 0;
            var y2 = emblem.Height;

            // scans each row
            for (var y = y1; y < y2; y++)
            {
                for (var x = x1; x < x2; x++)
                {

                    if (IsBackGround(x, y))
                    {
                        //emblem.SetPixel(x, y, Color.Violet);
                        continue;
                    }

                    if (HorisontalRepeat(x, y) < scan_step)
                    {
                        //emblem.SetPixel(x, y, Color.Red);
                        continue;
                    }

                    shieldRect.X = x;
                    shieldRect.Y = y;
                    isShieldFound = true;
                    break;
                }

                if (isShieldFound)
                    break;
            }

            shieldRect.X -= Convert.ToInt32(emblem.Width * 0.01);
            shieldRect.Y -= Convert.ToInt32(emblem.Height * 0.01);
        }

        private int HorisontalRepeat(int x, int y)
        {
            var start = GetAveregePixel(x, y);
            var colorRepeatCount = 0;
            do
            {
                x++;
                colorRepeatCount++;
            } while (
                (x < (emblem.Width / 2)) &&
                (y < emblem.Height) &&
                (Math.Abs(GetAveregePixel(x, y) - start) <= colorTolerance)
                );

            return colorRepeatCount;
        }

        private int GetShieldWidth(int x, int y)
        {
            var startColor = GetAveregePixel(x, y);
            var width = 0;
            var maxCount = 0;
            do
            {
                var tmp = HorisontalRepeat(x, y);
                width = tmp > width ? tmp : width;
                if (Math.Abs(tmp - width) <= 2)
                    maxCount++;

                y++;

                if (y >= emblem.Height)
                    break;

            } while ((maxCount < widthTolerance) || (GetAveregePixel(x, y) == startColor));

            return width;
        }

        /// <summary>
        /// Returns average value of R, G and B of the pixel.
        /// </summary>
        /// <param name="x">x coordinate in image-file.</param>
        /// <param name="y">y coordinate in image-file.</param>
        /// <returns>Returns average value of R, G and B of the pixel.</returns>
        private int GetAveregePixel(int x, int y)
        {
            return (emblem.GetPixel(x, y).R + emblem.GetPixel(x, y).G + emblem.GetPixel(x, y).B) / 3;
        }

        private bool IsBackGround(int x, int y)
        {
            return GetAveregePixel(x, y) > BACKGROUND_THRESHOLD;
        }
        #endregion
    }
}

//ShieldHeight = GetHeight();

//private int GetWidth(int i, int j)
//{
//    var width = GetTmpWidth(i, j);
//    var width1 = width;
//    var width2 = width;
//    while (Math.Round((double)width1 / width2 - 1) < 0.5)
//    {
//        width1 = width;
//        var j2 = width1 + 10;
//        width2 = GetWidth(i, j2);
//        width = width1 + width2;
//    }
//    return width;
//}

//private int GetHeight()
//        {
//            var i = shieldRect.X;
//            var j = shieldRect.Y;
//            var start = GetAveregePixel(i, j);
//            var height = 0;
//            for (var m = 0; m < shieldRect.Width; m++)
//            {
//                j = shieldRect.Y;
//                do
//                    j++;
//                while ((j < emblem.Height) && (i < emblem.Width) && (Math.Abs(GetAveregePixel(i++, j) - start) <= 1));
//                height = j > height ? j : height;
//            }
//            return height;
//        }

//private bool IsHorizontalRepeat(int i, int j, int lengthTolerance, int colorTolerance) //defines wether the color repeats within a horizontal set of pixels
//{
//    var start = GetAveregePixel(i, j);
//    var colorRepeatCount = 0;
//    do
//    {
//        i++;
//        colorRepeatCount++;
//    } while ((i < emblem.Width) && (Math.Abs(GetAveregePixel(i, j) - start) <= colorTolerance));
//    return (colorRepeatCount >= lengthTolerance);
//}