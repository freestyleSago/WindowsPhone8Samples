using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal static class DragDrop
    {
        // Using a DependencyProperty as the backing store for AllowDrag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDragProperty =
            DependencyProperty.RegisterAttached("AllowDrag", typeof(bool), typeof(DragDrop), new PropertyMetadata(false, OnAllowDragChanged));

        // Using a DependencyProperty as the backing store for AllowDrop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(DragDrop), new PropertyMetadata(false, OnAllowDropChanged));

        // Using a DependencyProperty as the backing store for DragPositionMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragPositionModeProperty =
            DependencyProperty.RegisterAttached("DragPositionMode", typeof(DragPositionMode), typeof(DragDrop), new PropertyMetadata(DragPositionMode.Free));

        private static DependencyProperty dragInitializerProperty =
            DependencyProperty.RegisterAttached("dragInitializer", typeof(DragInitializer), typeof(DragDrop), new PropertyMetadata(null));
        
        private static List<DragDropOperation> runningOperations = new List<DragDropOperation>();

        public static bool GetAllowDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDragProperty);
        }

        public static void SetAllowDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDragProperty, value);
        }

        public static bool GetAllowDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDropProperty);
        }

        public static void SetAllowDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDropProperty, value);
        }

        public static DragPositionMode GetDragPositionMode(DependencyObject obj)
        {
            return (DragPositionMode)obj.GetValue(DragPositionModeProperty);
        }

        public static void SetDragPositionMode(DependencyObject obj, DragPositionMode value)
        {
            obj.SetValue(DragPositionModeProperty, value);
        }

        internal static void StartDrag(object sender, PointerRoutedEventArgs e)
        {
            var dragDropElement = sender as IDragDropElement;
            var uiSource = sender as UIElement;

            if (dragDropElement == null || !dragDropElement.CanStartDrag())
            {
                return;
            }

            var context = dragDropElement.DragStarting();

            var startDragPosition = e.GetCurrentPoint(context.DragSurface.RootElement).Position;
            var relativeStartDragPosition = e.GetCurrentPoint(uiSource).Position;
            var dragPositionMode = DragDrop.GetDragPositionMode(uiSource);

            runningOperations.Add(new DragDropOperation(context, dragDropElement, dragPositionMode, e.Pointer, startDragPosition, relativeStartDragPosition));
        }

        internal static void OnOperationFinished(DragDropOperation dragDropOperation)
        {
            runningOperations.Remove(dragDropOperation);
        }

        private static void OnAllowDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnAllowDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            if ((bool)e.NewValue)
            {
                element.SetValue(DragDrop.dragInitializerProperty, new DragInitializer(element));
            }
            else
            {
                element.ClearValue(DragDrop.dragInitializerProperty);
            }
        }
    }
}
