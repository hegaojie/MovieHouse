using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MovieHouse
{
    public class ReflectionControl : Decorator
    {
        #region Private Fields

        private readonly VisualBrush _reflection;
        private readonly LinearGradientBrush _opacityMask;
        private const int _gap = 5;

        #endregion

        #region Constructor

        public ReflectionControl()
        {
            // Set defaults for this control
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Center;

            // Create brushes were going to use
            _opacityMask = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };

            _opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0));
            _opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 0.2));

            _reflection = new VisualBrush
            {
                RelativeTransform = new ScaleTransform(1, -1, 0.5, 0.5)
            };
        }

        #endregion

        #region Public Override

        protected override Size MeasureOverride(Size constraint)
        {
            // We need twice the space that our content needs
            if (Child == null) return new Size(0, 0);

            Child.Measure(constraint);
            return new Size(Child.DesiredSize.Width, Child.DesiredSize.Height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // always put out content at the upper half of the control
            if (Child == null) return new Size(0, 0);

            Child.Arrange(new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height));
            return arrangeBounds;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // draw everything except the reflection
            base.OnRender(drawingContext);

            // set opacity
            drawingContext.PushOpacityMask(_opacityMask);
            drawingContext.PushOpacity(0.4);

            // set reflection parameters based on content size
            _reflection.Visual = Child;

            // draw the reflection
            drawingContext.DrawRectangle(_reflection, null, new Rect(0, ActualHeight + _gap, ActualWidth, ActualHeight));

            // cleanup
            drawingContext.Pop();
            drawingContext.Pop();
        }

        #endregion
    }
}