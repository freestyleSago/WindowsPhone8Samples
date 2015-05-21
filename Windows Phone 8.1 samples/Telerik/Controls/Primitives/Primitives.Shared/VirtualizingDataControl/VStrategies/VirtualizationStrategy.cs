using System;
using Telerik.Core;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a base class for all virtualization strategies supported by
    /// <see cref="RadVirtualizingDataControl"/>.
    /// </summary>
    internal abstract class VirtualizationStrategy
    {
        internal const double Epsilon = 0.001;
        internal const double MaxScrollableLength = 0;

        internal VirtualizationStrategyDefinition strategyDefinition;
        internal Orientation orientationCache = Orientation.Vertical;
        internal Size lastViewportSize;
        internal double scrollableItemsLength = 0;
        internal double averageItemLength = 0;
        internal double realizedItemsLength;

        internal double scrollableContentHeightAdjustment = 1600;

        internal double topVirtualizationThreshold;
        internal double bottomVirtualizationThreshold;

        internal RadVirtualizingDataControl owner;

        /// <summary>
        /// Gets an instance of the <see cref="Orientation"/> enum
        /// that represents the stack.
        /// </summary>
        internal Orientation Orientation
        {
            get
            {
                return this.orientationCache;
            }
            set
            {
                if (value != this.orientationCache)
                {
                    this.orientationCache = value;
                    this.OnOrientationChanged(value);
                }
            }
        }

        /// <summary>
        /// This property returns a value from the Orientation enumeration
        /// that represents the direction in which containers are virtualized.
        /// In the case of the Wrap Strategy Horizontal orientaiton means Vertical layout orientation.
        /// </summary>
        internal virtual Orientation LayoutOrientation
        {
            get
            {
                return this.orientationCache;
            }
        }

        internal virtual double ScrollOffset
        {
            get
            {
                if (this.LayoutOrientation == Orientation.Horizontal)
                {
                    return this.owner.manipulationContainer.HorizontalOffset;
                }

                return this.owner.manipulationContainer.VerticalOffset;
            }
        }

        internal virtual double ScrollableLength
        {
            get
            {
                if (this.LayoutOrientation == Orientation.Horizontal)
                {
                    return this.owner.manipulationContainer.ScrollableWidth;
                }

                return this.owner.manipulationContainer.ScrollableHeight;
            }
        }

        internal virtual double ScrollableContentLength
        {
            get
            {
                if (this.LayoutOrientation == Orientation.Horizontal)
                {
                    return this.owner.scrollableContent.Width;
                }

                return this.owner.scrollableContent.Height;
            }
        }

        internal virtual double ViewportExtent
        {
            get
            {
                if (this.LayoutOrientation == Orientation.Horizontal)
                {
                    return this.owner.availableHeight;
                }

                return this.owner.availableWidth;
            }
        }

        internal virtual double ViewportLength
        {
            get
            {
                if (this.LayoutOrientation == Orientation.Horizontal)
                {
                    return this.owner.availableWidth;
                }

                return this.owner.availableHeight;
            }
        }

        internal virtual Size Measure(Size availableSize)
        {
            this.owner.availableWidth = availableSize.Width;
            this.owner.availableHeight = availableSize.Height;

            if (double.IsInfinity(this.owner.availableWidth) || double.IsNaN(this.owner.availableWidth))
            {
                if (this.LayoutOrientation == Orientation.Vertical)
                {
                    this.owner.availableWidth = this.owner.MinWidth;
                }
                else
                {
                    if (!this.owner.IsAsyncBalanceEnabled && this.owner.GetItemCount() > 0)
                    {
                        this.owner.availableWidth = double.MaxValue;
                        this.owner.ManageViewport();
                        this.owner.availableWidth = this.realizedItemsLength;
                    }
                    else
                    {
                        this.owner.availableWidth = this.owner.MinWidth;
                    }
                }
            }
            else
            {
                ////We need to check for AsyncBalance and for ItemCount since the ManageViewport
                ////method does not check whether the source has items. On the other hand, if the AsyncBalance is
                ////enabled, the ManageViewport method will not calculate the length of all items since they are realized on a dispatcher.
                if (!this.owner.IsAsyncBalanceEnabled && this.owner.GetItemCount() > 0)
                {
                    if (this.LayoutOrientation == Orientation.Horizontal &&
                        this.owner.HorizontalAlignment != HorizontalAlignment.Stretch)
                    {
                        if (double.IsInfinity(this.owner.availableHeight))
                        {
                            this.owner.availableHeight = this.owner.MinHeight;
                        }
                        this.owner.ManageViewport();

                        if (this.realizedItemsLength > availableSize.Width)
                        {
                            this.owner.availableWidth = availableSize.Width;
                        }
                        else
                        {
                            this.owner.availableWidth = this.realizedItemsLength;
                        }
                    }
                }
            }

            if (double.IsInfinity(this.owner.availableHeight) || double.IsNaN(this.owner.availableHeight))
            {
                if (this.LayoutOrientation == Orientation.Horizontal)
                {
                    this.owner.availableHeight = this.owner.MinHeight;
                }
                else
                {
                    if (!this.owner.IsAsyncBalanceEnabled && this.owner.GetItemCount() > 0)
                    {
                        this.owner.availableHeight = double.MaxValue;
                        this.owner.ManageViewport();
                        this.owner.availableHeight = this.realizedItemsLength;
                    }
                    else
                    {
                        this.owner.availableHeight = this.owner.MinHeight;
                    }
                }
            }
            else
            {
                ////We need to check for AsyncBalance and for ItemCount since the ManageViewport
                ////method does not check whether the source has items. On the other hand, if the AsyncBalance is
                ////enabled, the ManageViewport method will not calculate the length of all items since they are realized on a dispatcher.
                if (!this.owner.IsAsyncBalanceEnabled && this.owner.GetItemCount() > 0)
                {
                    if (this.LayoutOrientation == Orientation.Vertical &&
                        this.owner.VerticalAlignment != VerticalAlignment.Stretch)
                    {
                        if (double.IsInfinity(this.owner.availableWidth))
                        {
                            this.owner.availableWidth = this.owner.MinWidth;
                        }
                        this.owner.ManageViewport();
                        if (this.realizedItemsLength >= availableSize.Height)
                        {
                            this.owner.availableHeight = availableSize.Height;
                        }
                        else
                        {
                            this.owner.availableHeight = this.realizedItemsLength;
                        }
                    }
                }
            }

            this.owner.scrollableContent.Height = this.owner.availableHeight;
            this.owner.scrollableContent.Width = this.owner.availableWidth;

            return new Size(this.owner.availableWidth, this.owner.availableHeight);
        }

        internal virtual void ResetOwner()
        {
            this.owner = null;
        }

        internal virtual void InitializeOwner(RadVirtualizingDataControl owner)
        {
            if (this.owner != null)
            {
                throw new InvalidOperationException("Owner can be set only once.");
            }

            this.owner = owner;

            this.ResetRealizedItemsBuffers();

            if (this.owner.IsProperlyTemplated)
            {
                this.SynchOwnerScrollViewerForOrientation();
            }
        }

        internal virtual void InvalidateItemOffset(RadVirtualizingDataControlItem item)
        {
            if (this.LayoutOrientation == Orientation.Vertical)
            {
                item.InvalidateVerticalOffset();
            }
            else
            {
                item.InvalidateHorizontalOffset();
            }
        }

        internal virtual void SetItemOffset(RadVirtualizingDataControlItem item, double offset)
        {
            if (this.LayoutOrientation == Orientation.Vertical)
            {
                item.SetVerticalOffset(offset);
            }
            else
            {
                item.SetHorizontalOffset(offset);
            }
        }

        internal virtual double GetElementCanvasOffset(UIElement element)
        {
            if (this.LayoutOrientation == Orientation.Vertical)
            {
                return Canvas.GetTop(element);
            }

            return Canvas.GetLeft(element);
        }

        internal virtual void SetElementCanvasOffset(UIElement element, double offset)
        {
            if (this.LayoutOrientation == Orientation.Vertical)
            {
                Canvas.SetTop(element, offset);
            }
            else
            {
                Canvas.SetLeft(element, offset);
            }
        }

        internal virtual double GetItemExtent(RadVirtualizingDataControlItem item)
        {
            if (this.LayoutOrientation == Orientation.Horizontal)
            {
                return item.height;
            }

            return item.width;
        }

        internal virtual double GetItemLength(RadVirtualizingDataControlItem item)
        {
            if (this.LayoutOrientation == Orientation.Horizontal)
            {
                return item.width;
            }

            return item.height;
        }

        internal virtual void PlaySingleItemAddedAnimation(RadVirtualizingDataControlItem item)
        {
            item.scheduledForBatchAnimation = false;
            int realizedIndex = item.associatedDataItem.Index - this.owner.firstItemCache.associatedDataItem.Index;
            RadAnimationManager.Play(item, this.owner.itemAddedAnimationCache);
            this.owner.scheduledAddAnimations.Add(new SingleItemAnimationContext() { AssociatedItem = item, RealizedIndex = realizedIndex, RealizedLength = this.GetItemLength(item) });
        }

        internal virtual void PlaySingleItemRemovedAnimation(RadVirtualizingDataControlItem item)
        {
            int realizedIndex = item.associatedDataItem.Index - this.owner.firstItemCache.associatedDataItem.Index;
            RadAnimationManager.Play(item, this.owner.itemRemovedAnimationCache);
            this.owner.scheduledRemoveAnimations.Add(new SingleItemAnimationContext() { AssociatedItem = item, RealizedLength = this.GetItemLength(item), RealizedIndex = realizedIndex });
        }

        internal void RecycleItem(RadVirtualizingDataControlItem item)
        {
            this.RecycleItem(item, true);
        }

        internal virtual void RecycleItem(RadVirtualizingDataControlItem item, bool setVisibility)
        {
            if (item.scheduledForBatchAnimation)
            {
                if (this.owner.itemAddedAnimationCache != null)
                {
                    this.owner.itemAddedAnimationCache.ClearAnimation(item);
                }
            }
            item.scheduledForBatchAnimation = false;
            IDataSourceItem associatedItem = item.associatedDataItem;

            this.owner.OnContainerStateChanged(item, associatedItem, ItemState.Recycling);

            item.ResetDataItemBinding();
            if (setVisibility)
            {
                item.Visibility = Visibility.Collapsed;
            }
            this.owner.realizedItems.Remove(item);
            this.owner.recycledItems.Enqueue(item);

            if (item.next != null)
            {
                item.next.previous = item.previous;
            }

            if (item.previous != null)
            {
                item.previous.next = item.next;
            }

            item.next = null;
            item.previous = null;

            int realizedItemsCount = this.owner.realizedItems.Count;

            if (this.owner.realizedItems.Count > 0)
            {
                this.owner.lastItemCache = this.owner.realizedItems[realizedItemsCount - 1];
                this.owner.firstItemCache = this.owner.realizedItems[0];
            }
            else
            {
                this.averageItemLength = 0;
                this.owner.lastItemCache = null;
                this.owner.firstItemCache = null;
            }

            this.owner.OnContainerStateChanged(item, associatedItem, ItemState.Recycled);
        }

        internal virtual void CheckBottomScrollableBounds()
        {
            double lastItemBottom = this.GetRealizedItemsBottom();
            double scrollableContentBottom = this.GetScrollableContentEnd();

            bool isLastItemRealized = this.owner.GetItemAfter(this.owner.lastItemCache.associatedDataItem) == null;

            if (!RadMath.AreClose(lastItemBottom, scrollableContentBottom) || !isLastItemRealized)
            {
                if (isLastItemRealized)
                {
                    double offset = lastItemBottom - scrollableContentBottom;
                    this.CorrectScrollableContentSize(offset);
                    this.owner.OnBottomEdgeCorrected();
                }
                else if (scrollableContentBottom < this.scrollableContentHeightAdjustment + this.ViewportLength)
                {
                    //     this.CorrectScrollableContentSize(this.scrollableItemsLength - this.ScrollableContentLength);
                    double amount = this.owner.GetDataItemCount() - Math.Max(this.owner.GetLastItemCacheIndex(), 0);
                    this.CorrectScrollableContentSize(amount * this.averageItemLength);
                }
                else
                {
                    this.CheckResizeScrollableContentWhenAllItemsRealized();
                }
            }

            //this.owner.previousScrollOffset = scrollOffset;
        }

        //internal virtual void CheckBottomScrollableBounds()
        //{
        //    double lastItemBottom = this.GetRealizedItemsBottom();
        //    double scrollableContentBottom = this.GetScrollableContentEnd();

        //    bool isLastItemRealized = this.owner.GetItemAfter(this.owner.lastItemCache.associatedDataItem) == null;

        //    var scrollOffset = this.LayoutOrientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ?
        //        this.owner.manipulationContainer.HorizontalOffset :
        //        this.owner.manipulationContainer.VerticalOffset;

        //    var scrollChange = this.owner.previousScrollOffset - scrollOffset;

        //    if (!RadMath.AreClose(lastItemBottom, scrollableContentBottom, Math.Abs(scrollChange) + Epsilon) || !isLastItemRealized)
        //    {
        //        if (isLastItemRealized)
        //        {
        //            double offset = lastItemBottom - scrollableContentBottom - scrollChange;
        //            this.CorrectScrollableContentSize(offset);
        //            this.owner.OnBottomEdgeCorrected();
        //        }
        //        else if (scrollableContentBottom < this.scrollableContentHeightAdjustment + this.ViewportLength)
        //        {
        //            //     this.CorrectScrollableContentSize(this.scrollableItemsLength - this.ScrollableContentLength);
        //            double amount = this.owner.GetDataItemCount() - Math.Max(this.owner.GetLastItemCacheIndex(), 0);
        //            this.CorrectScrollableContentSize(amount * this.averageItemLength);
        //        }
        //        else
        //        {
        //            this.CheckResizeScrollableContentWhenAllItemsRealized();
        //        }
        //    }

        //    //this.owner.previousScrollOffset = scrollOffset;
        //}

        internal virtual void CorrectScrollableContentSize(double value)
        {
            switch (this.LayoutOrientation)
            {
                case Orientation.Horizontal:
                    this.owner.scrollableContent.Width += value;
                    break;
                case Orientation.Vertical:
                    this.owner.scrollableContent.Height += value;
                    break;
            }
            this.owner.manipulationContainer.UpdateLayout();
        }

        internal virtual double GetScrollableContentEnd()
        {
            switch (this.LayoutOrientation)
            {
                case Orientation.Vertical:
                    return -this.owner.manipulationContainer.VerticalOffset + this.owner.scrollableContent.Height;
                case Orientation.Horizontal:
                    return -this.owner.manipulationContainer.HorizontalOffset + this.owner.scrollableContent.Width;
            }

            return 0;
        }

        internal virtual void CheckResizeScrollableContentWhenAllItemsRealized()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            bool lastRealized = this.owner.GetItemAfter(this.owner.lastItemCache.associatedDataItem) == null;
            bool firstRealized = this.owner.GetItemBefore(this.owner.firstItemCache.associatedDataItem) == null;

            if (lastRealized && firstRealized)
            {
                this.CorrectScrollableContentSize(-this.ScrollableContentLength + this.realizedItemsLength);
                this.owner.manipulationContainer.UpdateLayout();
            }
        }

        internal virtual void CheckTopScrollableBounds()
        {
            double firstItemTop = this.GetRealizedItemsTop();
            double scrollableContentTop = -this.ScrollOffset;

            bool isFirstItemRealized = this.owner.GetItemBefore(this.owner.firstItemCache.associatedDataItem) == null;

            if (firstItemTop != scrollableContentTop)
            {
                if (isFirstItemRealized && firstItemTop > scrollableContentTop)
                {
                    double currentScrollOffset = -scrollableContentTop;
                    double offset = firstItemTop - scrollableContentTop;
                    double newOffset = currentScrollOffset - offset;
                    if (!RadMath.AreClose(newOffset, this.ScrollOffset, Epsilon))
                    {
                        this.ScrollToOffset(newOffset, null);
                    }
                }
                else if (isFirstItemRealized && firstItemTop < scrollableContentTop)
                {
                    double amount = this.GetElementCanvasOffset(this.owner.itemsPanel) + (scrollableContentTop - firstItemTop);
                    this.SetElementCanvasOffset(this.owner.itemsPanel, amount);
                    double newScrollOffset = this.ScrollOffset + (scrollableContentTop - firstItemTop);
                    double heightCorrection = Math.Max(0, newScrollOffset - this.ScrollableLength);
                    if (heightCorrection > 0)
                    {
                        this.CorrectScrollableContentSize(heightCorrection);
                    }
                    //     this.ScrollToOffset(Math.Min(newScrollOffset, this.ScrollableLength), null);
                }
                else if (scrollableContentTop > -this.scrollableContentHeightAdjustment)
                {
                    double currentScrollOffset = -scrollableContentTop;
                    double adjustment = Math.Max(this.owner.GetFirstItemCacheIndex(), 1) * this.averageItemLength;
                    double newOffset = currentScrollOffset + adjustment;
                    double heightCorrection = Math.Max(0, newOffset - this.ScrollableLength);
                    if (heightCorrection > 0)
                    {
                        this.CorrectScrollableContentSize(heightCorrection);
                    }
                    newOffset = Math.Min(newOffset, this.ScrollableLength);
                    //this.ScrollToOffset(newOffset, () =>
                    //{
                    //    double correction = newOffset - this.ScrollOffset;
                    //    double amount = this.GetElementCanvasOffset(this.owner.itemsPanel) + adjustment - correction;
                    //    this.SetElementCanvasOffset(this.owner.itemsPanel, amount);
                    //});
                }
                else
                {
                    this.CheckResizeScrollableContentWhenAllItemsRealized();
                }
            }
        }

        internal RadVirtualizingDataControlItem GetContainerForItem(IDataSourceItem item, bool insertLast)
        {
            int insertAtIndex = insertLast ? this.owner.realizedItems.Count : 0;
            return this.GetContainerForItem(item, insertAtIndex);
        }

        internal virtual RadVirtualizingDataControlItem GetContainerForItem(IDataSourceItem item, int insertAt)
        {
            RadVirtualizingDataControlItem cp = null;
            if (this.owner.recycledItems.Count == 0)
            {
                cp = this.owner.GenerateContainerForItem(item);
            }
            else
            {
                cp = this.owner.recycledItems.Dequeue();

                if ((this.owner.itemAddedAnimationCache != null && RadAnimationManager.IsAnimationScheduled(cp, this.owner.itemAddedAnimationCache)) ||
                    (this.owner.itemRemovedAnimationCache != null && RadAnimationManager.IsAnimationScheduled(cp, this.owner.itemRemovedAnimationCache)))
                {
                    this.owner.recycledItems.Enqueue(cp);
                    cp = this.owner.GenerateContainerForItem(item);
                }
                else
                {
                    this.owner.OnContainerStateChanged(cp, item, ItemState.Realizing);
                    this.owner.PrepareContainerForItemInternal(cp, item);
                    cp.Visibility = Visibility.Visible;
                }
            }

            this.MeasureContainer(cp);

            this.InsertIntoViewport(cp, insertAt);

            this.owner.firstItemCache = this.owner.realizedItems[0];
            this.owner.lastItemCache = this.owner.realizedItems[this.owner.realizedItems.Count - 1];

            return cp;
        }

        internal virtual void InsertIntoViewport(RadVirtualizingDataControlItem cp, int insertAt)
        {
            this.owner.realizedItems.Insert(insertAt, cp);

            if (insertAt > 0)
            {
                RadVirtualizingDataControlItem previousItem = this.owner.realizedItems[insertAt - 1];
                cp.previous = previousItem;
                previousItem.next = cp;
            }

            if (insertAt < this.owner.realizedItems.Count - 1)
            {
                RadVirtualizingDataControlItem nextItem = this.owner.realizedItems[insertAt + 1];
                cp.next = nextItem;
                nextItem.previous = cp;
            }
        }

        internal virtual void OnAfterItemAddedAnimationEnded(SingleItemAnimationContext context)
        {
        }

        internal virtual void OnAfterItemRemovedAnimationEnded(SingleItemAnimationContext context)
        {
        }

        internal virtual void OnViewportSizeChanged(Size newSize, Size oldSize)
        {
            if (this.lastViewportSize == newSize)
            {
                return;
            }

            this.lastViewportSize = newSize;

            this.ResetRealizedItemsBuffers();
        }

        internal virtual void ResetRealizedItemsBuffers()
        {
            this.topVirtualizationThreshold = -(this.owner.realizedItemsBufferScale * this.ViewportLength);
            this.bottomVirtualizationThreshold = -this.topVirtualizationThreshold;
            this.scrollableContentHeightAdjustment = Math.Min(Math.Abs(this.bottomVirtualizationThreshold), 1600);
        }

        internal abstract void ReorderViewportItemsOnItemRemoved(int removedAt, RadVirtualizingDataControlItem removedContainer);

        internal abstract void ReorderViewportItemsOnItemAdded(int physicalChangeLocation, RadVirtualizingDataControlItem addedItem);

        internal virtual void ReorderViewportItemsOnItemRemovedFromTop(IDataSourceItem removedItem)
        {
        }

        internal virtual void ReorderViewportItemsOnItemAddedOnTop(IDataSourceItem addedItem)
        {
        }

        internal abstract void ReorderViewportItemsOnItemReplaced(RadVirtualizingDataControlItem replacedItem);

        internal abstract void MeasureContainer(RadVirtualizingDataControlItem container);

        internal virtual void RefreshViewportOnItemAdded(IDataSourceItem addedItem)
        {
            int firstRealizedIndex = this.owner.GetFirstItemCacheIndex();
            int lastRealizedIndex = this.owner.GetLastItemCacheIndex();

            int startIndex = addedItem.Index;
            int? listStartIndex = null;
            ////Check whether the change has occured among the currently realized items.
            if (startIndex + 1 >= firstRealizedIndex && startIndex <= lastRealizedIndex)
            {
                listStartIndex = startIndex;

                if (startIndex + 1 == firstRealizedIndex)
                {
                    listStartIndex = startIndex + 1;
                }
            }
            ////If the change has occured among the realized items, update the screen
            if (listStartIndex.HasValue)
            {
                int physicalChangeLocation = this.owner.GetItemRealizedIndexFromListSourceIndex(listStartIndex.Value, firstRealizedIndex);

                ////If the change has occured somewhere between the first or last realized items...
                if (physicalChangeLocation < this.owner.realizedItems.Count)
                {
                    ////...add the new data items in the viewport where they belong and...
                    RadVirtualizingDataControlItem newItem = this.GetContainerForItem(addedItem, physicalChangeLocation);

                    this.ReorderViewportItemsOnItemAdded(physicalChangeLocation, newItem);

                    if (this.owner.itemAddedAnimationCache != null && (this.owner.itemAnimationModeCache & ItemAnimationMode.PlayOnAdd) != 0 && this.owner.CanPlayAnimationForItem(newItem, true))
                    {
                        if (!this.owner.itemAddedBatchAnimationScheduled)
                        {
                            this.owner.itemAddedAnimationCache.ApplyInitialValues(newItem);
                            this.owner.PlaySingleItemAddedAnimation(newItem);
                        }
                        else
                        {
                            this.owner.itemAddedAnimationCache.ApplyInitialValues(newItem);
                            newItem.scheduledForBatchAnimation = true;
                        }
                    }
                }
            }
            else if (startIndex < firstRealizedIndex)
            {
                this.ReorderViewportItemsOnItemAddedOnTop(addedItem);
            }
            else if (startIndex > lastRealizedIndex)
            {
                if (!this.CanRealizeBottom(this.GetRealizedItemsBottom()))
                {
                    return;
                }

                this.ManageLowerViewport(false);

                // If the index of the added item is at the end of the viewport, we simply need to perform bottom balance and start the animation for the last realized item.
                RadVirtualizingDataControlItem lastContainer = this.owner.LastRealizedDataItem;
                if (this.owner.itemAddedAnimationCache != null && (this.owner.itemAnimationModeCache & ItemAnimationMode.PlayOnAdd) != 0 && this.owner.CanPlayAnimationForItem(lastContainer, true))
                {
                    // We need to make this check to make sure the item we will be animating has just been realized. Since
                    // there are cases in which, when in wrap mode, the CanRealizeBottom will return true but the ManageLowerViewport will recycle the last realized which
                    // will trigger the code below, we need to check whether an item has been realized, which means that the last realized data index should be equal to the startIndex.
                    if (lastContainer.associatedDataItem.Index == startIndex)
                    {
                        if (!this.owner.itemAddedBatchAnimationScheduled)
                        {
                            this.owner.itemAddedAnimationCache.ApplyInitialValues(lastContainer);
                            this.owner.PlaySingleItemAddedAnimation(lastContainer);
                        }
                        else
                        {
                            this.owner.itemAddedAnimationCache.ApplyInitialValues(lastContainer);
                            this.owner.lastItemCache.scheduledForBatchAnimation = true;
                        }
                    }
                }
            }

            ////At the end we balance the viewport to recycle items that are out of the buffer
            if (this.owner.scheduledRemoveAnimations.Count == 0)
            {
                this.owner.BalanceVisualSpace();
            }
        }

        internal virtual void RefreshViewportOnItemRemoved(int startIndex, IDataSourceItem removedItem)
        {
            int firstRealizedIndex = this.owner.GetFirstItemCacheIndex();
            int lastRealizedIndex = this.owner.GetLastItemCacheIndex();
            int? listStartIndex = null;
            ////Check whether the change has occured among the currently realized items.
            if (startIndex >= firstRealizedIndex && startIndex <= lastRealizedIndex)
            {
                listStartIndex = startIndex;
            }
            ////If the change has occured among the realized items, update the screen
            if (listStartIndex.HasValue)
            {
                int iterationStartIndex = this.owner.GetItemRealizedIndexFromListSourceIndex(listStartIndex.Value, firstRealizedIndex);

                RadVirtualizingDataControlItem itemToRecycle = this.owner.realizedItems[iterationStartIndex];
                if (this.owner.itemRemovedAnimationCache != null && (this.owner.itemAnimationModeCache & ItemAnimationMode.PlayOnRemove) != 0 && this.owner.CanPlayAnimationForItem(itemToRecycle, false))
                {
                    // If we have item remove animation defined, we recycle the item without hiding it and start the remove animation.
                    this.owner.itemRemovedAnimationCache.ApplyInitialValues(itemToRecycle);
                    this.owner.PlaySingleItemRemovedAnimation(itemToRecycle);
                    this.RecycleItem(itemToRecycle, false);
                }
                else
                {
                    ////Recycle the visual item associated with the removed data item
                    this.owner.ClearContainerForItemInternal(itemToRecycle, itemToRecycle.associatedDataItem);
                    this.ReorderViewportItemsOnItemRemoved(iterationStartIndex, itemToRecycle);
                }
            }
            else if (startIndex < firstRealizedIndex)
            {
                this.ReorderViewportItemsOnItemRemovedFromTop(removedItem);
            }

            ////If there are no more items reset the scroll offset
            if (this.owner.GetItemCount() == 0)
            {
                this.ScrollToOffset(0, null);
                return;
            }

            ////Balance the visual space
            if (this.owner.scheduledRemoveAnimations.Count == 0)
            {
                this.owner.BalanceVisualSpace();
            }
        }

        internal virtual void OnSourceCollectionReset()
        {
        }

        internal virtual void RefreshViewportOnItemReplaced(IDataSourceItem replacedItem)
        {
            int firstRealizedIndex = this.owner.GetFirstItemCacheIndex();
            int lastRealizedIndex = this.owner.GetLastItemCacheIndex();

            int startIndex = replacedItem.Index;
            int? listStartIndex = null;
            ////Check whether the change has occured among the currently realized items.
            if (startIndex >= firstRealizedIndex && startIndex <= lastRealizedIndex)
            {
                listStartIndex = startIndex;
            }

            ////If the change has occured among the realized items, update the screen
            if (listStartIndex.HasValue)
            {
                int iterationStartIndex = this.owner.GetItemRealizedIndexFromListSourceIndex(listStartIndex.Value, firstRealizedIndex);

                if (iterationStartIndex > -1)
                {
                    RadVirtualizingDataControlItem currentItem = this.owner.realizedItems[iterationStartIndex];

                    ////Reevaluate the height of each affected visual item, propagate the new vertical offsets and
                    ////calculate the offset correction to be applied to all other affected visual items.
                    ////This correction is the integral height difference before the update and after that.

                    this.owner.PrepareContainerForItemInternal(currentItem, replacedItem);

                    this.MeasureContainer(currentItem);

                    this.ReorderViewportItemsOnItemReplaced(currentItem);
                }
            }

            if (this.owner.scheduledRemoveAnimations.Count == 0)
            {
                this.owner.BalanceVisualSpace();
            }
        }

        internal void UpdateScrollBarsVisibility()
        {
            switch (this.LayoutOrientation)
            {
                case Orientation.Horizontal:
                    this.owner.manipulationContainer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    this.owner.manipulationContainer.VerticalScrollMode = ScrollMode.Disabled;

                    this.owner.manipulationContainer.HorizontalScrollBarVisibility = this.owner.horizontalScrollBarVisibilityCache;
                    this.owner.manipulationContainer.HorizontalScrollMode = ScrollMode.Enabled;
                    break;
                case Orientation.Vertical:
                    this.owner.manipulationContainer.VerticalScrollBarVisibility = this.owner.verticalScrollBarVisibilityCache;
                    this.owner.manipulationContainer.VerticalScrollMode = ScrollMode.Enabled;

                    this.owner.manipulationContainer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    this.owner.manipulationContainer.HorizontalScrollMode = ScrollMode.Disabled;
                    break;
            }
        }

        internal virtual void ScrollToOffset(double offset, Action scrollCompleted)
        {
            if (!this.owner.IsOperational())
            {
                return;
            }

            if (this.LayoutOrientation == Orientation.Vertical)
            {
                if (this.owner.scrollContentPresenter != null)
                {
                    this.owner.scrollContentPresenter.SetVerticalOffset(offset);
                }
                else
                {
                    this.owner.manipulationContainer.ChangeView(0, offset, 0);
                    //this.owner.manipulationContainer.ScrollToVerticalOffset(offset);
                }
            }
            else
            {
                if (this.owner.scrollContentPresenter != null)
                {
                    this.owner.scrollContentPresenter.SetHorizontalOffset(offset);
                }
                else
                {
                    this.owner.manipulationContainer.ChangeView(offset, 0, 0);
                    //this.owner.manipulationContainer.ScrollToHorizontalOffset(offset);
                }
            }

            if (scrollCompleted != null)
            {
                this.owner.scrollUpdateService.RegisterUpdate(new DelegateUpdate(scrollCompleted));
            }
        }

        internal abstract void ResetRealizationStartWhenUpperUIBufferRecycled(double position);

        internal abstract void ResetRealizationStartWhenLowerUIBufferRecycled(double position);

        internal abstract bool IsItemSizeChangeValid(Size previousSize, Size newSize);

        internal abstract void OnContainerSizeChanged(RadVirtualizingDataControlItem container, Size newSize, Size oldSize);

        internal abstract double GetItemRelativeOffset(RadVirtualizingDataControlItem item);

        internal abstract RadVirtualizingDataControlItem GetTopVisibleContainer();

        internal abstract double GetRealizedItemsBottom();

        internal abstract double GetRealizedItemsTop();

        internal abstract bool CanRecycleTop(double visibleItemsTop);

        internal abstract void RecycleTop(ref double visibleItemsTop);

        internal abstract bool CanRealizeBottom(double visibleItemsBottom);

        internal abstract bool PositionBottomRealizedItem(ref double visibleItemsBottom);

        internal abstract bool CanRecycleBottom(double visibleItemsBottom);

        internal abstract void RecycleBottom(ref double visibleItemsBottom);

        internal abstract bool CanRealizeTop(double visibleItemsTop);

        internal abstract bool PositionTopRealizedItem(ref double visibleItemsTop);

        internal virtual void ManageLowerViewport(bool recycle)
        {
            if (recycle)
            {
                double visibleItemsTop = this.GetRealizedItemsTop();

                while (this.CanRecycleTop(visibleItemsTop))
                {
                    this.RecycleTop(ref visibleItemsTop);
                }

                if (this.owner.firstItemCache != null &&
                    this.owner.firstItemCache == this.owner.lastItemCache &&
                    !(this.owner.IsFirstItemFirstInListSource() || this.owner.IsLastItemLastInListSource()))
                {
                    this.ResetRealizationStartWhenUpperUIBufferRecycled(visibleItemsTop);
                }
            }

            bool processed = false;

            bool isViewportFilled = false;

            double visibleItemsBottom = this.GetRealizedItemsBottom();

            while (this.CanRealizeBottom(visibleItemsBottom))
            {
                IDataSourceItem item = null;

                if (this.owner.realizedItems.Count > 0)
                {
                    item = this.owner.GetItemAfter(this.owner.lastItemCache.associatedDataItem);
                }
                else
                {
                    item = this.owner.GetInitialVirtualizationItem();

                    if (item == null)
                    {
                        item = this.owner.GetFirstItem();
                    }
                }

                isViewportFilled = this.IsViewportFilled(visibleItemsBottom);

                if (item == null)
                {
                    break;
                }

                bool shouldPerformAsynchBalance = this.owner.asyncBalanceMode == AsyncBalanceMode.Standard ? true : isViewportFilled;
                if (this.owner.useAsyncBalance && true && processed && shouldPerformAsynchBalance)
                {
                    this.owner.ScheduleAsyncBalance();
                    return;
                }

                RadVirtualizingDataControlItem uiItem = this.GetContainerForItem(item, true);

                if (this.owner.itemAddedBatchAnimationScheduled && !isViewportFilled && !this.owner.useAsyncBalance && this.owner.CanPlayAnimationForItem(uiItem, true))
                {
                    uiItem.scheduledForBatchAnimation = true;
                    this.owner.itemAddedAnimationCache.ApplyInitialValues(uiItem);
                }
                else if (this.owner.useAsyncBalance && this.owner.itemAddedBatchAnimationScheduled)
                {
                    if (!isViewportFilled && this.owner.CanPlayAnimationForItem(uiItem, true))
                    {
                        this.owner.itemAddedAnimationCache.ApplyInitialValues(uiItem);
                        this.owner.PlaySingleItemAddedAnimation(uiItem);
                        this.owner.itemAddedAnimationCache.InitialDelay += this.owner.itemAddedAnimationIntervalCache;
                    }
                    else
                    {
                        this.owner.itemAddedAnimationCache.InitialDelay = TimeSpan.Zero;
                        this.owner.itemAddedBatchAnimationScheduled = false;
                    }
                }

                bool itemPositioned = this.PositionBottomRealizedItem(ref visibleItemsBottom);
                if (itemPositioned)
                {
                    this.owner.OnContainerStateChanged(uiItem, item, ItemState.Realized);
                    processed = true;
                }
                else
                {
                    this.owner.RecycleLastItem();
                }
            }
        }

        internal virtual void ManageUpperViewport(bool recycle)
        {
            if (this.owner.firstItemCache == null)
            {
                return;
            }

            if (recycle)
            {
                double visibleItemsBottom = this.GetRealizedItemsBottom();
                while (this.CanRecycleBottom(visibleItemsBottom))
                {
                    this.RecycleBottom(ref visibleItemsBottom);
                }

                if (this.owner.lastItemCache != null &&
                    this.owner.firstItemCache == this.owner.lastItemCache &&
                    !(this.owner.IsFirstItemFirstInListSource() || this.owner.IsLastItemLastInListSource()))
                {
                    this.ResetRealizationStartWhenLowerUIBufferRecycled(visibleItemsBottom);
                }
            }

            bool processed = false;

            double visibleItemsTop = this.GetRealizedItemsTop();

            while (this.CanRealizeTop(visibleItemsTop))
            {
                IDataSourceItem item = this.owner.GetItemBefore(this.owner.firstItemCache.associatedDataItem);
                if (item == null)
                {
                    break;
                }

                if (processed && this.owner.useAsyncBalance)
                {
                    this.owner.ScheduleAsyncBalance();
                    return;
                }

                RadVirtualizingDataControlItem uiItem = this.GetContainerForItem(item, false);
                bool itemPositioned = this.PositionTopRealizedItem(ref visibleItemsTop);
                if (itemPositioned)
                {
                    this.owner.OnContainerStateChanged(uiItem, item, ItemState.Realized);
                    processed = true;
                }
                else
                {
                    this.owner.RecycleFirstItem();
                }
            }
        }

        internal abstract void RecalculateViewportMeasurements();

        internal virtual bool IsViewportFilled(double visibleItemsBottom)
        {
            return RadMath.AreClose(visibleItemsBottom, this.ViewportLength);
        }

        internal virtual void OnOrientationChanged(Orientation newValue)
        {
            this.orientationCache = newValue;

            if (this.owner != null && this.owner.IsTemplateApplied)
            {
                this.SynchOwnerScrollViewerForOrientation();
            }
        }

        internal virtual void OnOwnerApplyTemplate()
        {
            this.SynchOwnerScrollViewerForOrientation();

            this.owner.scrollableContent.Height = MaxScrollableLength;
            this.owner.scrollableContent.Width = MaxScrollableLength;
        }

        internal virtual void SynchOwnerScrollViewerForOrientation()
        {
            this.UpdateScrollBarsVisibility();
            Canvas.SetTop(this.owner.itemsPanel, 0);
            Canvas.SetLeft(this.owner.itemsPanel, 0);
            this.owner.manipulationContainer.ChangeView(0, 0, 0);
        }
    }
}