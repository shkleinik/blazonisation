//----------------------------------------------------------------------------------
// <copyright file="Template.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------



namespace Blazonisation.DAL
{
    using System;
    using System.Drawing;
    using System.Windows.Media.Imaging;

    public class Template
    {
        public int ID { get; set; }

        public TemplateType TemplateType { get; set; }

        public Bitmap Image { get; set; }

        public BitmapImage BitmapImage { get; set; }

        public String MetaInfo { get; set; }

        public String Description { get; set; }

    }
}