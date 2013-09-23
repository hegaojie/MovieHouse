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

        public static readonly DependencyProperty TriggerDetailsAnimationProperty =
            DependencyProperty.RegisterAttached("TriggerDetailsAnimation", typeof(string), typeof(AnimationManager),
            new PropertyMetadata(string.Empty, OnTriggerDetailsAnimationPropertyChanged));

        public static void SetTriggerAnimation(UIElement e, object value)
        {
            e.SetValue(TriggerAnimationProperty, value);
        }

        public static string GetTriggerAnimation(UIElement e)
        {
            return (string)e.GetValue(TriggerAnimationProperty);
        }

        public static void SetTriggerDetailsAnimation(UIElement e, object value)
        {
            e.SetValue(TriggerDetailsAnimationProperty, value);
        }

        public static string GetTriggerDetailsAnimation(UIElement e)
        {
            return (string)e.GetValue(TriggerDetailsAnimationProperty);
        }

        private const double AnimationMilliseconds = 300;

        private static void OnTriggerAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MovieViewContainer;
            var mvm = control.Tag as MovieBriefViewModel;
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

        private static void OnTriggerDetailsAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ContentControl;
            
            var height = (double)d.GetValue(ContentControl.HeightProperty);
            var from = (double)d.GetValue(Canvas.TopProperty);
            var to = from < 0 ? from + height : from - height;

            var sb = new Storyboard();
            var ctA = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            Storyboard.SetTargetProperty(ctA, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(ctA);

            control.BeginStoryboard(sb);
        }
    }
}