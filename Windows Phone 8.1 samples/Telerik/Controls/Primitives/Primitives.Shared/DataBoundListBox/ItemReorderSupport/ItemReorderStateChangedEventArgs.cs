﻿using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Contains information about the <see cref="RadDataBoundListBox.ItemReorderStateChanged"/> event.
    /// </summary>
    public class ItemReorderStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReorderStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="container">The container.</param>
        internal ItemReorderStateChangedEventArgs(bool isActive, RadDataBoundListBoxItem container)
        {
            this.IsReorderActive = isActive;
            this.PivotItem = container;
        }

        /// <summary>
        /// Gets a boolean value determining whether the <see cref="RadDataBoundListBox.ItemReorderStateChanged"/> event
        /// is fired in result of activating or deactivating the reorder functionality.
        /// </summary>
        public bool IsReorderActive
        {
            get;
            private set;
        }

        /// <summary>
        /// The container that is being reordered.
        /// </summary>
        public RadDataBoundListBoxItem PivotItem
        {
            get;
            private set;
        }
    }
}
