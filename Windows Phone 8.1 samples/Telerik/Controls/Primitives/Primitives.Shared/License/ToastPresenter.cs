using System;
using Telerik.UI.Xaml.Controls.Primitives.License;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.License
{
    internal class ToastPresenter : ContentControl
    {
        private ToastPointerDownAnimation pointerDownAnimation;
        private OpacityAnimation closeButtonAnimation;
        private OpacityAnimation fadeInOutAnimation;
        private ToastTranslateAnimation translateAnimation;
        private bool isPointerOver;
        private bool touchPresent;
        private ToastState state;
        private Button closeButton;

        private DispatcherTimer dismissTimer = new DispatcherTimer();
        private DispatcherTimer initialDelayTimer;

        public ToastPresenter()
        {
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            this.pointerDownAnimation = new ToastPointerDownAnimation(this);

            this.translateAnimation = new ToastTranslateAnimation(this);
            this.translateAnimation.Completed += this.OnDismissed;

            this.fadeInOutAnimation = new OpacityAnimation(this) { Duration = new Duration(TimeSpan.FromSeconds(1)) };
            this.fadeInOutAnimation.Completed += this.OnDismissed;

            this.dismissTimer.Interval = TimeSpan.FromSeconds(5);
            this.dismissTimer.Tick += this.OnDismissTimerTick;

            this.initialDelayTimer = new DispatcherTimer();
            this.initialDelayTimer.Interval = TimeSpan.FromSeconds(3);
            this.initialDelayTimer.Tick += this.OnInitialDelayTimerTick;

            this.Opacity = 0;
        }

        public event EventHandler Dismissed;

        private enum ToastState : short
        {
            Displayed,
            UserDismissed,
            FadingOut,
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e == null)
            {
                return;
            }

            this.Focus(FocusState.Pointer);
            this.CapturePointer(e.Pointer);
            this.InitManipulation(e.Pointer);

            this.UpdateState(e);

            this.pointerDownAnimation.Start();

            this.isPointerOver = true;
        }

        /// <summary>
        /// Called before the PointerCaptureLost event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);

            this.pointerDownAnimation.Stop();
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);

            if (e == null)
            {
                return;
            }

            this.UpdateState(e);

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            this.closeButton.IsHitTestVisible = true;
            this.closeButtonAnimation.Start(1);
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            if (e == null)
            {
                return;
            }

            this.UpdateDismissTimer(e);

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            this.closeButton.IsHitTestVisible = false;
            this.closeButtonAnimation.Start(0);
        }

        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e == null)
            {
                return;
            }

            this.UpdateState(e);

            if (this.touchPresent || !e.Pointer.IsInContact)
            {
                return;
            }

            if (!this.HitTest(e))
            {
                if (this.isPointerOver)
                {
                    this.isPointerOver = false;
                    this.pointerDownAnimation.Stop();
                }
            }
            else if (!this.isPointerOver)
            {
                this.isPointerOver = true;
                this.pointerDownAnimation.Start();
            }
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e == null)
            {
                return;
            }

            this.ReleasePointerCaptures();
            this.pointerDownAnimation.Stop();
            this.UpdateState(e);

            if (!this.touchPresent)
            {
                return;
            }

            if (this.translateAnimation.Position >= this.ActualWidth / 2)
            {
                this.DismissWithTranslate();
            }
            else
            {
                this.translateAnimation.Start(0);
            }
        }

        /// <summary>
        /// Called before the ManipulationDelta event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);

            if (e == null)
            {
                return;
            }

            this.pointerDownAnimation.Stop();

            if (!e.IsInertial)
            {
                this.translateAnimation.MoveTo(e.Delta.Translation.X);
            }
            else if (e.Delta.Translation.X > 0 && this.state == ToastState.Displayed)
            {
                this.DismissWithTranslate();
            }
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (this.state != ToastState.UserDismissed)
            {
                var warningSuppressionVariable = Launcher.LaunchUriAsync(new Uri(TelerikLicense.PurchaseUrl));
            }

            this.DismissWithTranslate();
        }

        private bool HitTest(PointerRoutedEventArgs e)
        {
            Rect bounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            return bounds.Contains(e.GetCurrentPoint(this).Position);
        }

        private void UpdateDismissTimer(PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact)
            {
                this.dismissTimer.Stop();
                return;
            }

            if (this.HitTest(e))
            {
                this.dismissTimer.Stop();
            }
            else
            {
                this.dismissTimer.Start();
            }
        }

        private void OnDismissed(object sender, EventArgs e)
        {
            if (this.state == ToastState.Displayed)
            {
                return;
            }

            EventHandler eh = this.Dismissed;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.DismissWithTranslate();
        }

        private void OnDismissTimerTick(object sender, object e)
        {
            this.DismissWithFadeOut();
        }

        private void OnInitialDelayTimerTick(object sender, object e)
        {
            this.initialDelayTimer.Stop();

            this.fadeInOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            this.fadeInOutAnimation.Start(1);

            this.translateAnimation.Position = 20;
            this.translateAnimation.Start(0);

            this.dismissTimer.Start();
        }

        private void DismissWithTranslate()
        {
            this.state = ToastState.UserDismissed;
            this.translateAnimation.Start(this.ActualWidth);
        }

        private void DismissWithFadeOut()
        {
            this.state = ToastState.FadingOut;
            this.fadeInOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            this.fadeInOutAnimation.Start(0);
        }

        private void UpdateState(PointerRoutedEventArgs e)
        {
            this.UpdateDismissTimer(e);

            if (this.state == ToastState.FadingOut)
            {
                this.state = ToastState.Displayed;
                this.fadeInOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                this.fadeInOutAnimation.Start(1);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.dismissTimer.Stop();
            this.initialDelayTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.closeButton = ElementTreeHelper.FindVisualDescendant<Button>(this);
            this.closeButton.Click += this.OnCloseButtonClick;
            this.closeButtonAnimation = new OpacityAnimation(this.closeButton) { Duration = new Duration(TimeSpan.FromMilliseconds(200)) };
            this.initialDelayTimer.Start();
        }

        private void InitManipulation(Pointer pointer)
        {
            this.touchPresent = pointer.PointerDeviceType == PointerDeviceType.Touch;
            if (this.touchPresent)
            {
                this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
            }
            else
            {
                this.ManipulationMode = ManipulationModes.System;
            }
        }
    }
}
