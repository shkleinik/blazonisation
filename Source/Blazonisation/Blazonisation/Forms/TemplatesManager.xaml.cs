//----------------------------------------------------------------------------------
// <copyright file="TemplatesManager.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------



namespace Blazonisation.Forms
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using BLL.Devisions;
    using DAL;
    using BLL.Colors;
    using Microsoft.Win32;
    using C = BLL.Common;
    using TM = DAL.TemplatesManager;

    /// <summary>
    /// Interaction logic for TemplatesManager.xaml
    /// </summary>
    public partial class TemplatesManager
    {
        #region Constants
        private const String TEXTBOX_DESC = "Description . . .";
        #endregion

        #region Constructors
        public TemplatesManager()
        {
            InitializeComponent();
        }
        #endregion

        #region Events handling
        private void On_TemplateManage_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateFormWithTemplates();
        }

        private void On_btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
            {
                Filter = "Images | *.bmp; *.png; *.jpg; *.jpeg",
                Multiselect = false
            };

            if (!((bool)fileOpenDialog.ShowDialog(this)))
                return;

            tbPath.Text = fileOpenDialog.FileName;
        }

        private void On_btnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(tbPath.Text))
            {
                MessageBox.Show("Please, choose template location.", "Verify input data.", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (!File.Exists(tbPath.Text))
                return;

            if (tbDescription.Text == TEXTBOX_DESC)
                tbDescription.Text = String.Empty;

            var template = InitTemplate((TemplateType)cbTypes.SelectedIndex);
            //= new Template
            //                  {
            //                      Description = tbDescription.Text,
            //                      Image = new Bitmap(tbPath.Text),
            //                      MetaInfo = Guid.NewGuid().ToString(),
            //                      TemplateType = (TemplateType)cbTypes.SelectedIndex
            //                  };

            TM.AddTemplate(template);
            tbDescription.Text = TEXTBOX_DESC;
            tbPath.Text = String.Empty;
            PopulateFormWithTemplates();
        }

        private void On_tbDescription_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            tbDescription.Text = String.Empty;
        }

        private void On_tbDescription_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (tbDescription.Text == String.Empty)
                tbDescription.Text = TEXTBOX_DESC;
        }

        private void On_cbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateFormWithTemplates();
        }
        #endregion

        #region Methods
        private void PopulateFormWithTemplates()
        {
            var templates = TM.GetTemlates((TemplateType)(cbTypes.SelectedIndex));

            spTemplates.Children.Clear();

            foreach (var template in templates)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    MinHeight = 120,
                    Height = 120
                };

                var lblID = new Label
                {
                    Content = template.ID,
                    FlowDirection = FlowDirection.LeftToRight,
                    MinHeight = 120,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var lblTemplateType = new Label
                {
                    Content = template.TemplateType,
                    FlowDirection = FlowDirection.LeftToRight,
                    MinHeight = 120
                };

                // template.Image.Save("template" + template.ID + ".bmp");

                var img = new System.Windows.Controls.Image
                {
                    Source = template.BitmapImage,
                    Width = 120,
                    Height = 80,
                    MinHeight = 120//,
                };


                var lblMetaInfo = new Label
                {
                    Content = template.MetaInfo,
                    FlowDirection = FlowDirection.LeftToRight,
                    MinHeight = 120
                };

                var lblDescription = new Label
                {
                    Content = template.Description,
                    FlowDirection = FlowDirection.LeftToRight,
                    MinHeight = 120
                };


                stackPanel.Children.Add(lblID);
                stackPanel.Children.Add(img);
                stackPanel.Children.Add(lblTemplateType);
                stackPanel.Children.Add(lblMetaInfo);
                stackPanel.Children.Add(lblDescription);

                spTemplates.Children.Add(stackPanel);
                spTemplates.Children.Add(new Separator());
            }
        }

        private Template InitTemplate(TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.Devisions:
                    var devisionDefiner = new DivisionDefiner(new Bitmap(tbPath.Text));

                    return new Template
                     {
                         Description = tbDescription.Text,
                         Image = new Bitmap(tbPath.Text),
                         MetaInfo = devisionDefiner.DevisionCode,
                         TemplateType = (TemplateType)cbTypes.SelectedIndex
                     };

                case TemplateType.Colors:
                    var shield = new Bitmap(tbPath.Text);
                    var trueColor = ColorRange.GetTrueColorByColor(shield.GetPixel(0, 0));

                    return new Template
                    {
                        Description = tbDescription.Text,
                        Image = new Bitmap(tbPath.Text),
                        MetaInfo = trueColor.Name,
                        TemplateType = (TemplateType)cbTypes.SelectedIndex
                    };

                default:
                    return new Template
                     {
                         Description = tbDescription.Text,
                         Image = new Bitmap(tbPath.Text),
                         MetaInfo = Guid.NewGuid().ToString(),
                         TemplateType = templateType
                     };
            }
        }
        #endregion
    }
}
