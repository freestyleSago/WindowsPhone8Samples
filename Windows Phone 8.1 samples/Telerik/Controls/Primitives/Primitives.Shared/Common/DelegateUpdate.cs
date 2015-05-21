using System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class DelegateUpdate
    {
        private Action updateAction;

        public DelegateUpdate(Action action)
        {
            this.updateAction = action;
            this.Priority = CoreDispatcherPriority.Normal;
        }

        internal virtual void Process()
        {
            if (this.updateAction != null)
            {
                this.updateAction();
            }
        }

        public CoreDispatcherPriority Priority { get; set; }
    }
}
