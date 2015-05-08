using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace MessageControl
{
    public class MyMessage : ContentControl
    {
        private ContentPresenter body;
        private Rectangle backgroundRect;
        private object content;
        private Visibility appBarVisual;
        public MyMessage()
        {
            this.DefaultStyleKey = typeof(MyMessage);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.body = this.GetTemplateChild("body") as ContentPresenter;
            this.backgroundRect = this.GetTemplateChild("backgroundRect") as Rectangle;
            InitializeMessagePrompt();

        }

        internal Popup ChildWindowPopup
        {
            get;
            private set;
        }

        private static Page Page
        {
            get { return RootVisual.GetVisualDescendants().OfType<Page>().FirstOrDefault(); }
        }

        private static Frame RootVisual
        {
            get
            {
                return Application.Current == null ? null : Window.Current.Content as Frame;
            }
        }

        public object MessageContent
        {
            get
            {
                return this.content;
            }
            set
            {
                this.content = value;
            }
        }

        public void Hide()
        {
            if (this.body != null)
            {
                this.ChildWindowPopup.IsOpen = false;
            }
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            if (Page.BottomAppBar != null)
                Page.BottomAppBar.Visibility = appBarVisual;
        }

        public bool IsOpen
        {
            get
            {
                return ChildWindowPopup != null && ChildWindowPopup.IsOpen;
            }
        }

        public void Show()
        {
            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new Popup();
                this.ChildWindowPopup.Child = this;
            }

            if (this.ChildWindowPopup != null && Window.Current.Content != null)
            {
                InitializeMessagePrompt();
                this.ChildWindowPopup.IsOpen = true;
            }
            //CommandBar
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            if (Page != null)
            {
                appBarVisual = Page.BottomAppBar.Visibility;
                Page.BottomAppBar.IsSticky = false;
                Page.BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void InitializeMessagePrompt()
        {
            if (this.body == null)
                return;
            this.backgroundRect.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.body.Content = MessageContent;
            this.Height = 800;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }
            if (IsOpen)
            {
                e.Handled = true;
                Hide();
            }
        }
    }
}
