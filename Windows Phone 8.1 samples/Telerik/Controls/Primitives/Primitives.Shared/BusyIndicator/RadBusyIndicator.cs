﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.BusyIndicator;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Resources;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that shows a progress indicator.
    /// </summary>
    [TemplatePart(Name = "PART_Animation", Type = typeof(BusyIndicatorAnimation))]
    [TemplatePart(Name = "PART_InfoContent", Type = typeof(ContentPresenter))]
    public class RadBusyIndicator : RadContentControl
    {
        /// <summary>
        /// Identifies the <see cref="InitialDelay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialDelayProperty =
            DependencyProperty.Register("InitialDelay", typeof(TimeSpan), typeof(RadBusyIndicator), new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// Identifies the <see cref="ContentPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentPositionProperty =
            DependencyProperty.Register("ContentPosition", typeof(ContentPosition), typeof(RadBusyIndicator), new PropertyMetadata(ContentPosition.Bottom, OnContentPositionChanged));

        /// <summary>
        /// Identifies the <see cref="IndicatorAnimationStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorAnimationStyleProperty =
            DependencyProperty.Register("IndicatorAnimationStyle", typeof(Style), typeof(RadBusyIndicator), null);

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(RadBusyIndicator), new PropertyMetadata(false, OnIsActiveChanged));

        /// <summary>
        /// Identifies the <see cref="AnimationStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationStyleProperty =
            DependencyProperty.Register("AnimationStyle", typeof(AnimationStyle), typeof(RadBusyIndicator), new PropertyMetadata(AnimationStyle.AnimationStyle1, OnAnimationStyleChanged));

        private ContentPosition contentPositionCache = ContentPosition.Bottom;
        private BusyIndicatorAnimation animation;
        private ContentPresenter infoContent;
        private DispatcherTimer initalDelayTimer;
        private Style[] stylesCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadBusyIndicator"/> class.
        /// </summary>
        public RadBusyIndicator()
        {
            this.stylesCache = new Style[9];
            this.DefaultStyleKey = typeof(RadBusyIndicator);
            this.initalDelayTimer = new DispatcherTimer();
            this.initalDelayTimer.Tick += this.OnInitalDelayTimer_Tick;
            this.SizeChanged += this.OnSizeChanged;
        }

        
        /// <summary>
        /// Gets or sets a value defined by the <see cref="AnimationStyle"/> enumeration
        /// that determines the type of animation shown in this <see cref="RadBusyIndicator"/>.
        /// </summary>
        public AnimationStyle AnimationStyle
        {
            get
            {
                return (AnimationStyle)this.GetValue(AnimationStyleProperty);
            }
            set
            {
                this.SetValue(AnimationStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="TimeSpan"/> struct that
        /// determines the initial delay before the <see cref="RadBusyIndicator"/> is
        /// shown and the animation is started.
        /// </summary>
        public TimeSpan InitialDelay
        {
            get
            {
                return (TimeSpan)this.GetValue(InitialDelayProperty);
            }
            set
            {
                this.SetValue(InitialDelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the animation of the busy indicator is running.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (bool)this.GetValue(IsActiveProperty);
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the content relative to the busy indicator animation.
        /// </summary>
        public ContentPosition ContentPosition
        {
            get
            {
                return this.contentPositionCache;
            }
            set
            {
                this.SetValue(ContentPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style that defines the <see cref="RadBusyIndicator"/> animation.
        /// </summary>
        public Style IndicatorAnimationStyle
        {
            get
            {
                return this.GetValue(IndicatorAnimationStyleProperty) as Style;
            }
            set
            {
                this.SetValue(IndicatorAnimationStyleProperty, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.animation = this.GetTemplateChild("PART_Animation") as BusyIndicatorAnimation;

            this.infoContent = this.GetTemplateChild("PART_InfoContent") as ContentPresenter;

            if (this.infoContent != null)
            {
                object contentValue = this.ReadLocalValue(ContentProperty);
                if (contentValue == DependencyProperty.UnsetValue)
                {
                    object actualContentValue = this.GetValue(ContentProperty);
                    if (actualContentValue == null)
                    {
                        this.Content = PrimitivesLocalizationManager.Instance.BusyIndicatorContentString;
                    }
                }
                this.SynchInfoContentPosition();
            }

            if (this.ReadLocalValue(IndicatorAnimationStyleProperty) == DependencyProperty.UnsetValue)
            {
                this.ApplyAnimationStyle(this.AnimationStyle);
            }

            this.UpdateVisualState(false);

            if (this.animation != null)
            {
                if (this.CurrentVisualState == "Running")
                {
                    this.animation.Start();
                }
                else
                {
                    this.animation.Stop();
                }
            }
        }

        /// <summary>
        /// Occurs when this object is no longer connected to the main object tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);
            this.initalDelayTimer.Stop();
            if (this.animation != null)
            {
                this.animation.Stop();
            }
        }

        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            if (this.animation != null)
            {
                if (this.CurrentVisualState == "Running")
                {
                    this.animation.Start();
                }
                else if (!this.initalDelayTimer.IsEnabled)
                {
                    this.animation.Stop();
                }
            }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        /// <returns></returns>
        protected override string ComposeVisualStateName()
        {
            if (this.IsActive)
            {
                return "Running";
            }

            return "NotRunning";
        }

        private static void OnAnimationStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBusyIndicator typedSender = sender as RadBusyIndicator;
            typedSender.OnAnimationStyleChanged(args);
        }

        private static void OnContentPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBusyIndicator typedSender = sender as RadBusyIndicator;
            typedSender.OnContentPositionChanged(args);
        }

        private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadBusyIndicator typedSender = sender as RadBusyIndicator;
            typedSender.OnIsActiveChanged(args);
        }

        private void OnInitalDelayTimer_Tick(object sender, object e)
        {
            this.UpdateVisualState(true);
            this.initalDelayTimer.Stop();
        }

        private void OnAnimationStyleChanged(DependencyPropertyChangedEventArgs args)
        {
            this.ApplyAnimationStyle((AnimationStyle)args.NewValue);
        }

        private void ApplyAnimationStyle(AnimationStyle newValue)
        {
            int styleCacheIndex = (int)newValue;

            if (this.stylesCache[styleCacheIndex] == null)
            {
                Stream stream = typeof(RadBusyIndicator).GetTypeInfo().Assembly.GetManifestResourceStream("Telerik.UI.Xaml.Controls.Primitives." + newValue.ToString() + ".xaml");

                using (StreamReader reader = new StreamReader(stream))
                {
                    string styleText = reader.ReadToEnd();
                    this.stylesCache[styleCacheIndex] = XamlReader.Load(styleText) as Style;
                }
            }

            if (this.animation != null)
            {
                this.animation.Stop();
            }
            this.IndicatorAnimationStyle = this.stylesCache[styleCacheIndex];

            if (this.animation != null && this.CurrentVisualState == "Running")
            {
                this.animation.Start();
            }
        }

        private void OnIsActiveChanged(DependencyPropertyChangedEventArgs args)
        {
            bool isActive = (bool)args.NewValue;
            TimeSpan initialDelay = this.InitialDelay;
            if (isActive)
            {
                if (initialDelay == TimeSpan.Zero)
                {
                    this.UpdateVisualState(true);
                }
                else
                {
                    this.initalDelayTimer.Interval = initialDelay;
                    this.initalDelayTimer.Start();
                }
            }
            else
            {
                this.initalDelayTimer.Stop();
                this.UpdateVisualState(true);
            }
        }

        private void OnContentPositionChanged(DependencyPropertyChangedEventArgs args)
        {
            this.contentPositionCache = (ContentPosition)args.NewValue;

            if (this.infoContent == null)
            {
                return;
            }

            this.SynchInfoContentPosition();
        }

        private void SynchInfoContentPosition()
        {
            switch (this.contentPositionCache)
            {
                case ContentPosition.Top:
                    Grid.SetRow(this.infoContent, 0);
                    Grid.SetColumn(this.infoContent, 1);
                    break;
                case ContentPosition.Bottom:
                    Grid.SetRow(this.infoContent, 2);
                    Grid.SetColumn(this.infoContent, 1);
                    break;
                case ContentPosition.Left:
                    Grid.SetRow(this.infoContent, 1);
                    Grid.SetColumn(this.infoContent, 0);
                    break;
                case ContentPosition.Right:
                    Grid.SetRow(this.infoContent, 1);
                    Grid.SetColumn(this.infoContent, 2);
                    break;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };
        }
    }
}
