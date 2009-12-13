#define VISUALCHILD
namespace Blazonisation.BLL
{
    using System;
    // using System.Drawing;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using Forms;

    public class RubberbandAdorner : Adorner
    {
        #region Fields
        private MainWindow window;
        private RectangleGeometry rectGeometry;
        private System.Windows.Shapes.Path rubberband;
        private UIElement adornedElement;
        private Rect selectRect;
        private Point anchorPoint;
        #endregion

        #region Properties
        public Rect SelectRect { get { return selectRect; } }
        public System.Windows.Shapes.Path Rubberband { get { return rubberband; } }
        protected override int VisualChildrenCount { get { return 1; } }
        public MainWindow Window { set { window = value; } }

        #endregion

        #region Contructors
        public RubberbandAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.adornedElement = adornedElement;
            selectRect = new Rect();
            rectGeometry = new RectangleGeometry();
            rubberband = new System.Windows.Shapes.Path
                             {
                                 Data = rectGeometry,
                                 StrokeThickness = 3,
                                 Stroke = Brushes.Black,
                                 Opacity = .75,
                                 Visibility = Visibility.Visible
                             };
            AddVisualChild(rubberband);
            MouseMove += DrawSelection;
            MouseUp += EndSelection;
        }
        #endregion

        #region Methods
        protected override Size ArrangeOverride(Size size)
        {
            System.Windows.Size finalSize = base.ArrangeOverride(size);
            ((UIElement)GetVisualChild(0)).Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }

        public void StartSelection(Point anchPoint)
        {
            anchorPoint = anchPoint;
            selectRect.Size = new Size(10, 10);
            selectRect.Location = anchorPoint;
            rectGeometry.Rect = selectRect;
            if (Visibility.Visible != rubberband.Visibility)
                rubberband.Visibility = Visibility.Visible;
        }

        public void DrawSelection(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point mousePosition = e.GetPosition(adornedElement);
                if (mousePosition.X < anchorPoint.X)
                    selectRect.X = mousePosition.X;
                else
                    selectRect.X = anchorPoint.X;
                if (mousePosition.Y < anchorPoint.Y)
                    selectRect.Y = mousePosition.Y;
                else
                    selectRect.Y = anchorPoint.Y;
                selectRect.Width = Math.Abs(mousePosition.X - anchorPoint.X);
                selectRect.Height = Math.Abs(mousePosition.Y - anchorPoint.Y);
                rectGeometry.Rect = selectRect;
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(adornedElement);
                layer.InvalidateArrange();
            }
        }

        internal void DrawSelection(System.Drawing.Rectangle rectangle)
        {
            rubberband.Visibility = Visibility.Visible;
            selectRect = ConvertRectangleToRect(rectangle);
            rectGeometry.Rect = selectRect;
            var layer = AdornerLayer.GetAdornerLayer(adornedElement);
            layer.InvalidateArrange();
        }

        private void EndSelection(object sender, MouseButtonEventArgs e)
        {
            if (3 >= selectRect.Width || 3 >= selectRect.Height)
                rubberband.Visibility = Visibility.Hidden;
            else
                window.btnStartRecognition.IsEnabled = true;
            ReleaseMouseCapture();
        }

        protected override Visual GetVisualChild(int index)
        {
            return rubberband;
        }

        private static Rect ConvertRectangleToRect(System.Drawing.Rectangle rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
        #endregion
    }
}