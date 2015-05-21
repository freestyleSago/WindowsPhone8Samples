using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class DynamicGridVirtualizationStrategy : VirtualizationStrategy
    {

        private readonly Dictionary<int, int> slotsRepository;
        private readonly List<RadVirtualizingDataControlItem> topRealized = new List<RadVirtualizingDataControlItem>();
        private int stackCount;

        internal double itemExtent;

        internal DynamicGridVirtualizationStrategy()
        {
            this.slotsRepository = new Dictionary<int, int>();
        }

        internal int StackCount
        {
            get
            {
                return this.stackCount;
            }
            set
            {
                if (this.stackCount != value)
                {
                    if (value < 2)
                    {
                        throw new ArgumentException("Stack count must be at least 2.");
                    }

                    this.stackCount = value;
                }
            }
        }

        internal override Orientation LayoutOrientation
        {
            get
            {
                return this.orientationCache == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
            }
        }

        internal override Size Measure(Size availableSize)
        {
            Size measuredSize = base.Measure(availableSize);

            this.itemExtent = this.ViewportExtent / this.stackCount;

            return measuredSize;
        }

        internal override void OnAfterItemRemovedAnimationEnded(SingleItemAnimationContext context)
        {
            this.ReorderViewportItemsOnItemRemoved(context.RealizedIndex, context.AssociatedItem);

            if (this.owner.IsLoaded)
            {
                this.ManageLowerViewport(false);
                this.CheckBottomScrollableBounds();
            }
        }

        internal override void ReorderViewportItemsOnItemRemoved(int removedAt, RadVirtualizingDataControlItem removedContainer)
        {
            if (removedAt < this.owner.realizedItems.Count)
            {
                IDataSourceItem firstRealized = this.owner.realizedItems[removedAt].associatedDataItem;

                while (firstRealized != null)
                {
                    this.slotsRepository.Remove(firstRealized.GetHashCode());
                    firstRealized = firstRealized.Next;
                }

                this.ReorderViewportOnItemsChanged();
            }
        }

        internal override void ReorderViewportItemsOnItemRemovedFromTop(IDataSourceItem removedItem)
        {
            IDataSourceItem firstRealized = this.owner.listSource.GetItemAt(removedItem.Index);

            while (firstRealized != null)
            {
                this.slotsRepository.Remove(firstRealized.GetHashCode());
                firstRealized = firstRealized.Previous;
            }
        }

        internal override void ReorderViewportItemsOnItemAddedOnTop(IDataSourceItem addedItem)
        {
            IDataSourceItem firstDataItem = addedItem.Previous;

            while (firstDataItem != null)
            {
                this.slotsRepository.Remove(firstDataItem.GetHashCode());
                firstDataItem = firstDataItem.Previous;
            }
        }

        internal override void ReorderViewportItemsOnItemAdded(int physicalChangeLocation, RadVirtualizingDataControlItem addedItem)
        {
            this.owner.OnContainerStateChanged(addedItem, addedItem.associatedDataItem, ItemState.Realized);

            int slot = -1;

            if (this.slotsRepository.TryGetValue(addedItem.next.associatedDataItem.GetHashCode(), out slot))
            {
                this.slotsRepository.Remove(addedItem.next.associatedDataItem.GetHashCode());
                this.slotsRepository.Add(addedItem.associatedDataItem.GetHashCode(), slot);
                RadVirtualizingDataControlItem nextContainer = addedItem.next;
                addedItem.SetHorizontalOffset(nextContainer.horizontalOffsetCache);
                addedItem.SetVerticalOffset(nextContainer.verticalOffsetCache);
            }

            RadVirtualizingDataControlItem pivotContainer = addedItem.next;

            int containersToSkip = 0;

            if (pivotContainer != null)
            {
                containersToSkip = Math.Max(this.stackCount - this.owner.realizedItems.IndexOf(pivotContainer), 0);
            }

            while (pivotContainer != null)
            {
                if (containersToSkip > 0)
                {
                    containersToSkip--;
                    continue;
                }

                this.slotsRepository.Remove(pivotContainer.associatedDataItem.Index);
                pivotContainer = pivotContainer.next;
            }

            this.ReorderViewportOnItemsChanged();
        }

        internal override void ReorderViewportItemsOnItemReplaced(RadVirtualizingDataControlItem replacedItem)
        {
            RadVirtualizingDataControlItem pivotContainer = replacedItem.next;

            int containersToSkip = 0;

            if (pivotContainer != null)
            {
                containersToSkip = Math.Max(this.stackCount - this.owner.realizedItems.IndexOf(pivotContainer), 0);
            }

            while (pivotContainer != null)
            {
                if (containersToSkip > 0)
                {
                    containersToSkip--;
                    continue;
                }
                this.slotsRepository.Remove(pivotContainer.associatedDataItem.Index);
                pivotContainer = pivotContainer.next;
            }

            this.ReorderViewportOnItemsChanged();
        }

        internal override void OnSourceCollectionReset()
        {
            base.OnSourceCollectionReset();

            this.slotsRepository.Clear();
        }

        internal override bool IsViewportFilled(double visibleItemsBottom)
        {
            int realizedItemsCount = this.owner.realizedItems.Count;

            if (realizedItemsCount == 0)
            {
                return false;
            }

            return visibleItemsBottom - this.GetItemLength(this.owner.realizedItems[realizedItemsCount - 1]) > this.ViewportLength;
        }

        internal override void MeasureContainer(RadVirtualizingDataControlItem container)
        {
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    container.Width = this.itemExtent;
                    container.Measure(new Size(this.itemExtent, double.PositiveInfinity));
                    break;
                case Orientation.Vertical:
                    container.Height = this.itemExtent;
                    container.Measure(new Size(double.PositiveInfinity, this.itemExtent));
                    break;
            }
            container.InvalidateCachedSize();
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

            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            bool dirty = false;
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    dirty = newSize.Width != oldSize.Width;
                    break;
                case Orientation.Vertical:
                    dirty = newSize.Height != oldSize.Height;
                    break;
            }

            if (!dirty)
            {
                return;
            }

            IDataSourceItem initialItem = this.owner.realizedItems[0].associatedDataItem;
            while (this.owner.realizedItems.Count > 0)
            {
                this.owner.RecycleFirstItem();
            }
            this.slotsRepository.Clear();
            this.owner.initialVirtualizationItem = initialItem;
            this.owner.BalanceVisualSpace();
        }

        internal override void OnContainerSizeChanged(RadVirtualizingDataControlItem container, Size newSize, Size oldSize)
        {
            if (!this.topRealized.Contains(container))
            {
                RadVirtualizingDataControlItem pivotContainer = container.next;

                int containersToSkip = 0;

                if (pivotContainer != null)
                {
                    containersToSkip = Math.Max(this.stackCount - this.owner.realizedItems.IndexOf(pivotContainer), 0);
                }

                while (pivotContainer != null)
                {
                    if (containersToSkip > 0)
                    {
                        containersToSkip--;
                        continue;
                    }

                    this.slotsRepository.Remove(pivotContainer.associatedDataItem.GetHashCode());
                    pivotContainer = pivotContainer.next;
                }

                this.ReorderViewportOnItemsChanged();
            }
            else
            {
                double newOffset = 0;
                double offsetDelta = 0;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        offsetDelta = newSize.Height - oldSize.Height;
                        newOffset = container.verticalOffsetCache - offsetDelta;
                        container.SetVerticalOffset(newOffset);
                        double horizontalOffset = container.horizontalOffsetCache;
                        while (container.previous != null)
                        {
                            container = container.previous;
                            if (container.horizontalOffsetCache == horizontalOffset)
                            {
                                container.SetVerticalOffset(container.verticalOffsetCache - offsetDelta);
                            }
                        }
                        break;
                    case Orientation.Vertical:
                        offsetDelta = newSize.Width - oldSize.Width;
                        newOffset = container.horizontalOffsetCache - offsetDelta;
                        container.SetHorizontalOffset(newOffset);
                        double verticalOffset = container.verticalOffsetCache;

                        while (container.previous != null)
                        {
                            container = container.previous;
                            if (container.verticalOffsetCache == verticalOffset)
                            {
                                container.SetHorizontalOffset(container.horizontalOffsetCache - offsetDelta);
                            }
                        }
                        break;
                }
            }
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

                while (this.owner.realizedItems.Count > 0)
                {
                    RadVirtualizingDataControlItem lastItem = this.owner.realizedItems[this.owner.realizedItems.Count - 1];
                    lastItem.SetVerticalOffset(0);
                    lastItem.SetHorizontalOffset(0);
                    this.owner.RecycleLastItem();
                }

                this.owner.BeginAsyncBalance();
                this.owner.BalanceVisualSpace();
            }
        }

        internal override double GetItemRelativeOffset(RadVirtualizingDataControlItem item)
        {
            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    return item.verticalOffsetCache + Canvas.GetTop(this.owner.itemsPanel) - this.owner.manipulationContainer.VerticalOffset;
                case Orientation.Vertical:
                    return item.horizontalOffsetCache + Canvas.GetLeft(this.owner.itemsPanel) - this.owner.manipulationContainer.HorizontalOffset;
            }

            return 0;
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
            int countOfTopItems = Math.Min((int)((topThresholdAbs / this.averageItemLength) * this.stackCount), this.owner.realizedItems.Count - 1);
            RadVirtualizingDataControlItem pivotItem = this.owner.realizedItems[countOfTopItems];
            int deltaFactor = Math.Round(this.GetItemRelativeOffset(pivotItem), 1) > 0 ? -1 : 1;
            int realizedItemsCount = this.owner.realizedItems.Count;

            for (int i = countOfTopItems; i > -1 && i < realizedItemsCount; i += deltaFactor)
            {
                pivotItem = this.owner.realizedItems[i];

                if (deltaFactor < 0)
                {
                    if (Math.Round(this.GetItemRelativeOffset(pivotItem), 1) <= 0)
                    {
                        return pivotItem;
                    }
                }
                else
                {
                    if (Math.Round(this.GetItemRelativeOffset(pivotItem), 1) == 0)
                    {
                        return pivotItem;
                    }

                    if (Math.Round(this.GetItemRelativeOffset(pivotItem), 1) > 0)
                    {
                        return pivotItem.previous != null ? pivotItem.previous : pivotItem;
                    }
                }
            }

            return pivotItem;
        }

        internal override double GetRealizedItemsBottom()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            double bottomMost = double.MinValue;

            foreach (RadVirtualizingDataControlItem item in this.owner.realizedItems)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (item.height + item.verticalOffsetCache > bottomMost)
                        {
                            bottomMost = item.height + item.verticalOffsetCache;
                        }
                        break;
                    case Orientation.Vertical:
                        if (item.width + item.horizontalOffsetCache > bottomMost)
                        {
                            bottomMost = item.width + item.horizontalOffsetCache;
                        }
                        break;
                }
            }

            return bottomMost - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel);
        }

        internal override double GetRealizedItemsTop()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return 0;
            }

            double topMost = double.MaxValue;

            foreach (RadVirtualizingDataControlItem item in this.owner.realizedItems)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (item.verticalOffsetCache < topMost)
                        {
                            topMost = item.verticalOffsetCache;
                        }
                        break;
                    case Orientation.Vertical:
                        if (item.horizontalOffsetCache < topMost)
                        {
                            topMost = item.horizontalOffsetCache;
                        }
                        break;
                }
            }

            return topMost - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel);
        }

        internal override bool CanRecycleTop(double visibleItemsTop)
        {
            return this.owner.realizedItems.Count > 1 &&
                   this.GetElementCanvasOffset(this.owner.firstItemCache) + this.GetItemLength(this.owner.firstItemCache) - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel) < this.topVirtualizationThreshold;
        }

        internal override bool CanRecycleBottom(double visibleItemsBottom)
        {
            return this.owner.realizedItems.Count > 1 &&
                   this.GetElementCanvasOffset(this.owner.lastItemCache) - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel) > this.ViewportLength + this.bottomVirtualizationThreshold;
        }

        internal override void RecycleBottom(ref double visibleItemsBottom)
        {
            this.slotsRepository.Remove(this.owner.lastItemCache.associatedDataItem.GetHashCode());

            this.owner.RecycleLastItem();
        }

        internal override void RecycleTop(ref double visibleItemsTop)
        {
            this.owner.RecycleFirstItem();
        }

        internal override void RecycleItem(RadVirtualizingDataControlItem item, bool setVisibility)
        {
            this.topRealized.Remove(item);
            base.RecycleItem(item, setVisibility);
        }

        internal override bool CanRealizeBottom(double visibleItemsBottom)
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return true;
            }

            return this.GetElementCanvasOffset(this.owner.lastItemCache) +
                   this.GetItemLength(this.owner.lastItemCache) -
                   this.ScrollOffset +
                   this.GetElementCanvasOffset(this.owner.itemsPanel) < this.ViewportLength + this.bottomVirtualizationThreshold;
        }

        internal override bool PositionTopRealizedItem(ref double visibleItemsTop)
        {
            this.topRealized.Add(this.owner.firstItemCache);
            this.PositionRealizedItemTop(this.owner.firstItemCache, ref visibleItemsTop);
            return true;
        }

        internal override bool PositionBottomRealizedItem(ref double visibleItemsBottom)
        {
            this.PositionRealizedItemBottom(this.owner.lastItemCache, ref visibleItemsBottom);
            return true;
        }

        internal override bool CanRealizeTop(double visibleItemsTop)
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return true;
            }

            return this.GetElementCanvasOffset(this.owner.firstItemCache) - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel) > this.topVirtualizationThreshold;
        }

        internal override void RecalculateViewportMeasurements()
        {
            if (this.owner.realizedItems.Count == 0)
            {
                return;
            }

            this.realizedItemsLength = this.owner.lastItemCache.currentOffset + this.GetItemLength(this.owner.lastItemCache) - this.owner.firstItemCache.currentOffset;
            this.averageItemLength = (this.realizedItemsLength * this.stackCount) / this.owner.realizedItems.Count;
            this.scrollableItemsLength = this.averageItemLength * (this.owner.GetItemCount() / this.stackCount);
        }

        internal override bool IsItemSizeChangeValid(Size previousSize, Size newSize)
        {
            return previousSize.Width != newSize.Width || previousSize.Height != newSize.Height;
        }

        internal override void ResetRealizationStartWhenLowerUIBufferRecycled(double position)
        {
            double bottomDifference = position - this.GetItemLength(this.owner.lastItemCache);
            if (bottomDifference > this.bottomVirtualizationThreshold)
            {
                int rowCount = (int)Math.Round(bottomDifference / this.averageItemLength, 0);
                int itemCount = rowCount * this.stackCount;
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

                while (this.owner.realizedItems.Count > 0)
                {
                    this.RecycleItem(this.owner.realizedItems[0]);
                }

                this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset - this.GetElementCanvasOffset(this.owner.itemsPanel);
                this.PositionBottomRealizedItem(ref currentTop);
            }
        }

        internal override void ResetRealizationStartWhenUpperUIBufferRecycled(double position)
        {
            double topDifference = position + this.GetItemLength(this.owner.lastItemCache);
            if (topDifference < this.topVirtualizationThreshold)
            {
                int rowCount = (int)Math.Round(Math.Abs(topDifference) / this.averageItemLength, 0);
                int itemCount = rowCount * this.stackCount;
                RadVirtualizingDataControlItem lastRealizedDataItem = this.owner.LastRealizedDataItem;
                int lastEvenIndex = lastRealizedDataItem.associatedDataItem.Index - ((lastRealizedDataItem.associatedDataItem.Index + 1) % this.stackCount) + 1;
                int currentLastItemIndex = this.owner.GetDataItemCount();

                if (lastRealizedDataItem != null)
                {
                    currentLastItemIndex = Math.Min(lastEvenIndex + itemCount, currentLastItemIndex - this.stackCount);
                }

                IDataSourceItem newDataItem = lastRealizedDataItem.associatedDataItem;

                while (newDataItem.Index < currentLastItemIndex)
                {
                    newDataItem = newDataItem.Next;

                    Debug.Assert(newDataItem != null, "The currentLastItemIndex should be within the bounds of the flattened view of the collection.");
                }

                while (this.owner.realizedItems.Count > 0)
                {
                    this.RecycleItem(this.owner.realizedItems[0]);
                }

                this.GetContainerForItem(newDataItem, false);
                double currentTop = this.ScrollOffset - this.GetElementCanvasOffset(this.owner.itemsPanel);
                this.PositionBottomRealizedItem(ref currentTop);
            }
        }

        private void ReorderViewportOnItemsChanged()
        {
            foreach (RadVirtualizingDataControlItem item in this.owner.realizedItems)
            {
                Rect slot = this.FindFreeSpotForItemBottom(item);
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        item.SetHorizontalOffset(slot.Left);
                        item.SetVerticalOffset(slot.Top);
                        this.slotsRepository[item.associatedDataItem.GetHashCode()] = (int)(slot.Left / this.itemExtent);
                        break;
                    case Orientation.Vertical:
                        item.SetVerticalOffset(slot.Top);
                        item.SetHorizontalOffset(slot.Left);
                        this.slotsRepository[item.associatedDataItem.GetHashCode()] = (int)(slot.Top / this.itemExtent);
                        break;
                }
            }
        }

        private Rect FindFreeSpotForItemTop(RadVirtualizingDataControlItem item)
        {
            RadVirtualizingDataControlItem startingItem = item.next;

            Dictionary<double, Rect> slots = new Dictionary<double, Rect>();

            while (startingItem != null)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (!slots.ContainsKey(startingItem.horizontalOffsetCache))
                        {
                            slots.Add(startingItem.horizontalOffsetCache,
                                new Rect(startingItem.horizontalOffsetCache, startingItem.verticalOffsetCache, startingItem.width, startingItem.height));
                        }
                        break;
                    case Orientation.Vertical:
                        if (!slots.ContainsKey(startingItem.verticalOffsetCache))
                        {
                            slots.Add(startingItem.verticalOffsetCache,
                                new Rect(startingItem.horizontalOffsetCache, startingItem.verticalOffsetCache, startingItem.width, startingItem.height));
                        }
                        break;
                }

                if (slots.Count == this.stackCount)
                {
                    break;
                }

                startingItem = startingItem.next;
            }

            Rect? slot = null;
            int slotIndex;
            if (this.slotsRepository.TryGetValue(item.associatedDataItem.GetHashCode(), out slotIndex))
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        Rect itemSlot = slots.Values.Where<Rect>(rect => RadMath.AreClose(rect.Left, slotIndex * this.itemExtent, Epsilon)).FirstOrDefault<Rect>();
                        return new Rect(itemSlot.Left, itemSlot.Top - item.height, item.width, item.height);
                    case Orientation.Vertical:
                        itemSlot = slots.Values.Where<Rect>(rect => RadMath.AreClose(rect.Top, slotIndex * this.itemExtent, Epsilon)).FirstOrDefault<Rect>();
                        return new Rect(itemSlot.Left - item.width, itemSlot.Top, item.width, item.height);
                }
            }

            Rect[] sortedValues = new Rect[this.stackCount];

            Rect[] sorted = this.orientationCache == Orientation.Horizontal ? slots.Values.OrderByDescending<Rect, double>(r => r.Left).ToArray<Rect>() :
                            slots.Values.OrderByDescending<Rect, double>(r => r.Top).ToArray<Rect>();
            sorted.CopyTo(sortedValues, 0);
            if (sorted.Length < this.stackCount)
            {
                int difference = sorted.Length;
                for (int i = difference; i < this.stackCount; i++)
                {
                    switch (this.orientationCache)
                    {
                        case Orientation.Horizontal:
                            sortedValues[i] = new Rect(i * this.itemExtent, this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel), 0, 0);
                            break;
                        case Orientation.Vertical:
                            sortedValues[i] = new Rect(this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel), i * this.itemExtent, 0, 0);
                            break;
                    }
                }
            }

            foreach (Rect freeSlot in sortedValues)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }
                        else
                        {
                            if (freeSlot.Top > slot.Value.Top)
                            {
                                slot = freeSlot;
                            }
                        }
                        break;
                    case Orientation.Vertical:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }
                        else
                        {
                            if (freeSlot.Left > slot.Value.Left)
                            {
                                slot = freeSlot;
                            }
                        }
                        break;
                }
            }

            if (slot == null)
            {
                slot = new Rect();
            }
            else
            {
                Rect valueSlot = slot.Value;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        slot = new Rect(valueSlot.Left, valueSlot.Top - item.height, item.width, item.height);
                        break;
                    case Orientation.Vertical:
                        slot = new Rect(valueSlot.Left - item.width, valueSlot.Top, item.width, item.height);
                        break;
                }
            }

            return slot.Value;
        }

        private Rect FindFreeSpotForItemBottom(RadVirtualizingDataControlItem item)
        {
            RadVirtualizingDataControlItem startingItem = item.previous;

            Dictionary<double, Rect> slots = new Dictionary<double, Rect>();
            while (startingItem != null)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (!slots.ContainsKey(startingItem.horizontalOffsetCache))
                        {
                            slots.Add(startingItem.horizontalOffsetCache,
                                new Rect(startingItem.horizontalOffsetCache, startingItem.verticalOffsetCache, startingItem.width, startingItem.height));
                        }
                        break;
                    case Orientation.Vertical:
                        if (!slots.ContainsKey(startingItem.verticalOffsetCache))
                        {
                            slots.Add(startingItem.verticalOffsetCache,
                                new Rect(startingItem.horizontalOffsetCache, startingItem.verticalOffsetCache, startingItem.width, startingItem.height));
                        }
                        break;
                }

                if (slots.Count == this.stackCount)
                {
                    break;
                }
                startingItem = startingItem.previous;
            }

            int slotIndex;
            if (this.slotsRepository.TryGetValue(item.associatedDataItem.GetHashCode(), out slotIndex))
            {
                return new Rect(item.horizontalOffsetCache, item.verticalOffsetCache, item.width, item.height);
            }

            Rect[] sortedValues = new Rect[this.stackCount];

            Rect[] sorted = this.orientationCache == Orientation.Horizontal ? slots.Values.OrderBy<Rect, double>(r => r.Left).ToArray<Rect>() :
                            slots.Values.OrderBy<Rect, double>(r => r.Top).ToArray<Rect>();
            sorted.CopyTo(sortedValues, 0);
            if (sorted.Length < this.stackCount)
            {
                int difference = sorted.Length;
                for (int i = difference; i < this.stackCount; i++)
                {
                    switch (this.orientationCache)
                    {
                        case Orientation.Horizontal:
                            sortedValues[i] = new Rect(i * this.itemExtent, this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel), 0, 0);
                            break;
                        case Orientation.Vertical:
                            sortedValues[i] = new Rect(this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel), i * this.itemExtent, 0, 0);
                            break;
                    }
                }
            }

            Rect? slot = null;
            foreach (Rect freeSlot in sortedValues)
            {
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }

                        if (freeSlot.Bottom < slot.Value.Bottom)
                        {
                            slot = freeSlot;
                        }
                        break;
                    case Orientation.Vertical:
                        if (slot == null)
                        {
                            slot = freeSlot;
                            continue;
                        }

                        if (freeSlot.Right < slot.Value.Right)
                        {
                            slot = freeSlot;
                        }
                        break;
                }
            }

            if (slot == null)
            {
                slot = new Rect();
            }
            else
            {
                Rect valueSlot = slot.Value;
                switch (this.orientationCache)
                {
                    case Orientation.Horizontal:
                        slot = new Rect(valueSlot.Left, valueSlot.Bottom, item.width, item.height);
                        break;
                    case Orientation.Vertical:
                        slot = new Rect(valueSlot.Right, valueSlot.Top, item.width, item.height);
                        break;
                }
            }

            return slot.Value;
        }

        private void PositionRealizedItemBottom(RadVirtualizingDataControlItem lastRealizedItem, ref double visibleItemsBottom)
        {
            Rect slot = this.FindFreeSpotForItemBottom(lastRealizedItem);

            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    visibleItemsBottom = slot.Bottom - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel);
                    lastRealizedItem.SetHorizontalOffset(slot.Left);
                    lastRealizedItem.SetVerticalOffset(slot.Top);
                    this.slotsRepository[lastRealizedItem.associatedDataItem.GetHashCode()] = (int)(slot.Left / this.itemExtent);
                    break;
                case Orientation.Vertical:
                    visibleItemsBottom = slot.Right - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel);
                    lastRealizedItem.SetVerticalOffset(slot.Top);
                    lastRealizedItem.SetHorizontalOffset(slot.Left);
                    this.slotsRepository[lastRealizedItem.associatedDataItem.GetHashCode()] =  (int)(slot.Top / this.itemExtent);
                    break;
            }
        }

        private void PositionRealizedItemTop(RadVirtualizingDataControlItem firstRealizedItem, ref double visibleItemsTop)
        {
            Rect slot = this.FindFreeSpotForItemTop(firstRealizedItem);

            switch (this.orientationCache)
            {
                case Orientation.Horizontal:
                    firstRealizedItem.SetHorizontalOffset(slot.Left);
                    firstRealizedItem.SetVerticalOffset(slot.Top);
                    visibleItemsTop = slot.Top - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel);
                    break;
                case Orientation.Vertical:
                    firstRealizedItem.SetVerticalOffset(slot.Top);
                    firstRealizedItem.SetHorizontalOffset(slot.Left);
                    visibleItemsTop = slot.Left - this.ScrollOffset + this.GetElementCanvasOffset(this.owner.itemsPanel);
                    break;
            }
        }
    }
}