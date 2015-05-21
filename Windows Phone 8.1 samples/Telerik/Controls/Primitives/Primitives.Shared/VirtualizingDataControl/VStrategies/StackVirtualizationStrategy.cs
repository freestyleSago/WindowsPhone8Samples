using System;
using System.Diagnostics;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Implements a stack layout strategy for the viewport items of <see cref="RadDataBoundListBox"/>.
    /// Supports vertical and horizontal stack orientation.
    /// </summary>
    internal class StackVirtualizationStrategy : VirtualizationStrategy
    {
        internal CollectionChangeItemReorderMode reorderMode = CollectionChangeItemReorderMode.MoveItemsDown;

        internal CollectionChangeItemReorderMode ReorderMode
        {
            get
            {
                return this.reorderMode;
            }
            set
            {
                if (this.reorderMode != value)
                {
                    this.reorderMode = value;
                }
            }
        }

        internal override bool IsItemSizeChangeValid(Size previousSize, Size newSize)
        {
            return this.owner.virtualizationStrategy.orientationCache == Orientation.Vertical ? previousSize.Height != newSize.Height : previousSize.Width != newSize.Width;
        }

        internal override void OnAfterItemRemovedAnimationEnded(SingleItemAnimationContext context)
        {
            double startingOffset = context.AssociatedItem.currentOffset;
            double offset = context.RealizedLength;
            ////Apply the aggregated height correction to all affected visual items reamining on the viewport
            for (int i = 0; i < this.owner.realizedItems.Count; i++)
            {
                RadVirtualizingDataControlItem firstItem = this.owner.realizedItems[i];

                if (firstItem.currentOffset > startingOffset)
                {
                    if (firstItem.previous == null || (firstItem.previous != null && firstItem.previous.currentOffset + this.GetItemLength(firstItem.previous) <= firstItem.currentOffset - offset))
                    {
                        this.SetItemOffset(firstItem, firstItem.currentOffset - offset);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            this.TranslateRemoveAnimatedItemsWithOffset(startingOffset, -offset);

            if (this.owner.IsLoaded)
            {
                this.ManageLowerViewport(false);
                this.CheckBottomScrollableBounds();
            }
        }

        internal override void RecycleItem(RadVirtualizingDataControlItem item, bool setVisibility)
        {
            base.RecycleItem(item, setVisibility);
            this.realizedItemsLength -= this.GetItemLength(item);
        }

        internal override RadVirtualizingDataControlItem GetContainerForItem(IDataSourceItem item, int insertAt)
        {
            RadVirtualizingDataControlItem container = base.GetContainerForItem(item, insertAt);

            this.realizedItemsLength += this.GetItemLength(container);

            return container;
        }

        internal override void OnViewportSizeChanged(Size newSize, Size oldSize)
        {
            if (this.lastViewportSize == newSize)
            {
                base.OnViewportSizeChanged(newSize, oldSize);
                return;
            }

            base.OnViewportSizeChanged(newSize, oldSize);

            // do not balance if layout is still not updated
            if (!this.owner.layoutUpdated)
            {
                return;
            }
            bool needsUpdate = this.orientationCache == Orientation.Vertical ? newSize.Width != oldSize.Width : newSize.Height != oldSize.Height;
            if (!needsUpdate && this.owner.IsOperational())
            {
                this.owner.BalanceVisualSpace();
            }
            else if (this.owner.realizedItems.Count > 0)
            {
                this.owner.StopAllAddedAnimations();
                this.owner.StopAllRemovedAnimations();

                // We do a double check here since StopAllRemovedAnimations may recycle all items
                // if a batch remove animation has been scheduled.
                if (this.owner.realizedItems.Count > 0)
                {
                    double currentBottom = this.owner.firstItemCache.currentOffset;
                    foreach (RadVirtualizingDataControlItem realizedItem in this.owner.realizedItems)
                    {
                        this.MeasureContainer(realizedItem);
                        double currentHeight = this.GetItemLength(realizedItem);
                        this.SetItemOffset(realizedItem, currentBottom);
                        currentBottom = realizedItem.currentOffset + currentHeight;

                        object par = new object();
                        realizedItem.PerformSpecialItemAction("SynchCheckBoxState", null, ref par);
                    }
                }
            }
        }

        internal override void MeasureContainer(RadVirtualizingDataControlItem container)
        {
            if (this.orientationCache == Orientation.Vertical)
            {
                container.Width = this.owner.availableWidth;
                container.Measure(new Size(this.owner.availableWidth, double.PositiveInfinity));
            }
            else
            {
                container.Height = this.owner.availableHeight;
                container.Measure(new Size(double.PositiveInfinity, this.owner.availableHeight));
            }
            container.InvalidateCachedSize();
        }

        internal override void ReorderViewportItemsOnItemAdded(int physicalChangeLocation, RadVirtualizingDataControlItem addedItem)
        {
            double startingOffset = addedItem.next != null ? addedItem.next.currentOffset : 0;

            double newItemLength = this.GetItemLength(addedItem);

            if (this.reorderMode == CollectionChangeItemReorderMode.MoveItemsUp)
            {
                startingOffset -= newItemLength;
            }

            this.SetItemOffset(addedItem, startingOffset);

            this.owner.OnContainerStateChanged(addedItem, addedItem.associatedDataItem, ItemState.Realized);

            ////... iterate all affected visual items, i.e. items starting from the index where the change occured,
            ////in order to update their vertical offset accordingly (since they must be offset by the newly added items)
            if (this.reorderMode == CollectionChangeItemReorderMode.MoveItemsDown)
            {
                for (int i = physicalChangeLocation + 1; i < this.owner.realizedItems.Count; i++)
                {
                    RadVirtualizingDataControlItem currentItem = this.owner.realizedItems[i];
                    this.SetItemOffset(currentItem, currentItem.currentOffset + newItemLength);
                }
            }
            else
            {
                for (int i = physicalChangeLocation - 1; i > -1; i--)
                {
                    RadVirtualizingDataControlItem currentItem = this.owner.realizedItems[i];
                    this.SetItemOffset(currentItem, currentItem.currentOffset - newItemLength);
                }
            }

            if (!this.owner.itemRemovedBatchAnimationScheduled)
            {
                foreach (SingleItemAnimationContext removed in this.owner.scheduledRemoveAnimations)
                {
                    if (removed.AssociatedItem.currentOffset > startingOffset)
                    {
                        this.SetItemOffset(removed.AssociatedItem, removed.AssociatedItem.currentOffset + this.GetItemLength(addedItem));
                    }
                }
            }
        }

        internal override void ReorderViewportItemsOnItemRemovedFromTop(IDataSourceItem removedItem)
        {
            if (this.reorderMode == CollectionChangeItemReorderMode.MoveItemsDown)
            {
                this.ScrollToOffset(this.owner.manipulationContainer.VerticalOffset + this.averageItemLength, null);
            }
        }

        internal override void ReorderViewportItemsOnItemAddedOnTop(IDataSourceItem addedItem)
        {
            if (this.reorderMode == CollectionChangeItemReorderMode.MoveItemsDown)
            {
                this.ScrollToOffset(this.owner.manipulationContainer.VerticalOffset - this.averageItemLength, null);
            }
        }

        internal override void ReorderViewportItemsOnItemRemoved(int removedAt, RadVirtualizingDataControlItem removedContainer)
        {
            double correctionLength = this.GetItemLength(removedContainer);

            if (this.reorderMode == CollectionChangeItemReorderMode.MoveItemsDown)
            {
                ////Apply the aggregated height correction to all affected visual items reamining on the viewport
                for (int i = removedAt; i < this.owner.realizedItems.Count; i++)
                {
                    RadVirtualizingDataControlItem itemToOffset = this.owner.realizedItems[i];
                    double currentVerticalOffset = itemToOffset.currentOffset;
                    this.SetItemOffset(itemToOffset, currentVerticalOffset - correctionLength);
                }
            }
            else
            {
                ////Apply the aggregated height correction to all affected visual items reamining on the viewport
                for (int i = 0; i < removedAt; i++)
                {
                    RadVirtualizingDataControlItem itemToOffset = this.owner.realizedItems[i];
                    double currentVerticalOffset = itemToOffset.currentOffset;
                    this.SetItemOffset(itemToOffset, currentVerticalOffset + correctionLength);
                }
            }
        }

        internal override void ReorderViewportItemsOnItemReplaced(RadVirtualizingDataControlItem replacedItem)
        {
            if (this.reorderMode == CollectionChangeItemReorderMode.MoveItemsDown)
            {
                RadVirtualizingDataControlItem nextItem = replacedItem.next;
                while (nextItem != null)
                {
                    this.SetItemOffset(nextItem, nextItem.previous.currentOffset + this.GetItemLength(nextItem.previous));

                    nextItem = nextItem.next;
                }
            }
            else
            {
                RadVirtualizingDataControlItem processedItem = replacedItem;
                while (processedItem != null)
                {
                    if (processedItem.next != null)
                    {
                        this.SetItemOffset(processedItem, processedItem.next.currentOffset - this.GetItemLength(processedItem));
                    }

                    processedItem = processedItem.previous;
                }
            }
        }

        internal override void OnContainerSizeChanged(RadVirtualizingDataControlItem container, Size newSize, Size oldSize)
        {
            System.Diagnostics.Debug.WriteLine("Container size changed");
            double newLength = this.orientationCache == Orientation.Vertical ? newSize.Height : newSize.Width;
            double oldLength = this.orientationCache == Orientation.Vertical ? oldSize.Height : oldSize.Width;

            if (container.next == null)
            {
                this.realizedItemsLength += newLength - oldLength;
                if (this.owner.scheduledRemoveAnimations.Count == 0)
                {
                    this.CheckBottomScrollableBounds();
                }
                return;
            }

            double diff = newLength - oldLength;
            this.realizedItemsLength += diff;

            this.TranslateRemoveAnimatedItemsWithOffset(container.currentOffset, diff);

            this.RecalculateViewportMeasurements();

            while (container.next != null)
            {
                container = container.next;
                double adjustment = container.currentOffset + diff;
                this.SetItemOffset(container, adjustment);
            }

            if (this.owner.scheduledRemoveAnimations.Count == 0)
            {
                this.CheckBottomScrollableBounds();
            }
        }

        internal override double GetItemRelativeOffset(RadVirtualizingDataControlItem item)
        {
            return item.currentOffset + this.GetElementCanvasOffset(this.owner.itemsPanel) - this.ScrollOffset;
        }

        internal override RadVirtualizingDataControlItem GetTopVisibleContainer()
        {
            ////The approach here is to calculate the possible
            ////index of the topmost item by considering the upper buffer size
            ////and the average item height. In the ideal case of having equal height
            ////containers, the index will be calculated exactly. In case of wrong index calculation
            ////we estimate the direction we have to take in order to find the topmost item and
            ////interate to it.
            if (this.owner.realizedItems.Count == 0)
            {
                return null;
            }

            if (this.averageItemLength == 0)
            {
                return this.owner.firstItemCache;
            }

            double topThresholdAbs = Math.Abs(Math.Max(this.GetItemRelativeOffset(this.owner.firstItemCache), this.topVirtualizationThreshold));
            int countOfTopItems = Math.Min((int)(topThresholdAbs / this.averageItemLength), this.owner.realizedItems.Count - 1);
            RadVirtualizingDataControlItem topElement = this.owner.realizedItems[countOfTopItems];
            int deltaFactor = Math.Round(this.GetItemRelativeOffset(topElement), 1) > 0 ? -1 : 1;
            int realizedItemsCount = this.owner.realizedItems.Count;

            for (int i = countOfTopItems; i > -1 && i < realizedItemsCount; i += deltaFactor)
            {
                topElement = this.owner.realizedItems[i];

                if (deltaFactor < 0)
                {
                    if (Math.Round(this.GetItemRelativeOffset(topElement), 1) <= 0)
                    {
                        return topElement;
                    }
                }
                else
                {
                    if (Math.Round(this.GetItemRelativeOffset(topElement), 1) == 0)
                    {
                        return topElement;
                    }

                    if (Math.Round(this.GetItemRelativeOffset(topElement), 1) > 0)
                    {
                        return topElement.previous != null ? topElement.previous : topElement;
                    }
                }
            }

            return topElement;
        }

        internal override void RecalculateViewportMeasurements()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            int releasedItemsCount = this.owner.realizedItems.Count;
            int itemCount = this.owner.GetItemCount();
            this.averageItemLength = this.realizedItemsLength / releasedItemsCount;
            this.scrollableItemsLength = (this.averageItemLength * (itemCount - releasedItemsCount)) + this.realizedItemsLength;
        }

        internal override double GetRealizedItemsBottom()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            return this.GetElementCanvasOffset(this.owner.lastItemCache) + this.GetElementCanvasOffset(this.owner.itemsPanel) + this.GetItemLength(this.owner.lastItemCache) - this.ScrollOffset;
        }

        internal override double GetRealizedItemsTop()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            return this.GetElementCanvasOffset(this.owner.firstItemCache) + this.GetElementCanvasOffset(this.owner.itemsPanel) - this.ScrollOffset;
        }

        internal override bool CanRecycleTop(double visibleItemsTop)
        {
            return this.owner.firstItemCache != this.owner.lastItemCache &&
                    visibleItemsTop + this.GetItemLength(this.owner.firstItemCache) < this.topVirtualizationThreshold;
        }

        internal override void RecycleTop(ref double visibleItemsTop)
        {
            double height = this.GetItemLength(this.owner.firstItemCache);

            this.owner.RecycleFirstItem();

            visibleItemsTop += height;
        }

        internal override bool CanRealizeBottom(double visibleItemsBottom)
        {
            return visibleItemsBottom < this.ViewportLength + this.bottomVirtualizationThreshold;
        }

        internal override void ResetRealizationStartWhenLowerUIBufferRecycled(double position)
        {
            double bottomDifference = position - this.GetItemLength(this.owner.lastItemCache);
            if (bottomDifference > this.bottomVirtualizationThreshold)
            {
                int itemCount = (int)Math.Round(bottomDifference / this.averageItemLength, 0);

                RadVirtualizingDataControlItem firstRealizedDataItem = this.owner.FirstRealizedDataItem;
                int currentFirstItemIndex = 0;
                if (firstRealizedDataItem != null)
                {
                    currentFirstItemIndex = Math.Max(firstRealizedDataItem.associatedDataItem.Index - itemCount, 0);
                }

                IDataSourceItem newDataItem = firstRealizedDataItem.associatedDataItem;

                while (newDataItem.Index > currentFirstItemIndex)
                {
                    newDataItem = newDataItem.Previous;

                    Debug.Assert(newDataItem != null, "The currentLastItemIndex should be within the bounds of the flattened view of the collection.");
                }

                this.owner.ClearContainerForItemInternal(this.owner.lastItemCache, this.owner.lastItemCache.associatedDataItem);
                RadVirtualizingDataControlItem newRealized = this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset;
                this.SetItemOffset(newRealized, currentTop - this.GetElementCanvasOffset(this.owner.itemsPanel));
            }
        }

        internal override void ResetRealizationStartWhenUpperUIBufferRecycled(double position)
        {
            double topDifference = position + this.GetItemLength(this.owner.lastItemCache);
            if (topDifference < this.topVirtualizationThreshold)
            {
                int itemCount = (int)Math.Round(Math.Abs(topDifference) / this.averageItemLength, 0);

                RadVirtualizingDataControlItem lastRealizedDataItem = this.owner.LastRealizedDataItem;
                int currentLastItemIndex = this.owner.GetDataItemCount();

                if (lastRealizedDataItem != null)
                {
                    currentLastItemIndex = Math.Min(lastRealizedDataItem.associatedDataItem.Index + itemCount, currentLastItemIndex - 1);
                }

                IDataSourceItem newDataItem = lastRealizedDataItem.associatedDataItem;

                while (newDataItem.Index < currentLastItemIndex)
                {
                    newDataItem = newDataItem.Next;

                    Debug.Assert(newDataItem != null, "The currentLastItemIndex should be within the bounds of the flattened view of the collection.");
                }

                this.owner.ClearContainerForItemInternal(this.owner.lastItemCache, this.owner.lastItemCache.associatedDataItem);
                RadVirtualizingDataControlItem newRealized = this.GetContainerForItem(newDataItem, false);

                double currentBottom = this.ScrollOffset;
                this.SetItemOffset(newRealized, currentBottom - this.GetElementCanvasOffset(this.owner.itemsPanel));
            }
        }

        internal override bool PositionBottomRealizedItem(ref double visibleItemsBottom)
        {
            RadVirtualizingDataControlItem lastRealizedItem = this.owner.lastItemCache;
            double offset = lastRealizedItem.previous != null ? lastRealizedItem.previous.currentOffset + this.GetItemLength(lastRealizedItem.previous) : 0;
            this.SetItemOffset(lastRealizedItem, offset);
            visibleItemsBottom += this.GetItemLength(lastRealizedItem);

            return true;
        }

        internal override bool CanRecycleBottom(double visibleItemsBottom)
        {
            return this.owner.firstItemCache != this.owner.lastItemCache &&
                    visibleItemsBottom - this.GetItemLength(this.owner.lastItemCache) > this.ViewportLength + this.bottomVirtualizationThreshold;
        }

        internal override void RecycleBottom(ref double visibleItemsBottom)
        {
            double length = this.GetItemLength(this.owner.lastItemCache);
            this.owner.RecycleLastItem();
            visibleItemsBottom -= length;
        }

        internal override bool CanRealizeTop(double visibleItemsTop)
        {
            return visibleItemsTop > this.topVirtualizationThreshold;
        }

        internal override bool PositionTopRealizedItem(ref double visibleItemsTop)
        {
            RadVirtualizingDataControlItem firstRealizedItem = this.owner.firstItemCache;
            double verticalOffset = firstRealizedItem.next != null ? firstRealizedItem.next.currentOffset - this.GetItemLength(firstRealizedItem) : 0;
            this.SetItemOffset(firstRealizedItem, verticalOffset);
            visibleItemsTop -= this.GetItemLength(firstRealizedItem);

            return true;
        }

        internal override void OnOrientationChanged(Orientation newValue)
        {
            base.OnOrientationChanged(newValue);

            if (this.owner == null || !this.owner.IsTemplateApplied)
            {
                return;
            }

            if (this.owner.GetItemCount() > 0)
            {
                this.owner.StopAllAddedAnimations();
                this.owner.StopAllRemovedAnimations();

                IDataSourceItem firstViewportItem = this.GetTopVisibleContainer().associatedDataItem;
                while (this.owner.realizedItems.Count > 0)
                {
                    RadDataBoundListBoxItem lastItem = this.owner.realizedItems[this.owner.realizedItems.Count - 1] as RadDataBoundListBoxItem;
                    lastItem.SetVerticalOffset(0);
                    lastItem.SetHorizontalOffset(0);
                    this.owner.RecycleLastItem();
                }

                this.owner.initialVirtualizationItem = firstViewportItem;
                this.owner.BeginAsyncBalance();
                this.owner.BalanceVisualSpace();
            }
        }

        private void TranslateRemoveAnimatedItemsWithOffset(double startingFrom, double offset)
        {
            foreach (SingleItemAnimationContext item in this.owner.scheduledRemoveAnimations)
            {
                RadVirtualizingDataControlItem associatedItem = item.AssociatedItem;
                if (associatedItem.currentOffset > startingFrom)
                {
                    this.SetItemOffset(associatedItem, associatedItem.currentOffset + offset);
                }
            }
        }
    }
}
