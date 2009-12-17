//----------------------------------------------------------------------------------
// <copyright file="Common.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

using Blazonisation.DAL;

namespace Blazonisation.BLL
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Collections.Generic;

    public static class Common
    {
        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            var bitMapImage = new BitmapImage();
            var ms = new MemoryStream();

            bitmap.Save(ms, ImageFormat.Bmp);
            bitMapImage.BeginInit();
            bitMapImage.StreamSource = ms;
            bitMapImage.EndInit();

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
    }
}
