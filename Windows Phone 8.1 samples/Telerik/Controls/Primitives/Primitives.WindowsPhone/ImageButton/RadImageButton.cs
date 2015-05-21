using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that is a Button which has an image as its content.
    /// </summary>
    [TemplatePart(Name = "PART_ButtonImage", Type = typeof(Image))]
    [TemplatePart(Name = "PART_RectangleOpacityMaskImageBrush", Type = typeof(ImageBrush))]
    [TemplatePart(Name = "PART_EllipseOpacityMaskImageBrush", Type = typeof(ImageBrush))]
    public class RadImageButton : Button
    {
        /// <summary>
        /// Identifies the <see cref="ButtonType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ImageButtonType), typeof(RadImageButton), new PropertyMetadata(ImageButtonType.Search, OnButtonTypeChanged));

        /// <summary>
        /// Identifies the <see cref="ButtonBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonBehaviorProperty =
            DependencyProperty.Register("ButtonBehavior", typeof(ImageButtonBehavior), typeof(RadImageButton), new PropertyMetadata(ImageButtonBehavior.Normal, OnButtonBehaviorChanged));

        /// <summary>
        /// Identifies the <see cref="ButtonShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonShapeProperty =
            DependencyProperty.Register("ButtonShape", typeof(ImageButtonShape), typeof(RadImageButton), new PropertyMetadata(ImageButtonShape.Rectangle, OnButtonShapeChanged));

        /// <summary>
        /// Identifies the <see cref="RestStateImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RestStateImageSourceProperty =
            DependencyProperty.Register("RestStateImageSource", typeof(ImageSource), typeof(RadImageButton), new PropertyMetadata(null, OnRestStateImageSourceChanged));

        /// <summary>
        /// Identifies the <see cref="PressedStateImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PressedStateImageSourceProperty =
            DependencyProperty.Register("PressedStateImageSource", typeof(ImageSource), typeof(RadImageButton), new PropertyMetadata(null, OnImageSourceChanged));

        /// <summary>
        /// Identifies the <see cref="DisabledStateImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledStateImageSourceProperty =
            DependencyProperty.Register("DisabledStateImageSource", typeof(ImageSource), typeof(RadImageButton), new PropertyMetadata(null, OnImageSourceChanged));

        /// <summary>
        /// Identifies the <see cref="IsChecked"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(RadImageButton), new PropertyMetadata(false, OnIsCheckedChanged));

        /// <summary>
        /// Identifies the <see cref="ImageStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(RadImageButton), new PropertyMetadata(Stretch.None));

        internal bool isButtonPressed;
        private bool isTemplateApplied = false;
        private bool isOpacityMaskInitialized = false;
        private ImageBrush rectangleOpacityMaskImageBrush;
        private ImageBrush ellipseOpacityMaskImageBrush;
        private string currentVisualState = string.Empty;
        private Image buttonImage;
        private bool isImageSourceUpdatingSilently = false;
        private bool isCheckedUpdatingInternally = false;
        private int visualStateUpdateLock = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadImageButton"/> class.
        /// </summary>
        public RadImageButton()
        {
            this.IsEnabledChanged += this.OnIsEnabledChanged;
            //TelerikLicense.Verify(this);
            this.DefaultStyleKey = typeof(RadImageButton);
        }

        /// <summary>
        /// Occurs when the IsChecked property has changed.
        /// </summary>
        public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

        /// <summary>
        /// Occurs when the IsChecked property is set to true.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked;

        /// <summary>
        /// Occurs when the IsChecked property is set to false.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked;

        /// <summary>
        /// Gets or sets the type of the button. If this type is different than <see cref="ImageButtonType.Custom"/>, 
        /// the images used for the button will be relevant on the type and not on the values of the ImageSource properties.
        /// </summary>
        public ImageButtonType ButtonType
        {
            get
            {
                return (ImageButtonType)this.GetValue(ButtonTypeProperty);
            }
            set
            {
                this.SetValue(ButtonTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance will behave like a normal button, or like a toggle button, for example.
        /// </summary>
        public ImageButtonBehavior ButtonBehavior
        {
            get
            {
                return (ImageButtonBehavior)this.GetValue(ButtonBehaviorProperty);
            }
            set
            {
                this.SetValue(ButtonBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what is the shape of the button.
        /// </summary>
        public ImageButtonShape ButtonShape
        {
            get
            {
                return (ImageButtonShape)this.GetValue(ButtonShapeProperty);
            }
            set
            {
                this.SetValue(ButtonShapeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageSource that will be used for the button when it is not pressed.
        /// </summary>
        public ImageSource RestStateImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(RestStateImageSourceProperty);
            }
            set
            {
                this.SetValue(RestStateImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageSource that will be used for the button when it is pressed.
        /// </summary>
        public ImageSource PressedStateImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(PressedStateImageSourceProperty);
            }
            set
            {
                this.SetValue(PressedStateImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageSource that will be used for the button when it is disabled.
        /// </summary>
        public ImageSource DisabledStateImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(DisabledStateImageSourceProperty);
            }
            set
            {
                this.SetValue(DisabledStateImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button is checked if it is a ToggleButton.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return (bool)this.GetValue(IsCheckedProperty);
            }
            set
            {
                this.SetValue(IsCheckedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the type of Stretch that will be used for the image in this image button.
        /// </summary>
        public Stretch ImageStretch
        {
            get
            {
                return (Stretch)this.GetValue(ImageStretchProperty);
            }
            set
            {
                this.SetValue(ImageStretchProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button is currently pressed. This property is used internally and should not be set manually!
        /// </summary>
        public bool IsButtonPressed
        {
            get
            {
                return this.isButtonPressed;
            }
            set
            {
                this.isButtonPressed = value;
            }
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.Button"/> when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.buttonImage = GetTemplateChild("PART_ButtonImage") as Image;

            if (this.buttonImage == null)
            {
                throw new MissingTemplatePartException("PART_ButtonImage", typeof(Image));
            }

            this.SetButtonImageSource();
            this.isTemplateApplied = true;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Updates the state of the visual. This method is used internally and should not be called manually!
        /// </summary>
        public void UpdateVisualState(bool animate)
        {
            if (!this.CanUpdateVisualState())
            {
                return;
            }

            string state = this.ComposeVisualStateName();
            if (state != this.currentVisualState)
            {
                this.currentVisualState = state;
                this.SetVisualState(state, animate);
            }
        }

        internal virtual string ComposeVisualStateName()
        {
            string visualStateName;

            if (!this.IsEnabled)
            {
                visualStateName = "DisabledState";
            }
            else
            {
                if (!this.isButtonPressed)
                {
                    visualStateName = "RestState";
                }
                else
                {
                    visualStateName = "PressedState";
                }
            }
            if (this.ButtonShape != ImageButtonShape.Image)
            {
                visualStateName += "WithOpacityMask";
            }
            return visualStateName;
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationStarted"/> event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
            if (this.ButtonBehavior == ImageButtonBehavior.ToggleButton)
            {
                this.isButtonPressed ^= true;
                this.isCheckedUpdatingInternally = true;
                this.IsChecked ^= true;
                this.isCheckedUpdatingInternally = false;
            }
            else
            {
                this.isButtonPressed = true;
            }
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/>  event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            if (this.ButtonBehavior != ImageButtonBehavior.ToggleButton)
            {
                this.isButtonPressed = false;
                this.UpdateVisualState(true);
            }
        }

        private static void OnButtonTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageButton button = sender as RadImageButton;
            if (!button.isTemplateApplied)
            {
                return;
            }
            if (button.ButtonType != ImageButtonType.Custom)
            {
                button.isImageSourceUpdatingSilently = true;
                button.RestStateImageSource = null;
                button.isImageSourceUpdatingSilently = false;
            }
            button.SetButtonImageSource();
            button.UpdateVisualState(true);
        }

        private static void OnRestStateImageSourceChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageButton button = sender as RadImageButton;
            if (!button.isTemplateApplied)
            {
                return;
            }
            button.SetButtonImageSource();
            OnImageSourceChanged(sender, args);
        }

        private static void OnImageSourceChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageButton button = sender as RadImageButton;
            if (button.isImageSourceUpdatingSilently)
            {
                return;
            }
            if (!button.isTemplateApplied)
            {
                return;
            }
            if (button.ButtonShape != ImageButtonShape.Image)
            {
                //button.UpdateOpacityMask();
            }
            button.UpdateVisualState(true);
        }

        private static void OnButtonShapeChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageButton button = sender as RadImageButton;
            if (button.buttonImage == null)
            {
                return;
            }
            if (button.ButtonShape != ImageButtonShape.Image)
            {
                //button.UpdateOpacityMask();
            }
            button.UpdateVisualState(true);
        }

        private static void OnIsCheckedChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageButton button = sender as RadImageButton;
            if (button.isCheckedUpdatingInternally)
            {
                button.RaiseCheckedChangedEvents();
                return;
            }
            if (button.ButtonBehavior != ImageButtonBehavior.ToggleButton)
            {
                throw new InvalidOperationException("The IsChecked property should be used only if the button's ButtonBehavior is ToggleButton.");
            }
            button.isButtonPressed = button.IsChecked;
            button.RaiseCheckedChangedEvents();
            button.UpdateVisualState(true);
        }

        private static void OnButtonBehaviorChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageButton button = sender as RadImageButton;
            if (button.ButtonBehavior != ImageButtonBehavior.ToggleButton && button.IsChecked)
            {
                throw new InvalidOperationException("The IsChecked property should be used only if the button's ButtonBehavior is ToggleButton.");
            }
            button.UpdateVisualState(true);
        }

        private void RaiseCheckedChangedEvents()
        {
            if (this.IsChecked == true)
            {
                if (this.Checked != null)
                {
                    this.Checked(this, new RoutedEventArgs());
                }
            }
            else
            {
                if (this.Unchecked != null)
                {
                    this.Unchecked(this, new RoutedEventArgs());
                }
            }
            if (this.CheckedChanged != null)
            {
                this.CheckedChanged(this, new CheckedChangedEventArgs(!this.IsChecked, this.IsChecked));
            }
        }

        //private void InitializeOpacityMask()
        //{
        //    this.rectangleOpacityMaskImageBrush = this.GetTemplateChild("PART_RectangleOpacityMaskImageBrush") as ImageBrush;
        //    if (this.rectangleOpacityMaskImageBrush == null)
        //    {
        //        throw new MissingTemplatePartException("PART_RectangleOpacityMaskImageBrush", typeof(ImageBrush));
        //    }
        //    this.ellipseOpacityMaskImageBrush = this.GetTemplateChild("PART_EllipseOpacityMaskImageBrush") as ImageBrush;
        //    if (this.ellipseOpacityMaskImageBrush == null)
        //    {
        //        throw new MissingTemplatePartException("PART_EllipseOpacityMaskImageBrush", typeof(ImageBrush));
        //    }
        //    this.isOpacityMaskInitialized = true;
        //}

        //private void UpdateOpacityMask()
        //{
        //    if (!this.isOpacityMaskInitialized)
        //    {
        //        this.InitializeOpacityMask();
        //    }
        //    this.rectangleOpacityMaskImageBrush.ImageSource = this.GetImageSourceFromButtonType(this.ButtonType);
        //    this.ellipseOpacityMaskImageBrush.ImageSource = this.GetImageSourceFromButtonType(this.ButtonType);
        //}

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            this.UpdateVisualState(true);
        }

        private void BeginVisualStateUpdate()
        {
            this.visualStateUpdateLock++;
        }

        private void EndVisualStateUpdate(bool update, bool animate)
        {
            if (this.visualStateUpdateLock > 0)
            {
                this.visualStateUpdateLock--;
            }

            if (update && this.visualStateUpdateLock == 0)
            {
                this.UpdateVisualState(animate);
            }
        }

        private void SetVisualState(string state, bool animate)
        {
            string[] states = state.Split(RadControl.VisualStateDelimiter);
            foreach (string visualState in states)
            {
                VisualStateManager.GoToState(this, visualState, animate);
            }
        }

        private bool CanUpdateVisualState()
        {
            return this.isTemplateApplied && this.visualStateUpdateLock == 0;
        }

        private void SetButtonImageSource()
        {
            this.buttonImage.Source = this.GetImageSourceFromButtonType(this.ButtonType);

            if (this.ButtonShape != ImageButtonShape.Image)
            {
                //this.UpdateOpacityMask();
            }
        }

        private ImageSource GetImageSourceFromButtonType(ImageButtonType buttonType)
        {
            if (this.RestStateImageSource != null)
            {
                return this.RestStateImageSource;
            }
            switch (buttonType)
            {
                case ImageButtonType.Search:
                    return new BitmapImage(new Uri("ms-appx:///Telerik.UI.Xaml.Controls.Primitives/ImageButton/Images/SearchButton.png", UriKind.Absolute));
                case ImageButtonType.Clear:
                    return new BitmapImage(new Uri("ms-appx:///Telerik.UI.Xaml.Controls.Primitives/ImageButton/Images/ClearButton.png", UriKind.Absolute));
                case ImageButtonType.Arrow:
                    return new BitmapImage(new Uri("ms-appx:///Telerik.UI.Xaml.Controls.Primitives/ImageButton/Images/ArrowButton.png", UriKind.Absolute));
                case ImageButtonType.Peek:
                    return new BitmapImage(new Uri("ms-appx:///Telerik.UI.Xaml.Controls.Primitives/ImageButton/Images/PeekButton.png", UriKind.Absolute));
                case ImageButtonType.Custom:
                    return this.RestStateImageSource;
            }
            return null;
        }
    }
}
