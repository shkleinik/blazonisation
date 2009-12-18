//----------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="BNTU Inc.">
//     Copyright (c) BNTU Inc. All rights reserved.
// </copyright>
// <author>Alexander Kanaukou, Helen Grihanova, Maksim Zui, Pavel Shkleinik</author>
//----------------------------------------------------------------------------------

namespace Blazonisation.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using BLL;
    using BLL.Colors;
    using BLL.Devisions;
    using BLL.Figures;
    using BLL.ShieldForm;
    using BLL.ShieldLocation;
    using DAL;
    using Microsoft.Win32;
    using C = BLL.Common;
    using TM = DAL.TemplatesManager;
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants
        private const string STATE_PATTERN = "Recognition state : {0}";
        private const string MSG_READY = "Ready for work";
        private const string MSG_STARTED = "Started . . .";
        private const string MSG_FINISHED = "Recognition completed.";
        private const string MSG_TOOLTIP = "Click to start recognition";
        private const string ALBOM_NAME = "Geraldiceskii_Albom.pdf";
        private const string PATH_PATTERN = @"{0}\Foxit Reader\Foxit Reader.exe";
        #endregion

        #region Fields
        private string pathToEmblem;
        private bool isRecognitionStarted;
        private RubberbandAdorner cropSelector;
        private ShieldFinder shieldFinder;

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

            isRecognitionStarted = false;
            pathToEmblem = fileOpenDialog.FileName;
            imgEmblemHolder.Source = new BitmapImage(new Uri(pathToEmblem));
            imgEmblemHolder.Visibility = Visibility.Visible;
            brdEmblemHolder.Visibility = Visibility.Visible;
            brdEmblemHolder.MouseLeftButtonDown += On_brdEmblemHolder_MouseLeftButtonDown;
            imgEmblemHolder.ToolTip = MSG_TOOLTIP;
            ResetThumbnails();
            SetRecognitionState(MSG_READY);
        }

        private void On_miManageTemplates_Click(object sender, RoutedEventArgs e)
        {
            var templatesForm = new TemplatesManager();
            templatesForm.ShowDialog();
        }

        private void On_miSaveResult_Click(object sender, RoutedEventArgs e)
        {
            // Note: add implementation
        }

        private void On_miHelp_Click(object sender, RoutedEventArgs e)
        {
            var foxReaderExecutable = String.Format(PATH_PATTERN, (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)));
            Process.Start(foxReaderExecutable, ALBOM_NAME);
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
            if (isRecognitionStarted)
                return;

            brdEmblemHolder.MouseLeftButtonDown -= On_brdEmblemHolder_MouseLeftButtonDown;
            imgEmblemHolder.ToolTip = null;
            isRecognitionStarted = true;
            SetRecognitionState(MSG_STARTED);
            FindShield();
            AnimateThumbnails();
            AnimateTextBlocks();

            cropSelector.Visibility = Visibility.Visible;

            var result = MessageBox.Show("Was shield located correctly?",
                                        "Shield search result",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                ShowContinueButton();
            }
            else
            {
                imgEmblemHolder.Source = C.ConvertBitmapToBitmapImage(shieldFinder.Shield);
                cropSelector.Visibility = Visibility.Hidden;
                RecognizeElements(shieldFinder.Shield);
            }
        }

        private void On_imgEmblemHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isRecognitionStarted)
                return;

            var anchor = e.GetPosition(imgEmblemHolder);
            cropSelector.CaptureMouse();
            cropSelector.StartSelection(anchor);
        }

        private void On_btnContinueRecognition_Click(object sender, EventArgs e)
        {
            var shield = ShieldFinder.GetImagePart(shieldFinder.emblem, ZoomRectangle(cropSelector.Rectangle));
            shield.Save(@"d:\cropped_shield.bmp");
            imgEmblemHolder.Source = C.ConvertBitmapToBitmapImage(shield);
            cropSelector.Visibility = Visibility.Hidden;
            HideContinueButton();
            RecognizeElements(shield);
        }

        private void On_MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(pathToEmblem))
                return;

            AnimateThumbnails();
            AnimateTextBlocks();
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
        }

        private void SetEmblemImageSource(Bitmap bitmap)
        {
            imgEmblemHolder.Source = C.ConvertBitmapToBitmapImage(bitmap);
        }

        private Bitmap GetCurrentEmblem()
        {
            return C.ConvertBitmapImageToBitmap(imgEmblemHolder.Source as BitmapImage);
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

        private Rectangle ZoomRectangle(Rectangle rectangle)
        {
            var scaleX = imgEmblemHolder.Source.Width / imgEmblemHolder.ActualWidth;
            var scaleY = imgEmblemHolder.Source.Height / imgEmblemHolder.ActualHeight;

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

        private void AnimateThumbnails()
        {
            if (!isRecognitionStarted)
                return;

            brdEmblemHolder.VerticalAlignment = VerticalAlignment.Center;
            brdEmblemHolder.HorizontalAlignment = HorizontalAlignment.Left;
            brdEmblemHolder.Margin = new Thickness(65, 0, 0, 0);

            var x = brdEmblemHolder.ActualWidth + 100;
            var shieldY = ActualHeight * 0.07;
            var colorsY = ActualHeight * 0.27;
            var devisionsY = ActualHeight * 0.47;
            var figuresY = ActualHeight * 0.67;
            var animationDuratation = "0:0:0.7";

            var shieldThicknessAnimation = new ThicknessAnimation
                                                {
                                                    From = brdShieldHolder.Margin,
                                                    To = new Thickness(x, shieldY, 0, 0),
                                                    Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                                                    AutoReverse = false
                                                };

            brdShieldHolder.BeginAnimation(MarginProperty, shieldThicknessAnimation);

            var colorsThicknessAnimation = new ThicknessAnimation
                                                {
                                                    From = brdColorsHolder.Margin,
                                                    To = new Thickness(x, colorsY, 0, 0),
                                                    Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                                                    AutoReverse = false
                                                };

            brdColorsHolder.BeginAnimation(MarginProperty, colorsThicknessAnimation);

            var devisionsThicknessAnimation = new ThicknessAnimation
                                                  {
                                                      From = brdDevisionHolder.Margin,
                                                      To = new Thickness(x, devisionsY, 0, 0),
                                                      Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                                                      AutoReverse = false
                                                  };

            brdDevisionHolder.BeginAnimation(MarginProperty, devisionsThicknessAnimation);

            var figuresThicknessAnimation = new ThicknessAnimation
                                                   {
                                                       From = brdFiguresHolder.Margin,
                                                       To = new Thickness(x, figuresY, 0, 0),
                                                       Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                                                       AutoReverse = false
                                                   };

            brdFiguresHolder.BeginAnimation(MarginProperty, figuresThicknessAnimation);
        }

        private void AnimateTextBlocks()
        {
            if (!isRecognitionStarted)
                return;

            var x = brdEmblemHolder.ActualWidth + 100 + 130;
            var shieldY = ActualHeight * 0.07;
            var colorsY = ActualHeight * 0.27;
            var devisionsY = ActualHeight * 0.47;
            var figuresY = ActualHeight * 0.67;
            var animationDuratation = "0:0:0.7";


            var shieldThicknessAnimation = new ThicknessAnimation
            {
                From = brdShieldHolder.Margin,
                To = new Thickness(x, shieldY, 0, 0),
                Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                AutoReverse = false
            };

            tbShieldForm.BeginAnimation(MarginProperty, shieldThicknessAnimation);

            var colorsThicknessAnimation = new ThicknessAnimation
            {
                From = brdColorsHolder.Margin,
                To = new Thickness(x, colorsY, 0, 0),
                Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                AutoReverse = false
            };

            tbColors.BeginAnimation(MarginProperty, colorsThicknessAnimation);

            var devisionsThicknessAnimation = new ThicknessAnimation
            {
                From = brdDevisionHolder.Margin,
                To = new Thickness(x, devisionsY, 0, 0),
                Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                AutoReverse = false
            };

            tbDevisions.BeginAnimation(MarginProperty, devisionsThicknessAnimation);

            var figuresThicknessAnimation = new ThicknessAnimation
            {
                From = brdFiguresHolder.Margin,
                To = new Thickness(x + 230, figuresY, 0, 0),
                Duration = new Duration(TimeSpan.Parse(animationDuratation)),
                AutoReverse = false
            };

            tbFigures.BeginAnimation(MarginProperty, figuresThicknessAnimation);
        }

        private void ResetThumbnails()
        {
            //imgShield.Source = new BitmapImage(new Uri(@"..\Images\Shield.png"));
            //imgColors.Source = new BitmapImage(new Uri(@"..\Images\Colors.png"));
            //imgDevision.Source = new BitmapImage(new Uri(@"..\Images\Devision.png"));
            //imgFigures.Source = new BitmapImage(new Uri(@"..\Images\Figures.png"));
        }

        private void ShowContinueButton()
        {
            var buttonVisibilityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.Parse("0:0:0.5"))
            };

            btnContinueRecognition.BeginAnimation(OpacityProperty, buttonVisibilityAnimation);
        }

        private void HideContinueButton()
        {
            var buttonVisibilityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.Parse("0:0:0.5"))
            };

            btnContinueRecognition.BeginAnimation(OpacityProperty, buttonVisibilityAnimation);
        }

        private String GetColorsRecognitionResultText(List<ColorDetails> possibleImportant, int sumHeightWidth)
        {
            var importantColors = ColorDetector.GetImportantColors(possibleImportant, sumHeightWidth);
            var message = String.Empty;
            var resultBmp = new Bitmap(200, 200);

            var colorsTemplates = TM.GetTemlates(TemplateType.Colors);
            var coincidedColors = new List<Template>();

            foreach (var colorTemplate in colorsTemplates)
            {
                foreach (var importantColor in importantColors)
                {
                    if (importantColor.ColorName != colorTemplate.MetaInfo)
                        continue;

                    coincidedColors.Add(colorTemplate);
                    message += String.Format("{0}: {1}\n", colorTemplate.MetaInfo, colorTemplate.Description);
                    resultBmp = AddBitmaps(resultBmp, colorTemplate.Image);
                }
            }

            imgColors.Source = C.ConvertBitmapToBitmapImage(resultBmp);

            return message;
        }

        private static Bitmap AddBitmaps(Bitmap bitmap1, Bitmap bitmap2)
        {
            var result = new Bitmap(bitmap1.Width, bitmap1.Height + bitmap2.Height);

            // Is first time?
            if (bitmap1.GetPixel(0, 0).R == 0 && bitmap1.GetPixel(0, 0).G == 0 && bitmap1.GetPixel(0, 0).B == 0)
                return bitmap2;

            // copy first part
            for (var y = 0; y < bitmap1.Height; y++)
            {
                for (var x = 0; x < bitmap1.Width; x++)
                {
                    result.SetPixel(x, y, bitmap1.GetPixel(x, y));
                }
            }

            // copy second part
            for (var y = bitmap1.Height; y < bitmap1.Height + bitmap2.Height; y++)
            {
                for (var x = 0; x < bitmap1.Width; x++)
                {
                    result.SetPixel(x, y, bitmap2.GetPixel(x, y - bitmap1.Height));
                }
            }

            return result;
        }

        #endregion

        #region Recognition
        private void ReduceEmblemColorDepth()
        {
            var emblemWithReducedColors = ImageColorConverter.AverageImageColorsFromImage(GetCurrentEmblem());
            SetEmblemImageSource(emblemWithReducedColors);

        }

        private void FindShield()
        {
            Cursor = Cursors.Wait;

            ReduceEmblemColorDepth();

            // find shield
            shieldFinder = new ShieldFinder(GetCurrentEmblem());

            SetEmblemImageSource(shieldFinder.emblem);
            cropSelector.DrawSelection(ScaleRectangle(shieldFinder.ShieldRect));

            Cursor = Cursors.Arrow;
        }

        private void RecognizeElements(Bitmap shield)
        {
            Cursor = Cursors.Wait;

            // Define shield form
            var templatesShieldForm = TM.GetTemlates(TemplateType.ShieldForm);
            var shieldFormDefiner = new ShieldFormDefiner(shield, C.ConvertTemplatesToBitmaps(templatesShieldForm));
            var formIndex = shieldFormDefiner.GetRes();
            tbShieldForm.Text = templatesShieldForm[formIndex].Description;
            imgShield.Source = templatesShieldForm[formIndex].BitmapImage;

            // determine important colors
            var colorDetails = ColorDetails.GetColorDetailsFull(shield);
            tbColors.Text = GetColorsRecognitionResultText(colorDetails, shield.Width + shield.Height);

            // return;

            // Find shield devisions
            var colors = SectionCreator.GetSectionsArray(shield);
            var colorDetailsForSections = new List<List<ColorDetails>>();
            for (var i = 0; i < colors.Length; i++)
            {
                var details = ColorDetails.GetColorDetailsPixelPercentage(colors[i]);
                colorDetailsForSections.Add(details);
            }
            // MessageBox.Show(GetDevisionCode(colorDetailsList));
            var devisionDefiner = new DivisionDefiner(shield);
            var devisionTemplate = TM.GetDevisionTemplateByCode(devisionDefiner.DevisionCode);
            if(devisionTemplate == null)
            {
                tbDevisions.Text = "Высока вероятность того, что этот щит не разделен.";
                imgDevision.Source = null;
            }
            else
            {
                tbDevisions.Text = devisionTemplate.Description;
                imgDevision.Source = devisionTemplate.BitmapImage;
            }

            // Define figures
            var templates = TM.GetTemlates(TemplateType.Figures);
            var shieldFiguresDefiner = new FigureDefiner(shield, C.ConvertTemplatesToBitmaps(templates));
            tbFigures.Text = templates[shieldFiguresDefiner.resultOutputInt[0]].Description;
            imgFigures.Source = templates[shieldFiguresDefiner.resultOutputInt[0]].BitmapImage;
            imgFigures2.Source = templates[shieldFiguresDefiner.resultOutputInt[1]].BitmapImage;
            imgFigures3.Source = templates[shieldFiguresDefiner.resultOutputInt[2]].BitmapImage;

            SetRecognitionState(MSG_FINISHED);
            Cursor = Cursors.Arrow;
        }
        #endregion
    }
}