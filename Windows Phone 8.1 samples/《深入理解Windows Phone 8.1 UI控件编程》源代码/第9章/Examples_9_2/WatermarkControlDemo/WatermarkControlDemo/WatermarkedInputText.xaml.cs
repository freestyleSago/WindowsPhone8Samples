using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WatermarkControlDemo
{
    public sealed partial class WatermarkedInputText : UserControl
    {
        public WatermarkedInputText()
        {
            InitializeComponent();
        }

        private void MWInput_GotFocus_1(object sender, RoutedEventArgs e)
        {
            this.WMText.Opacity = 0;
        }

        private void MWInput_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (this.WMInput.Text == "")
            {
                this.WMText.Opacity = 1;
            }
            else
            {
                this.WMText.Opacity = 0;
            }
        }

        public string Watermark
        {
            get
            {
                return this.WMText.Text;
            }
            set
            {
                this.WMText.Text = value;
            }
        }

        public string Text
        {
            get
            {
                return this.WMInput.Text;
            }
            set
            {
                this.WMInput.Text = value;
            }
        }

        public InputScope InputScope
        {
            get
            {
                return WMInput.InputScope;
            }
            set
            {
                WMInput.InputScope = value;
            }

        }
    }
}
