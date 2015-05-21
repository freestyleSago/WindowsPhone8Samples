using System;
using System.IO;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.License
{
    internal class ToastNotification
    {
        private Popup popup;
        private ToastPresenter content;
        private TrialViewModel viewModel;

        public ToastNotification()
        {
            this.viewModel = new TrialViewModel();

            this.content = new ToastPresenter();
            this.content.Content = this.viewModel;
            this.LoadContentTemplate();
            this.content.Dismissed += this.OnDismissed;

            this.popup = new Popup();
            this.popup.Child = this.content;
        }

        public void Show()
        {
            this.SetPopupPosition();

            // TODO: The IsOpen call is Asynchronous as a synchronous one will not work in case we are already in external Popup
            var warningSuppression = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.popup.IsOpen = true);
        }

        internal void LoadContentTemplate()
        {
            Assembly assembly = typeof(ToastNotification).GetTypeInfo().Assembly;
            string path = "Telerik.UI.Xaml.Controls.Primitives.License.Resources.ToastTemplate.xaml";
            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                StreamReader reader = new StreamReader(stream);
                ResourceDictionary dictionary = XamlReader.Load(reader.ReadToEnd()) as ResourceDictionary;

                this.content.ContentTemplate = dictionary["ToastTemplate"] as DataTemplate;
                this.content.Foreground = dictionary["ForegroundBrush"] as Brush;
            }
        }

        private void SetPopupPosition()
        {
            Rect windowBounds = Window.Current.Bounds;
            double offsetX = windowBounds.Width - this.viewModel.Width;

            this.popup.HorizontalOffset = offsetX;
            this.popup.VerticalOffset = this.viewModel.OffsetTop;
        }

        private void OnDismissed(object sender, EventArgs e)
        {
            this.popup.IsOpen = false;
        }
    }
}
