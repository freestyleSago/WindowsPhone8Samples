using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal interface IDragDropElement
    {
        bool CanStartDrag();

        DragStartingContext DragStarting();

        void DragEnter(DragContext context);

        void DragOver(DragContext context);

        void DragLeave(DragContext context);

        bool CanDrop(DragContext dragContext);

        void OnDrop(DragContext dragContext);

        void OnDragDropComplete(DragCompleteContext dragContext);

        void OnDragVisualCleared(DragCompleteContext dragContext);   
    }
}
