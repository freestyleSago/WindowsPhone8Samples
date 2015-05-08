using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyProgress
{
    public class ProgressIndicator : ContentControl
    {
        private Rectangle backgroundRect;
        private StackPanel stackPanel;
        private ProgressBar progressBar;
        private TextBlock textBlockStatus;
        private ProgressTypes progressType = ProgressTypes.WaitCursor;

        private double progressBarValue = 0;
        private bool showLabel;
        private string labelText;

        public ProgressIndicator()
        {
            DefaultStyleKey = typeof(ProgressIndicator);
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (IsOpen)
            {
                e.Handled = true;
                Hide();
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            backgroundRect = GetTemplateChild("backgroundRect") as Rectangle;
            stackPanel = GetTemplateChild("stackPanel") as StackPanel;
            progressBar = GetTemplateChild("progressBar") as ProgressBar;
            textBlockStatus = GetTemplateChild("textBlockStatus") as TextBlock;
            InitializeProgressType();
        }

        public double ProgressBarValue
        {
            get
            {
                return progressBarValue;
            }
            set
            {
                progressBarValue = value;
                if (progressBar != null)
                {
                    progressBar.Value = value;
                }
            }
        }

        public string Text
        {
            get
            {
                return labelText;
            }
            set
            {
                labelText = value;
            }
        }

        public bool ShowLabel
        {
            get
            {
                return showLabel;
            }
            set
            {
                showLabel = value;
            }
        }

        internal Popup ChildWindowPopup
        {
            get;
            private set;
        }

        private static Frame RootVisual
        {
            get
            {
                return Window.Current == null ? null : Window.Current.Content as Frame;
            }
        }

        internal Page Page
        {
            get { return RootVisual.GetVisualDescendants().OfType<Page>().FirstOrDefault(); }
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
            if (ChildWindowPopup == null)
            {
                ChildWindowPopup = new Popup();
                ChildWindowPopup.Child = this;
            }
            ChildWindowPopup.IsOpen = true;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            if (Page != null && Page.BottomAppBar != null)
            {
                Page.BottomAppBar.IsSticky = false;
                Page.BottomAppBar.Visibility = Visibility.Collapsed;
            } 

        }

        public void Hide()
        {
             HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            if (Page != null && Page.BottomAppBar != null)
            {
                Page.BottomAppBar.Visibility = Visibility.Visible;
            } 
            ChildWindowPopup.IsOpen = false;
        }


        public ProgressTypes ProgressType
        {
            get
            {
                return progressType;
            }
            set
            {
                progressType = value;
            }
        }

        private void InitializeProgressType()
        {
            if (progressBar == null)
                return;
            progressBar.Value = 0;
            switch (progressType)
            {
                case ProgressTypes.WaitCursor:
                    backgroundRect.Visibility = Visibility.Visible;
                    stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    progressBar.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                    textBlockStatus.Visibility = Visibility.Collapsed;
                    progressBar.IsIndeterminate = true;
                    break;
                case ProgressTypes.DeterminateMiddle:
                    backgroundRect.Visibility = Visibility.Visible;
                    stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    progressBar.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                    textBlockStatus.Visibility = Visibility.Visible;
                    textBlockStatus.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                    textBlockStatus.FontSize = 20;
                    textBlockStatus.Opacity = 0.5;
                    textBlockStatus.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockStatus.Text = Text;
                    progressBar.IsIndeterminate = false;
                    break;
            }
        }

    }
}
