using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ListBoxUpdateDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int pageCount = 30;
        public MainPage()
        {
            InitializeComponent();
            for (int i = 0; i < pageCount; i++)
            {
                listbox1.Items.Add("项目" + i);
            }
            listbox1.Loaded += listbox1_Loaded;

        }
        ScrollViewer scrollViewer;
        void listbox1_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = FindChildOfType<ScrollViewer>(listbox1);
            scrollViewer.ViewChanged += scrollViewer_ViewChanged;
        }

        void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (scrollViewer == null)
            {
                throw new InvalidOperationException("erro");
            }
            else
            {
                if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight)
                {
                    for (int i = 0; i < pageCount; i++)
                    {
                        int k = listbox1.Items.Count;
                        listbox1.Items.Add("项目" + k);
                        k++;
                    }
                }
            }
        }

        static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
