using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UIThreadDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SynchronizationContext context;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ThreadProc1();
            });
        }

        public void ThreadProc1()
        {
            context.Post(async (s) =>
            {
                MessageDialog messageDialog = new MessageDialog("ThreadProc1");
                await messageDialog.ShowAsync();
            }, null);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ThreadProc2();
            });
        }

        public async void ThreadProc2()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog messageDialog = new MessageDialog("ThreadProc2");
                await messageDialog.ShowAsync();
            });

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
