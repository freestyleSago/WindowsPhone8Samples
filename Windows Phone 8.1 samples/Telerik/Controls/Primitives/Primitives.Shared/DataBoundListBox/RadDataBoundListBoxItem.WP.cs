
using System;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    public partial class RadDataBoundListBoxItem
    {
        //private ScrollState manipulationStartScrollState;
        private bool manipulationStartedHandled = false;

        internal virtual void OnItemManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            RadDataBoundListBox typedOwner = this.Owner as RadDataBoundListBox;
            if (typedOwner != null)
            {
                typedOwner.OnItemManipulationStarted(this, e.Container, e.Position);
            }
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        /// <param name="e">Do not use.</param>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            base.OnHolding(e);

            // TODO: Check availability in each version because of the "Do not use" comment
            if (this.ItemState == ItemState.Realized)
            {
                this.typedOwner.HideCheckBoxesPressIndicator();
            }

            this.typedOwner.OnItemHold(this, e);
        }

        private Point startPoint;

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                startPoint = e.GetCurrentPoint(this).Position;

                this.typedOwner.holdTimer.Tick -= holdTimer_Tick;
                this.typedOwner.holdTimer.Tick += holdTimer_Tick;
                this.typedOwner.holdTimer.Start();
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            var currentPoint =  e.GetCurrentPoint(this).Position;

            var xDistance = startPoint.X - currentPoint.X;
            var yDistance = startPoint.Y - currentPoint.Y;
            var distance = Math.Sqrt(xDistance*xDistance + yDistance*yDistance);

            if (this.typedOwner.holdTimer.IsEnabled && distance > 20)
            {
                this.typedOwner.holdTimer.Tick -= holdTimer_Tick;
                this.typedOwner.holdTimer.Stop();
            }
        }

        void holdTimer_Tick(object sender, object e)
        {
            if (this.typedOwner != null)
            {

                this.typedOwner.holdTimer.Tick -= holdTimer_Tick;
                this.typedOwner.holdTimer.Stop();

                if (this.ItemState == ItemState.Realized)
                {
                    this.typedOwner.HideCheckBoxesPressIndicator();
                }

                this.typedOwner.OnItemMouseHold(this);
            }
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);

            this.typedOwner.holdTimer.Stop();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            this.typedOwner.holdTimer.Stop();
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.Control.ManipulationStarted"/> event occurs. This member overrides <see cref="M:System.Windows.UIElement.OnManipulationStarted(System.Object,System.Windows.Input.ManipulationStartedEventArgs)"/>.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);

            if (this.ItemState != ItemState.Realized)
            {
                return;
            }

            //this.manipulationStartScrollState = this.Owner == null ? ScrollState.NotScrolling : this.Owner.ScrollState;

            //if (this.manipulationStartScrollState == ScrollState.NotScrolling)
            //{
            this.OnItemManipulationStarted(e);
            //}

            this.UpdateCheckBoxVisualState("Pressed");
            this.manipulationStartedHandled = e.Handled;
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.Control.ManipulationDelta"/> event occurs. This member overrides <see cref="M:System.Windows.UIElement.OnManipulationDelta(System.Object,System.Windows.Input.ManipulationDeltaEventArgs)"/>.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);

            if (this.ItemState != ItemState.Realized)
            {
                return;
            }

            this.UpdateCheckBoxVisualState("Normal");
            this.typedOwner.HideCheckBoxesPressIndicator();
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.Tap" /> event
        /// occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (this.ItemState != ItemState.Realized)
            {
                return;
            }

            if (e.Handled || this.manipulationStartedHandled)
            {
                this.manipulationStartedHandled = false;
                return;
            }

            //if (this.manipulationStartScrollState == ScrollState.NotScrolling)
            //{
            this.OnTap(e.OriginalSource as UIElement, e.OriginalSource as UIElement, e.GetPosition(e.OriginalSource as UIElement));
            //}

            this.UpdateCheckBoxVisualState("Normal");
        }

        /// <summary>
        /// Performs the core Tap implementation. Currently the owner is asked to handle the action.
        /// </summary>
        protected virtual void OnTap(UIElement container, UIElement originalSource, Point hitPoint)
        {
            RadDataBoundListBox owner = this.Owner as RadDataBoundListBox;
            if (owner != null)
            {
                owner.OnItemTap(this, container, originalSource, hitPoint);
            }
        }
    }
}
