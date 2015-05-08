using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace XamlReaderDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public void bt_addXAML_Click(object sender, RoutedEventArgs e)
        {
            string buttonXAML = "<Button xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'  " +
                 " Content=\"加载XAML文件\"  Foreground=\"Red\"></Button>";
            Button btnRed = (Button)XamlReader.Load(buttonXAML);
            btnRed.Click += btnRed_Click;
            sp_show.Children.Add(btnRed);
        }

        public async void btnRed_Click(object sender, RoutedEventArgs e)
        {
            string xaml = string.Empty;
            StorageFile fileRead = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Rectangle.xaml");
            xaml = await FileIO.ReadTextAsync(fileRead);
            Rectangle rectangle = (Rectangle)XamlReader.Load(xaml);
            sp_show.Children.Add(rectangle);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
