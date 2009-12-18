//----------------------------------------------------------------------------------
// <copyright file="Common.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.BLL
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;
    using DAL;


    public static class Common
    {
        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            var bitMapImage = new BitmapImage();
            var ms = new MemoryStream();

            try
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                bitMapImage.BeginInit();
                bitMapImage.StreamSource = ms;
                bitMapImage.EndInit();
            }
            catch (NotSupportedException e)
            {
                try
                {
                    bitMapImage = new BitmapImage();
                    bitmap.Save(ms, ImageFormat.Png);
                    bitMapImage.BeginInit();
                    bitMapImage.StreamSource = ms;
                    bitMapImage.EndInit();
                }
                catch (NotSupportedException e2)
                {
                    bitMapImage = new BitmapImage();
                    bitmap.Save(ms, ImageFormat.Bmp);
                    ms.Write(ms.ToArray(), 78, (int)(ms.Length - 78));
                    bitMapImage.BeginInit();
                    bitMapImage.StreamSource = ms;
                    bitMapImage.EndInit();
                }
            }

            return bitMapImage;
        }

        public static Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                return new Bitmap(outStream);
            }
        }

        public static List<Bitmap> ConvertTemplatesToBitmaps(List<Template> templates)
        {
            var bitmaps = new List<Bitmap>();

            foreach (var template in templates)
            {
                bitmaps.Add(template.Image);
            }

            return bitmaps;
        }

        public static bool CompareColors(Color color1, Color color2)
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
