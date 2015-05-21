using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Represents a visual item that resides within a <see cref="LoopingPanel"/> instance.
    /// </summary>
    public abstract class LoopingListItem : RadContentControl
    {
        /// <summary>
        /// Specifies the <see cref="LoopingListItem.IsSelected"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(LoopingListItem), new PropertyMetadata(false, OnIsSelectedChanged));

        internal TranslateTransform translateTransform;
        internal Point translation;
        internal bool isHidden;

        private bool isEmpty = false;
        private int logicalIndex;
        private LoopingPanel panel;
        private Rect arrangeRect;
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopingListItem"/> class.
        /// </summary>
        protected LoopingListItem()
        {
            this.IsEnabledChanged += this.OnIsEnabledChanged;

            this.logicalIndex = -1;

            this.translateTransform = new TranslateTransform();
            this.RenderTransform = this.translateTransform;
        }

        /// <summary>
        /// Determines whether the visual item is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets the logical index in the owning LoopingPanel, represented by this item. 
        /// </summary>
        public int LogicalIndex
        {
            get
            {
                return this.logicalIndex;
            }
            protected internal set
            {
                this.logicalIndex = value;
            }
        }

        /// <summary>
        /// Gets the Rect used by the owning LoopingPanel to arrange this item.
        /// </summary>
        public Rect ArrangeRect
        {
            get
            {
                return this.arrangeRect;
            }
            internal set
            {
                this.arrangeRect = value;
            }
        }

        /// <summary>
        /// Gets the amount of pixels this item is translated vertically relative to its owning panel.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return this.arrangeRect.Y + this.translation.Y;
            }
        }

        /// <summary>
        /// Gets the amount of pixels this item is translated horizontally relative to its owning panel.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return this.arrangeRect.X + this.translation.X;
            }
        }

        /// <summary>
        /// Determines whether the item is currently expanded (its owning list is in Expanded state).
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                if (this.panel != null && this.panel.Owner != null)
                {
                    return this.panel.Owner.IsExpanded;
                }

                return true;
            }
        }

        internal bool IsOwnerFocused
        {
            get
            {
                return this.panel != null && this.panel.Owner != null && this.panel.Owner.FocusState != FocusState.Unfocused;
            }
        }

        /// <summary>
        /// Gets the <see cref="LoopingPanel"/> instance where this item resides.
        /// </summary>
        internal LoopingPanel Panel
        {
            get
            {
                return this.panel;
            }
        }

        internal bool IsEmpty
        {
            get
            {
                return this.isEmpty;
            }
        }

        internal void SetIsEmpty(bool empty)
        {
            this.isEmpty = empty;
            this.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
        }

        internal virtual void SetIsHidden(bool hidden)
        {
            if (hidden == this.isHidden)
            {
                return;
            }

            this.isHidden = hidden;
            this.Opacity = hidden ? 0 : 1;
        }

        internal virtual void Attach(LoopingPanel owner)
        {
            this.panel = owner;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Applies the desired vertical offset by setting a TranslateTransform.Y value.
        /// </summary>
        internal void SetVerticalOffset(double offset)
        {
            this.translation.Y = offset;
            this.translateTransform.Y = offset;
        }

        /// <summary>
        /// Applies the desired horizontal offset by setting a TranslateTransform.X value.
        /// </summary>
        internal void SetHorizontalOffset(double offset)
        {
            this.translation.X = offset;
            this.translateTransform.X = offset;
        }

        /// <summary>
        /// Builds the current visual state that is valid for the item.
        /// </summary>
        /// <returns></returns>
        protected override string ComposeVisualStateName()
        {
            if (this.isSelected)
            {
                return this.IsOwnerFocused ? "Selected,Focused" : "Selected,NotFocused";
            }

            string expandedState = this.IsEnabled ? "Expanded" : "Disabled";
            if (this.IsExpanded)
            {
                return expandedState + ",NotFocused";
            }

            return "Collapsed";
        }

        /// <summary>
        /// Applies the specified visual state as current.
        /// </summary>
        /// <param name="state">The new visual state.</param>
        /// <param name="animate">True to use transitions, false otherwise.</param>
        protected override void SetVisualState(string state, bool animate)
        {
            if (this.isSelected)
            {
                animate = false;
            }

            base.SetVisualState(state, animate);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoopingListItem item = d as LoopingListItem;
            item.isSelected = (bool)e.NewValue;
            item.UpdateVisualState(false);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateVisualState(false);
        }
    }
}
