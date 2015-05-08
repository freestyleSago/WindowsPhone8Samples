using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyProgressDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MyProgress.ProgressIndicator progressIndicator = new MyProgress.ProgressIndicator();
            progressIndicator.Text = "正在加载中";
            progressIndicator.Show();

            Task.Factory.StartNew(async () =>
            {
                Task.Delay(3000).Wait();
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    progressIndicator.Hide();
                });
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MyProgress.ProgressIndicator progressIndicator = new MyProgress.ProgressIndicator();
            progressIndicator.Text = "处理的进度";
            progressIndicator.ProgressType = MyProgress.ProgressTypes.DeterminateMiddle;
            progressIndicator.Show();

            Task.Factory.StartNew(async () =>
            {

                for (int i = 0; i <= 10; i++)
                {

                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        progressIndicator.ProgressBarValue = i * 10;
                        if (i >= 10)
                        {
                            progressIndicator.Hide();
                        }
                    });
                    Task.Delay(1000).Wait();
                }
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
