//----------------------------------------------------------------------------------
// <copyright file="ImageColorConverter.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL.Colors
{
    using System.Drawing;

    public class ImageColorConverter
    {
        public static Bitmap AverageImageColorsFromFileName(string fileName)
        {
            Bitmap bitmap =new Bitmap( Bitmap.FromFile(fileName));
            return AverageImageColorsFromBitmap(bitmap);
        }

        public static Bitmap AverageImageColorsFromBitmap(Bitmap bitmap)
        {
            bitmap= (Bitmap)bitmap.Clone();
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    pixelColor = ColorRange.GetTrueColorByColor(pixelColor);
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }
            return bitmap;
        }

        public static Bitmap AverageImageColorsFromImage(Image image)
        {
            return AverageImageColorsFromBitmap(new Bitmap(image));
        }

        public static Image BitmapToImage(Bitmap bitmap)
        {
            return Image.FromHbitmap(bitmap.GetHbitmap());
        }
    }
}