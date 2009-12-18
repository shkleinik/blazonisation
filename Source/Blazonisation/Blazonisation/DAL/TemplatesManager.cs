//----------------------------------------------------------------------------------
// <copyright file="TemplatesProvider.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using System.Drawing.Imaging;
    using System.IO;

    public class TemplatesManager
    {
        private const string CONNECTION_STRING = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=Blazonisation; Integrated security=true;";
        private const string SELECT_PATTERN = "SELECT * FROM Templates WHERE TemplateType = {0} ORDER BY ID";
        private const string SELECT_DEVISION = "SELECT * FROM Templates WHERE TemplateType = 2 AND MetaInfo = \'{0}\'";

        public static List<Template> GetTemlates(TemplateType type)
        {
            var templates = new List<Template>();

            var query = String.Format(SELECT_PATTERN, (int)type);

            using (var cn = new SqlConnection(CONNECTION_STRING))
            {
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader();
                    if (reader == null)
                        throw new Exception("Database connection error");

                    while (reader.Read())
                    {
                        templates.Add(new Template
                                          {
                                              ID = (int)reader["ID"],
                                              Image = GetBitmap((byte[])reader["Image"]),
                                              BitmapImage = GetBitmapImage((byte[])reader["Image"]),
                                              Description = (string)reader["Description"],
                                              MetaInfo = (string)reader["MetaInfo"],
                                              TemplateType = (TemplateType)(reader["TemplateType"])
                                          }
                            );
                    }
                }
            }

            return templates;
        }

        public static Template GetDevisionTemplateByCode(string devisionCode)
        {
            var query = String.Format(SELECT_DEVISION, devisionCode);

            using (var cn = new SqlConnection(CONNECTION_STRING))
            {
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    IDataReader reader = cmd.ExecuteReader();
                    if (reader == null)
                        throw new Exception("Database connection error");

                    while(reader.Read())
                    {
                        return new Template
                                   {
                                       ID = (int) reader["ID"],
                                       Image = GetBitmap((byte[]) reader["Image"]),
                                       BitmapImage = GetBitmapImage((byte[]) reader["Image"]),
                                       Description = (string) reader["Description"],
                                       MetaInfo = (string) reader["MetaInfo"],
                                       TemplateType = (TemplateType) (reader["TemplateType"])
                                   };
                    }
                }
            }

            return null;
        }

        public static void AddTemplate(Template template)
        {
            var ms = new MemoryStream();

            template.Image.Save(ms, ImageFormat.Bmp);
            var buffer = ms.ToArray();

            using (var cn = new SqlConnection(CONNECTION_STRING))
            {
                using (var cmd = new SqlCommand("AddTemplate", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TemplateType", template.TemplateType));
                    cmd.Parameters.Add(new SqlParameter("@Image", buffer));
                    cmd.Parameters.Add(new SqlParameter("@MetaInfo", template.MetaInfo));
                    cmd.Parameters.Add(new SqlParameter("@Description", template.Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static Bitmap GetBitmap(byte[] imageSource)
        {
            var ms = new MemoryStream(imageSource);
            return new Bitmap(ms);
        }

        private static BitmapImage GetBitmapImage(byte[] imageSource)
        {
            var bitmap = new BitmapImage();
            var strm = new MemoryStream();

            var offset = 0;
            strm.Write(imageSource, offset, imageSource.Length - offset);

            bitmap.BeginInit();
            bitmap.StreamSource = strm;
            bitmap.EndInit();
            return bitmap;
        }
    }
}