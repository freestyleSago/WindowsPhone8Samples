using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.License
{
    internal class OpacityAnimation
    {
        private Storyboard storyboard;
        private DoubleAnimation animation;

        public OpacityAnimation(UIElement target)
        {
            this.storyboard = new Storyboard();
            Storyboard.SetTarget(this.storyboard, target);

            this.animation = new DoubleAnimation();
            Storyboard.SetTargetProperty(this.animation, "Opacity");

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

        public Duration Duration
        {
            get
            {
                return this.animation.Duration;
            }
            set
            {
                this.animation.Duration = value;
            }
        }

        public void Start(double to)
        {
            this.animation.To = to;
            this.storyboard.Begin();
        }
    }
}
