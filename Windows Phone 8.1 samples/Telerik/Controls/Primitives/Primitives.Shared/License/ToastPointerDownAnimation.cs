using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.License
{
    internal class ToastPointerDownAnimation
    {
        private PlaneProjection projection;
        private Storyboard storyboard;
        private DoubleAnimation rotationYAnimation;

        public ToastPointerDownAnimation(UIElement element)
        {
            this.RotationY = 10;
            this.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            this.projection = new PlaneProjection();
            this.projection.CenterOfRotationX = 1;

            this.rotationYAnimation = new DoubleAnimation();
            Storyboard.SetTargetProperty(this.rotationYAnimation, "RotationY");

            this.storyboard = new Storyboard();
            this.storyboard.Children.Add(this.rotationYAnimation);
            Storyboard.SetTarget(this.storyboard, this.projection);

            element.Projection = this.projection;
        }

        public double RotationY
        {
            get;
            set;
        }

        public Duration Duration
        {
            get;
            set;
        }

        public void Start()
        {
            if (this.rotationYAnimation.To == this.RotationY)
            {
                return;
            }

            this.rotationYAnimation.To = this.RotationY;
            this.rotationYAnimation.Duration = this.Duration;

            this.storyboard.Begin();
        }

        public void Stop()
        {
            if (this.rotationYAnimation.To == 0)
            {
                return;
            }

            this.rotationYAnimation.To = 0;
            this.storyboard.Begin();
        }
    }
}
