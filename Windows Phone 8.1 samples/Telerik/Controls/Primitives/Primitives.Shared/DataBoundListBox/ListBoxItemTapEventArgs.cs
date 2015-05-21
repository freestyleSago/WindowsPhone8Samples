using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Contains information about the <see cref="RadDataBoundListBox.ItemTap"/> event.
    /// </summary>
    public class ListBoxItemTapEventArgs : EventArgs
    {
        public ListBoxItemTapEventArgs(RadDataBoundListBoxItem item, UIElement container, UIElement originalSource, Point hitPoint, object SelectedItem)
        {
            Item = item;
            Container = container;
            OriginalSource = originalSource;
            HitPoint = hitPoint;
            TappedItem = SelectedItem;
        }
        public object TappedItem { get; private set; }

        public RadDataBoundListBoxItem Item { get; private set; }

        public UIElement Container { get; private set; }

        public Point HitPoint { get; private set; }

        public UIElement OriginalSource { get; private set; }
    }
}
