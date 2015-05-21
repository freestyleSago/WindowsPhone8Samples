using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class BringIntoViewOperation
    {
        internal static readonly int MaxScrollAttempts = 10;

        public BringIntoViewOperation(object item)
        {
            this.RequestedItem = item;
        }

        public int ScrollAttempts { get; set; }

        public object RequestedItem { get; private set; }

        public Action CompletedAction { get; set; }
    }

}
