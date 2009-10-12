using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Media.Animation;

namespace Blazonisation.Forms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        #region Constants
        private const string MSG_STARTED = "Started . . .";
        private const string STATE_PATTERN = "Recognition state : {0}";
        #endregion

        #region Fields
        private string pathToEmblem;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Menu items events
        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
                                     {
                                         Filter = "Images | *.bmp; *.png; *.jpg; *.jpeg",
                                         Multiselect = false
                                     };
            if ((bool)fileOpenDialog.ShowDialog(this))
            {
                pathToEmblem = fileOpenDialog.FileName;
                imgEmblemHolder.Source = new BitmapImage(new Uri(pathToEmblem));
                imgEmblemHolder.Visibility = Visibility.Visible;
                brdEmblemHolder.Visibility = Visibility.Visible;
                lblState.Visibility = Visibility.Visible;
            }
        }

        private void miSaveResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Events
        private void brdEmblemHolder_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            brdEmblemHolder.MouseLeftButtonDown -= brdEmblemHolder_MouseLeftButtonDown;
            Animate();
            SetRecognitionState(MSG_STARTED);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = e.NewSize.Width;
            Height = e.NewSize.Height;
        }
        #endregion

        #region Methods
        private void SetRecognitionState(string state)
        {
            lblState.Content = String.Format(STATE_PATTERN, state);
        }

        private void Animate()
        {
            var d = new DoubleAnimation
                        {
                            From = 0.0,
                            To = (-Width / 2 + brdEmblemHolder.Width / 2 + leftLine.Margin.Left + 30),
                            Duration = new Duration(TimeSpan.Parse("0:0:0.3")),
                            AutoReverse = false
                        };
            brdTranslateEmblem.BeginAnimation(TranslateTransform.XProperty, d);

            //          Components animation
            // Shield
            var daBrdShield = new DoubleAnimation
                                  {
                                      From = 0.0,
                                      To = Width * .6,
                                      Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                      AutoReverse = false
                                  };

            brdTranslateShield.BeginAnimation(TranslateTransform.XProperty, daBrdShield);

            daBrdShield = new DoubleAnimation
                              {
                                  From = 0.0,
                                  To = .25 * Height,
                                  Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                  AutoReverse = false
                              };
            brdTranslateShield.BeginAnimation(TranslateTransform.YProperty, daBrdShield);
            //  Devision
            var daBrdDevision = new DoubleAnimation
                                    {
                                        From = 0.0,
                                        To = Width * .6,
                                        Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                        AutoReverse = false
                                    };

            brdTranslateDivision.BeginAnimation(TranslateTransform.XProperty, daBrdDevision);

            daBrdDevision = new DoubleAnimation
                                {
                                    From = 0.0,
                                    To = -.5 * Height,
                                    Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                    AutoReverse = false
                                };
            brdTranslateDivision.BeginAnimation(TranslateTransform.YProperty, daBrdDevision);
            // Colors
            var daBrdColors = new DoubleAnimation
                                  {
                                      From = 0.0,
                                      To = -Width / 2,
                                      Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                      AutoReverse = false
                                  };

            brdTranslateColors.BeginAnimation(TranslateTransform.XProperty, daBrdColors);

            daBrdColors = new DoubleAnimation
                              {
                                  From = 0.0,
                                  To = -.7 * Height,
                                  Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                  AutoReverse = false
                              };
            brdTranslateColors.BeginAnimation(TranslateTransform.YProperty, daBrdColors);
            // Figures
            var daBrdFigures = new DoubleAnimation
                                   {
                                       From = 0.0,
                                       To = -Width / 2,
                                       Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                       AutoReverse = false
                                   };

            brdTranslateFigures.BeginAnimation(TranslateTransform.XProperty, daBrdFigures);

            daBrdFigures = new DoubleAnimation
                               {
                                   From = 0.0,
                                   To = .8 * Height,
                                   Duration = new Duration(TimeSpan.Parse("0:0:1")),
                                   AutoReverse = false
                               };
            brdTranslateFigures.BeginAnimation(TranslateTransform.YProperty, daBrdFigures);
        }
        #endregion
    }
}

