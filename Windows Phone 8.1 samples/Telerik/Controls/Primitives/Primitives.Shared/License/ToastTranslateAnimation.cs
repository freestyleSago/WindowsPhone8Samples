using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.License
{
    internal class ToastTranslateAnimation
    {
        private static Duration defaultDuration = new Duration(TimeSpan.FromMilliseconds(100));
        private static Duration immediateDuration = new Duration(TimeSpan.FromMilliseconds(1));

        private Storyboard storyboard;
        private DoubleAnimation animation;
        private TranslateTransform transform;
        private FrameworkElement target;

        public ToastTranslateAnimation(FrameworkElement target)
        {
            this.transform = new TranslateTransform();
            this.target = target;
            this.target.RenderTransform = this.transform;

            this.animation = new DoubleAnimation();
            this.animation.EasingFunction = new ExponentialEase() { Exponent = 2, EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTargetProperty(this.animation, "X");

            this.storyboard = new Storyboard();
            Storyboard.SetTarget(this.storyboard, this.transform);
            this.storyboard.Children.Add(this.animation);

            this.storyboard.Completed += (s, e) =>
            {
                if (this.Completed != null)
                {
                    this.Completed(this, EventArgs.Empty);
                }
            };
        }

        public event EventHandler Completed;

        public double Position
        {
            get
            {
                return this.transform.X;
            }
            set
            {
                this.transform.X = value;
            }
        }

        public void Start(double to)
        {
            if (this.animation.To == to)
            {
                return;
            }

            this.animation.To = to;
            this.animation.Duration = defaultDuration;
            this.storyboard.Begin();
        }

        public void MoveTo(double offset)
        {
            this.storyboard.SkipToFill();

            if (offset < 0 && this.Position <= 0)
            {
                double renderWidth = this.target.ActualWidth / 3;
                double negativeOffset = Math.Min(Math.Abs(this.Position), renderWidth);
                double scale = 1 - negativeOffset / renderWidth;
                offset *= scale;
            }

            double newPosition = this.Position + offset;

            this.animation.To = newPosition;
            this.animation.Duration = immediateDuration;

            this.storyboard.Begin();
        }
    }
}
