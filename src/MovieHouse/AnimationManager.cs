using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MovieHouse
{
    public class AnimationManager
    {
        public static readonly DependencyProperty TriggerAnimationProperty =
            DependencyProperty.RegisterAttached("TriggerAnimation", typeof(string), typeof(AnimationManager),
            new PropertyMetadata(string.Empty, OnTriggerAnimationPropertyChanged));

        public static void SetTriggerAnimation(UIElement e, object value)
        {
            e.SetValue(TriggerAnimationProperty, value);
        }

        public static string GetTriggerAnimation(UIElement e)
        {
            return (string)e.GetValue(TriggerAnimationProperty);
        }

        private const double AnimationMilliseconds = 300;

        private static void OnTriggerAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MovieViewContainer;
            var mvm = control.Tag as MovieViewModel;
            if (mvm == null) { return; }

            var curWidth = mvm._originalWidth * mvm.Scale;
            var tagWidth = mvm._originalWidth * mvm.NewScale;
            var curHeight = mvm._originalHeight * mvm.Scale;
            var tagHeight = mvm._originalHeight * mvm.NewScale;
            var newOffsetX = mvm.NewCenterX - (mvm._originalWidth * mvm.NewScale) / 2;
            var newOffsetY = mvm.NewCenterY - (mvm._originalHeight * mvm.NewScale) / 2;

            var sb = new Storyboard();

            var wA = new DoubleAnimation(curWidth, tagWidth, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            Storyboard.SetTargetProperty(wA, new PropertyPath("(Width)"));
            sb.Children.Add(wA);

            var hA = new DoubleAnimation(curHeight, tagHeight, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            Storyboard.SetTargetProperty(hA, new PropertyPath("(Height)"));
            sb.Children.Add(hA);

            var oA = new DoubleAnimation(mvm.Opacity, mvm.NewOpacity, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            Storyboard.SetTargetProperty(oA, new PropertyPath("(Opacity)"));
            sb.Children.Add(oA);

            var clA = new DoubleAnimation(mvm.OffsetX, newOffsetX, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            Storyboard.SetTargetProperty(clA, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(clA);

            var ctA = new DoubleAnimation(mvm.OffsetY, newOffsetY, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            Storyboard.SetTargetProperty(ctA, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(ctA);

            control.SetValue(Canvas.ZIndexProperty, mvm.NewZIndex);

            sb.CurrentStateInvalidated += (sender, args) =>
                                             {
                                                 var clock = sender as Clock;
                                                 if (clock.CurrentState == ClockState.Active) { return; }

                                                 mvm.Scale = mvm.NewScale;
                                                 mvm.Opacity = mvm.NewOpacity;
                                                 mvm.CenterX = mvm.NewCenterX;
                                                 mvm.CenterY = mvm.NewCenterY;
                                                 mvm.ZIndex = mvm.NewZIndex;
                                             };

            control.BeginStoryboard(sb);
        }
    }
}