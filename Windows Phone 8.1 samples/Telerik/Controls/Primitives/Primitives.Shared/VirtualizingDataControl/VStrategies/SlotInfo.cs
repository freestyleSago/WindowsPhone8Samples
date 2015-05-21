
using Windows.Foundation;
namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class SlotInfo
    {
        internal SlotInfo(Rect minSlot, Rect maxSlot)
        {
            this.MinSlot = minSlot;
            this.MaxSlot = maxSlot;
        }

        public Rect MinSlot
        {
            get;
            private set;
        }

        public Rect MaxSlot
        {
            get;
            private set;
        }
    }
}
