using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace ListAnimations
{
    public static class ListAnimationsHelper
    {
        #region AnimationLevel

        public static int GetAnimationLevel(DependencyObject obj)
        {
            return (int)obj.GetValue(AnimationLevelProperty);
        }

        public static void SetAnimationLevel(DependencyObject obj, int value)
        {
            obj.SetValue(AnimationLevelProperty, value);
        }


        public static readonly DependencyProperty AnimationLevelProperty =
            DependencyProperty.RegisterAttached("AnimationLevel", typeof(int),
            typeof(ListAnimationsHelper), new PropertyMetadata(-1));

        #endregion

        #region

        public static bool GetIsPivotAnimated(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPivotAnimatedProperty);
        }

        public static void SetIsPivotAnimated(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPivotAnimatedProperty, value);
        }

        public static readonly DependencyProperty IsPivotAnimatedProperty =
            DependencyProperty.RegisterAttached("IsPivotAnimated", typeof(bool),
            typeof(ListAnimationsHelper), new PropertyMetadata(false, OnIsPivotAnimatedChanged));

        private static bool fromRight = true;
        private static void OnIsPivotAnimatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ItemsControl list = d as ItemsControl;

            list.Loaded += (s2, e2) =>
            {

                Pivot pivot = GetParent<Pivot>(list);
                int pivotIndex = pivot.Items.IndexOf(GetParent<PivotItem>(list));

                pivot.SelectionChanged += (s3, e3) =>
                {
                    if (pivotIndex == pivot.SelectedIndex)
                        return;
                    var items = list.GetItemsInView().ToList();
                    AddSlideAnimation(items);
                };

                var items2 = list.GetItemsInView().ToList();
                AddSlideAnimation(items2);
            };
        }

        private async static void AddSlideAnimation(List<FrameworkElement> items)
        {
            for (int index = 0; index < items.Count; index++)
            {
                var lbi = items[index];

                await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    var animationTargets = lbi.Descendants()
                                           .Where(p => ListAnimationsHelper.GetAnimationLevel(p) > -1);
                    foreach (FrameworkElement target in animationTargets)
                    {
                        GetSlideAnimation(target, fromRight).Begin();
                    }
                });
            };
        }
        #endregion

        public static void Peel(this IEnumerable<FrameworkElement> elements, Action endAction)
        {
            var elementList = elements.ToList();
            var lastElement = elementList.Last();

            double delay = 0;
            foreach (FrameworkElement element in elementList)
            {
                var sb = GetPeelAnimation(element, delay);

                if (element.Equals(lastElement))
                {
                    sb.Completed += (s, e) =>
                    {
                        endAction();
                    };
                }

                sb.Begin();
                delay += 50;
            }
        }


        public static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl)
        {
            VirtualizingStackPanel vsp = GetChild<VirtualizingStackPanel>(itemsControl);

            int firstVisibleItem = (int)vsp.VerticalOffset;
            int visibleItemCount = (int)vsp.ViewportHeight;
            for (int index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount + 1; index++)
            {
                var item = itemsControl.ContainerFromIndex(index) as FrameworkElement;
                if (item == null)
                    continue;

                yield return item;
            }
        }

        public static T GetParent<T>(DependencyObject item) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (parent != null)
            {
                if (parent is T) return parent as T;
                else parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static T GetChild<T>(DependencyObject item) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(item);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }

        public static IEnumerable<DependencyObject> Descendants(this DependencyObject item)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(item);
            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    if (child != null)
                    {
                        yield return child;
                        queue.Enqueue(child);
                    }
                }
            }
        }

        private static Storyboard GetPeelAnimation(FrameworkElement element, double delay)
        {
            Storyboard sb;

            var projection = new PlaneProjection()
            {
                CenterOfRotationX = -0.1
            };
            element.Projection = projection;

            var width = element.ActualWidth;
            var targetAngle = Math.Atan(1000 / (width / 2));
            targetAngle = targetAngle * 180 / Math.PI;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromMilliseconds(delay);
            sb.Children.Add(CreateAnimation(0, -(180 - targetAngle), 0.3, "RotationY", projection));
            sb.Children.Add(CreateAnimation(0, 23, 0.3, "RotationZ", projection));
            sb.Children.Add(CreateAnimation(0, -23, 0.3, "GlobalOffsetZ", projection));
            return sb;
        }

        private static DoubleAnimation CreateAnimation(double from, double to, double duration,
          string targetProperty, DependencyObject target)
        {
            var db = new DoubleAnimation();
            db.To = to;
            db.From = from;

            db.EasingFunction = new SineEase();
            db.Duration = TimeSpan.FromSeconds(duration);
            Storyboard.SetTarget(db, target);
            Storyboard.SetTargetProperty(db, targetProperty);
            return db;
        }

        private static Storyboard GetSlideAnimation(FrameworkElement element, bool fromRight)
        {
            double from = fromRight ? 80 : -80;

            Storyboard sb;
            double delay = (ListAnimationsHelper.GetAnimationLevel(element)) * 0.1 + 0.1;
            TranslateTransform trans = new TranslateTransform() { X = from };
            element.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);
            sb.Children.Add(CreateAnimation(from, 0, 0.8, "X", trans));
            return sb;
        }
    }
}
