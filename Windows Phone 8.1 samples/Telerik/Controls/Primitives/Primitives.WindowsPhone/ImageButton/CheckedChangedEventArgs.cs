using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Encapsulates the arguments associated with the <see cref="E:RadToggleSwitch.CheckedChanged"/> event.
    /// </summary>
    public class CheckedChangedEventArgs : EventArgs
    {
        private bool prevState;
        private bool newState;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedChangedEventArgs"/> class.
        /// </summary>
        /// <param name="prevState">The previous state.</param>
        /// <param name="newState">The new state.</param>
        public CheckedChangedEventArgs(bool prevState, bool newState)
        {
            this.prevState = prevState;
            this.newState = newState;
        }

        /// <summary>
        /// Gets the previous state.
        /// </summary>
        public bool PreviousState
        {
            get
            {
                return this.prevState;
            }
        }

        /// <summary>
        /// Gets the new state.
        /// </summary>
        public bool NewState
        {
            get
            {
                return this.newState;
            }
        }
    }
}
