using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CustomThemesDemo
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

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            ChangeTheme(Colors.White, Colors.Orange);
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            ChangeTheme(Colors.Orange, Colors.LightGray);
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            ChangeTheme(Colors.LightGray, Colors.Red);
        }

        private void ChangeTheme(Color backgroundColor, Color accentColor)
        {
            (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color = accentColor;
            (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = accentColor;
            (App.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = backgroundColor;
            (App.Current.Resources["PhoneRadioCheckBoxBrush"] as SolidColorBrush).Color = backgroundColor;
            (App.Current.Resources["PhoneRadioCheckBoxPressedBrush"] as SolidColorBrush).Color = accentColor;
            (App.Current.Resources["PhoneRadioCheckBoxBrush"] as SolidColorBrush).Color = accentColor;
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
