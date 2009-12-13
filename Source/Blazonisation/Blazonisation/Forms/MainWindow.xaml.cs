//----------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="BNTU Inc.">
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
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using BLL;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants
        private const string MSG_STARTED = "Started . . .";
        private const string STATE_PATTERN = "Recognition state : {0}";
        private const string MSG_READY = "Ready for work";
        private const string CROPPED_EMBLEM_PATH = "Cropped emblem.bmp";
        #endregion

        #region Fields
        private string pathToEmblem;
        private RubberbandAdorner cropSelector;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Menu items events
        private void On_miOpen_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
                                     {
                                         Filter = "Images | *.bmp; *.png; *.jpg; *.jpeg",
                                         Multiselect = false
                                     };

            if (!((bool)fileOpenDialog.ShowDialog(this)))
                return;

            pathToEmblem = fileOpenDialog.FileName;
            imgEmblemHolder.Source = new BitmapImage(new Uri(pathToEmblem));
            imgEmblemHolder.Visibility = Visibility.Visible;
            brdEmblemHolder.Visibility = Visibility.Visible;
        }

        private void On_miSaveResult_Click(object sender, RoutedEventArgs e)
        {
            // Note: add implementation
        }

        private void On_miExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Events handling
        private void On_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lblState.Visibility = Visibility.Visible;
            SetRecognitionState(MSG_READY);
            TurnOnCropSelector();
        }

        private void On_brdEmblemHolder_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            brdEmblemHolder.MouseLeftButtonDown -= On_brdEmblemHolder_MouseLeftButtonDown;
            imgEmblemHolder.ToolTip = null;
            Animate();
            SetRecognitionState(MSG_STARTED);
        }

        private void On_imgEmblemHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var anchor = e.GetPosition(imgEmblemHolder);
            cropSelector.CaptureMouse();
            cropSelector.StartSelection(anchor);
            // CropButton.IsEnabled = true;
        }

        private void On_btnStartRecognition_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Wait;

            // find shield
            var shieldFinder = new ShieldFinder(new Bitmap(pathToEmblem));
            var pathCroppedEmblem = AppDomain.CurrentDomain.BaseDirectory + CROPPED_EMBLEM_PATH;
            var uriCroppedEmblem = new Uri(pathCroppedEmblem);

            if (File.Exists(pathCroppedEmblem))
                File.Delete(pathCroppedEmblem);

            shieldFinder.emblem.Save(pathCroppedEmblem);
            imgEmblemHolder.Source = new BitmapImage(uriCroppedEmblem);

            cropSelector.DrawSelection(ScaleRectangle(shieldFinder.ShieldRect));
            // ---
            shieldFinder.Shield.Save(AppDomain.CurrentDomain.BaseDirectory + "Shield.bmp");
            // ---
            // find figures
            var figureDefiner = new FigureDefiner(shieldFinder.Shield);
            MessageBox.Show(figureDefiner.resultOutput);

            Cursor = Cursors.Arrow;
        }

        private void On_MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = e.NewSize.Width;
            Height = e.NewSize.Height;
            if (!String.IsNullOrEmpty(pathToEmblem))
                Animate();
        }
        #endregion

        #region Methods

        private void TurnOnCropSelector()
        {
            var layer = AdornerLayer.GetAdornerLayer(imgEmblemHolder);
            cropSelector = new RubberbandAdorner(imgEmblemHolder)
                               {
                                   Window = this
                               };
            layer.Add(cropSelector);
            //#if VISUALCHILD
            //            cropSelector.Rubberband.Visibility = Visibility.Hidden;
            //#endif
            //#if NoVISUALCHILD
            //            cropSelector.ShowRect = false;
        }

        private Rectangle ScaleRectangle(Rectangle rectangle)
        {
            var scaleX = imgEmblemHolder.ActualWidth / imgEmblemHolder.Source.Width;
            var scaleY = imgEmblemHolder.ActualHeight / imgEmblemHolder.Source.Height;

            return new Rectangle(
                                Convert.ToInt32(rectangle.X * scaleX),
                                Convert.ToInt32(rectangle.Y * scaleY),
                                Convert.ToInt32(rectangle.Width * scaleX),
                                Convert.ToInt32(rectangle.Height * scaleY)
                                );
        }

        private void SetRecognitionState(string state)
        {
            lblState.Content = String.Format(STATE_PATTERN, state);
        }

        private void Animate()
        {
            brdEmblemHolder.VerticalAlignment = VerticalAlignment.Center;
            brdEmblemHolder.HorizontalAlignment = HorizontalAlignment.Left;
            brdEmblemHolder.Margin = new Thickness(65, 0, 0, 0);

            var x = Width * 0.30;
            var shieldY = Height * 0.07;
            var colorsY = Height * 0.27;
            var devisionsY = Height * 0.47;
            var figuresY = Height * 0.67;

            var shieldThicknessAnimation = new ThicknessAnimation
            {
                From = brdShieldHolder.Margin,
                To = new Thickness(0, shieldY, x, 0),
                Duration = new Duration(TimeSpan.Parse("0:0:0.3")),
                AutoReverse = false
            };

            brdShieldHolder.BeginAnimation(MarginProperty, shieldThicknessAnimation);

            var colorsThicknessAnimation = new ThicknessAnimation
                                                {
                                                    From = brdColorsHolder.Margin,
                                                    To = new Thickness(0, colorsY, x, 0),
                                                    Duration = new Duration(TimeSpan.Parse("0:0:0.3")),
                                                    AutoReverse = false
                                                };

            brdColorsHolder.BeginAnimation(MarginProperty, colorsThicknessAnimation);

            var devisionsThicknessAnimation = new ThicknessAnimation
                                                  {
                                                      From = brdDevisionHolder.Margin,
                                                      To = new Thickness(0, devisionsY, x, 0),
                                                      Duration = new Duration(TimeSpan.Parse("0:0:0.3")),
                                                      AutoReverse = false
                                                  };

            brdDevisionHolder.BeginAnimation(MarginProperty, devisionsThicknessAnimation);

            var figuresThicknessAnimation = new ThicknessAnimation
                                       {
                                           From = brdFiguresHolder.Margin,
                                           To = new Thickness(0, figuresY, x, 0),
                                           Duration = new Duration(TimeSpan.Parse("0:0:0.3")),
                                           AutoReverse = false
                                       };

            brdFiguresHolder.BeginAnimation(MarginProperty, figuresThicknessAnimation);

            var buttonVisibilityAnimation = new DoubleAnimation
                                                {
                                                    From = 0,
                                                    To = 1,
                                                    Duration = new Duration(TimeSpan.Parse("0:0:0.5"))
                                                };
            btnStartRecognition.BeginAnimation(OpacityProperty, buttonVisibilityAnimation);

        }
        #endregion
    }
}
