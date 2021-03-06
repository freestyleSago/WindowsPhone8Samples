﻿using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Holds information about the <see cref="RadDataBoundListBox.IsCheckModeActiveChanging"/> event.
    /// </summary>
    public class IsCheckModeActiveChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsCheckModeActiveChangingEventArgs" /> class.
        /// </summary>
        /// <param name="checkBoxesVisible">True if the check mode is about to be activated. False otherwise.</param>
        public IsCheckModeActiveChangingEventArgs(bool checkBoxesVisible)
        {
            this.CheckBoxesVisible = checkBoxesVisible;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsCheckModeActiveChangingEventArgs"/> class.
        /// </summary>
        /// <param name="checkBoxesVisible">True if the check mode is about to be activated. False otherwise.</param>
        /// <param name="tappedItem">The data item from the source collection that has been tapped to activate the check mode.</param>
        public IsCheckModeActiveChangingEventArgs(bool checkBoxesVisible, object tappedItem)
        {
            this.CheckBoxesVisible = checkBoxesVisible;
            this.TappedItem = tappedItem;
        }

        /// <summary>
        /// Holds a reference to the data item from the source collection
        /// which has been initially tapped to activate the check mode.
        /// This property will return null when the change is not a result
        /// from end-user input.
        /// </summary>
        public object TappedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value determining the new check mode state
        /// of the <see cref="RadDataBoundListBox"/>.
        /// </summary>
        public bool CheckBoxesVisible
        {
            get;
            private set;
        }
    }
}
