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

namespace GameDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            walkingStoryboard.Completed += walkingStoryboard_Completed;

        }

        private void _player1_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            player2.IsHitTestVisible = false;
            player1.IsHitTestVisible = false;
            player2Shadow.Visibility = Visibility.Collapsed;
            player2.Visibility = Visibility.Collapsed;
            player1Blood.Visibility = Visibility.Visible;
            player1Die.Visibility = Visibility.Visible;
            player1DieStoryboard.Begin();
        }

        private void _player2_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            player2.IsHitTestVisible = false;
            player1.IsHitTestVisible = false;

            this.player1Shadow.Visibility = Visibility.Collapsed;
            this.player1.Visibility = Visibility.Collapsed;

            this.player2Blood.Visibility = Visibility.Visible;
            player2Die.Visibility = Visibility.Visible;
            this.player2DieStoryboard.Begin();
        }

        private void walkingStoryboard_Completed(object sender, object e)
        {
            this.player1.IsHitTestVisible = true;
            this.player2.IsHitTestVisible = true;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            walkingStoryboard.Begin();
        }
    }
}
