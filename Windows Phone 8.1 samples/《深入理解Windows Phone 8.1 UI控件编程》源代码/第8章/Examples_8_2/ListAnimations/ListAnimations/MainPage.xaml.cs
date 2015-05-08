using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ListAnimations
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }



        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }
            if (!frame.CanGoBack)
            {
                e.Handled = true;

                var listInView = ((PivotItem)pivot.SelectedItem).Descendants().OfType<ItemsControl>().Single();
                var listItems = listInView.GetItemsInView().ToList();

                var peelList = new FrameworkElement[] { TitlePanel }.Union(listItems);

                peelList.Peel(() =>
                {

                    Application.Current.Exit();
                });
            }
        }

        //protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;

        //    var listInView = ((PivotItem)pivot.SelectedItem).Descendants().OfType<ItemsControl>().Single();
        //    var listItems = listInView.GetItemsInView().ToList();

        //    var header = this.Descendants().OfType<FrameworkElement>().Single(d => d.Name == "HeadersListElement");

        //    var peelList = new FrameworkElement[] { TitlePanel, header }.Union(listItems);

        //    peelList.Peel(() =>
        //    {
        //       // Application.Current.Terminate();
        //    });
        //}

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            List<Item> datas = new List<Item>();
            datas.Add(new Item { Title = "Title1", Text1 = "测试数据1", Text2 = "测试数据1" });
            datas.Add(new Item { Title = "Title2", Text1 = "测试数据2", Text2 = "测试数据2" });
            datas.Add(new Item { Title = "Title3", Text1 = "测试数据3", Text2 = "测试数据3" });
            datas.Add(new Item { Title = "Title4", Text1 = "测试数据4", Text2 = "测试数据4" });
            datas.Add(new Item { Title = "Title5", Text1 = "测试数据5", Text2 = "测试数据5" });
            listBox1.ItemsSource = datas;
            listBox2.ItemsSource = datas;
        }

    }

    public class Item
    {
        public string Title { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
    }
}
