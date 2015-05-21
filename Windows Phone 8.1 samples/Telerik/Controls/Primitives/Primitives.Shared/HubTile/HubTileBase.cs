﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Base class for all hub tiles.
    /// </summary>
    public abstract class HubTileBase : RadControl
    {
        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(HubTileBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TitleTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(HubTileBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the UpdateInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateIntervalProperty =
            DependencyProperty.Register("UpdateInterval", typeof(TimeSpan), typeof(HubTileBase), new PropertyMetadata(TimeSpan.FromSeconds(3), OnUpdateIntervalChanged));

        /// <summary>
        /// Identifies the IsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFrozenProperty =
           DependencyProperty.Register("IsFrozen", typeof(bool), typeof(HubTileBase), new PropertyMetadata(false, OnIsFrozenChanged));

        /// <summary>
        /// Identifies the BackContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentProperty =
            DependencyProperty.Register("BackContent", typeof(object), typeof(HubTileBase), new PropertyMetadata(null, OnBackContentChanged));

        /// <summary>
        /// Identifies the BackContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentTemplateProperty =
            DependencyProperty.Register("BackContentTemplate", typeof(DataTemplate), typeof(HubTileBase), new PropertyMetadata(null, OnBackContentChanged));

        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(HubTileBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsFlipped dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFlippedProperty =
            DependencyProperty.Register("IsFlipped", typeof(bool), typeof(HubTileBase), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(HubTileBase), new PropertyMetadata(null));

        private DispatcherTimer updateTimer = new DispatcherTimer();
        private UIElement layoutRoot;
        private FlipControl flipControl;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The tilt effect allowed types may not be initialized outside the constructor.")]
        static HubTileBase()
        {
            InteractionEffectManager.AllowedTypes.Add(typeof(HubTileBase));
        }

        /// <summary>
        /// Initializes a new instance of the HubTileBase class.
        /// </summary>
        protected HubTileBase()
        {
            this.DefaultStyleKey = typeof(HubTileBase);
            HubTileService.Tiles.Add(this);

            this.updateTimer.Tick += this.OnUpdateTimerTick;
            this.updateTimer.Interval = this.UpdateInterval;

            this.SizeChanged += this.OnSizeChanged;

            InteractionEffectManager.SetIsInteractionEnabled(this, true);
        }

        /// <summary>
        /// Cleans up any used resources when the object is destroyed.
        /// </summary>
        ~HubTileBase()
        {
            int index = HubTileService.Tiles.IndexOf(this);
            if (index != -1)
            {
                HubTileService.Tiles.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets or sets the flip state of the control.
        /// </summary>
        public bool IsFlipped
        {
            get { return (bool)this.GetValue(IsFlippedProperty); }
            set { this.SetValue(IsFlippedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a command that will be executed when the hub tile is tapped.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(HubTileBase.CommandProperty);
            }

            set
            {
                this.SetValue(HubTileBase.CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command parameter that will be passed to the command assigned to the Command property.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(HubTileBase.CommandParameterProperty);
            }

            set
            {
                this.SetValue(HubTileBase.CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        /// <remarks>The Title property works similar to the ContentControl.Content property. As it is of type <c>object</c>,
        /// there are no restrictions of its value - string, UI element, Panel or etc.</remarks>
        public object Title
        {
            get
            {
                return this.GetValue(HubTileBase.TitleProperty);
            }

            set
            {
                this.SetValue(HubTileBase.TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate that is used to visualize the <see cref="Title"/> property.
        /// </summary>
        /// <seealso cref="Title"/>
        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TitleTemplateProperty);
            }

            set
            {
                this.SetValue(TitleTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the IsFrozen property. Freezing a hub tile means that it will cease to
        /// periodically update itself. For example when it is off-screen.
        /// </summary>
        public bool IsFrozen
        {
            get
            {
                return (bool)this.GetValue(HubTileBase.IsFrozenProperty);
            }

            set
            {
                this.SetValue(HubTileBase.IsFrozenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the UpdateInterval. This interval determines how often the tile will
        /// update its visual states when it is not frozen. The default value is <c>new TimeSpan(0, 0, 3)</c>
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadHubTile UpdateInterval="0:0:1"/&gt;
        /// </code>
        /// </example>
        public TimeSpan UpdateInterval
        {
            get
            {
                return (TimeSpan)this.GetValue(HubTileBase.UpdateIntervalProperty);
            }

            set
            {
                this.SetValue(HubTileBase.UpdateIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the back content of the tile. When the back content is set,
        /// the tile flips with a swivel animation to its back side and periodically
        /// flips between its back and front states.
        /// </summary>
        public object BackContent
        {
            get
            {
                return this.GetValue(HubTileBase.BackContentProperty);
            }

            set
            {
                this.SetValue(HubTileBase.BackContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate that is used to visualize the BackContent
        /// property.
        /// </summary>
        public DataTemplate BackContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HubTileBase.BackContentTemplateProperty);
            }

            set
            {
                this.SetValue(HubTileBase.BackContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the timer internally used to perform the control's transitions. Exposed for testing purposes.
        /// </summary>
        internal DispatcherTimer UpdateTimer
        {
            get
            {
                return this.updateTimer;
            }
        }

        /// <summary>
        /// Determines whether the update timer can be started - meaning the control is completely loaded in the visual tree and not frozen.
        /// </summary>
        internal bool CanStartTimer
        {
            get
            {
                if (!this.IsTemplateApplied || this.IsFrozen)
                {
                    return false;
                }

                if (!this.IsLoaded && !this.IsLoading)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the LayoutRoot of the control template.
        /// </summary>
        protected internal UIElement LayoutRoot
        {
            get
            {
                return this.layoutRoot;
            }
        }

        /// <summary>
        /// Gets a value that determines whether a rectangle clip is set on the LayoutRoot.
        /// </summary>
        protected virtual bool ShouldClip
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the update timer used to update the tile's VisualState needs to be started.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsUpdateTimerNeeded
        {
            get
            {
                return this.BackContent != null || this.BackContentTemplate != null;
            }
        }

        /// <summary>
        /// Performs the Command execution logic. Exposed for testing purposes.
        /// </summary>
        internal void ExecuteCommand()
        {
            if (this.Command == null)
            {
                return;
            }

            if (!this.Command.CanExecute(this.CommandParameter))
            {
                return;
            }

            this.Command.Execute(this.CommandParameter);
        }

        /// <summary>
        /// Calls the OnUpdateTimerTick method. Exposed for testing purposes.
        /// </summary>
        internal void CallTimerTick()
        {
            this.OnUpdateTimerTick(null, null);
        }

        /// <summary>
        /// Re-evaluates the current visual state for the control and updates it if necessary.
        /// </summary>
        /// <param name="animate">True to use transitions during state update, false otherwise.</param>
        protected internal override void UpdateVisualState(bool animate)
        {
            if (this.flipControl != null)
            {
                this.flipControl.UpdateVisualState(animate);
            }
        }

        /// <summary>
        /// Resolves the ControlTemplate parts.
        /// </summary>
        /// <returns></returns>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.layoutRoot = this.GetTemplatePartField<UIElement>("PART_LayoutRoot");
            applied = applied && this.layoutRoot != null;

            this.flipControl = this.GetTemplatePartField<FlipControl>("PART_FlipControl");
            applied = applied && this.flipControl != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.UpdateTimerState();
        }

        /// <summary>
        /// Called before the <see cref="E:Tapped"/> event occurs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            this.ExecuteCommand();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            this.UpdateTimerState();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Stops the update timer (if currently running).
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.UpdateTimerState();
        }

        /// <summary>
        /// A virtual callback that is called periodically when the tile is not frozen. It can be used to
        /// update the tile visual states or other necessary operations.
        /// </summary>
        protected virtual void Update()
        {
            this.UpdateVisualState(this.IsLoaded);
        }

        /// <summary>
        /// A virtual method that resets the timer.
        /// </summary>
        protected void ResetTimer()
        {
            this.updateTimer.Stop();
            this.UpdateTimerState();
        }

        /// <summary>
        /// A virtual callback that is called when the BackContent property changes.
        /// </summary>
        /// <param name="newBackContent">The new BackContent value.</param>
        /// <param name="oldBackContent">The old BackContent value.</param>
        protected virtual void OnBackContentChanged(object newBackContent, object oldBackContent)
        {
            if (newBackContent == null)
            {
                this.OnBackStateDeactivated();
            }
            else
            {
                this.OnBackStateActivated();
            }
        }

        /// <summary>
        /// This callback is invoked when BackContent is set to a non-null value.
        /// </summary>
        protected virtual void OnBackStateActivated()
        {
            this.ResetTimer();
            this.UpdateVisualState(this.IsLoaded);
        }

        /// <summary>
        /// This callback is invoked when BackContent is set to a null value.
        /// </summary>
        protected virtual void OnBackStateDeactivated()
        {
            this.ResetTimer();
            this.UpdateVisualState(this.IsLoaded);
        }

        /// <summary>
        /// Checks whether the timer should be running or stopped and updates it accordingly.
        /// </summary>
        protected void UpdateTimerState()
        {
            if (!this.CanStartTimer || !this.IsUpdateTimerNeeded)
            {
                this.updateTimer.Stop();
            }
            else
            {
                this.updateTimer.Start();
            }
        }

        private static void OnIsFrozenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase hubTile = (HubTileBase)sender;
            hubTile.UpdateTimerState();
        }

        private static void OnUpdateIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase hubTile = sender as HubTileBase;
            hubTile.updateTimer.Interval = (TimeSpan)args.NewValue;
        }

        private static void OnBackContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase tile = sender as HubTileBase;
            tile.OnBackContentChanged(args.NewValue, args.OldValue);
        }

        private void OnUpdateTimerTick(object sender, object args)
        {
            if (!this.IsLoaded || !this.IsTemplateApplied)
            {
                return;
            }

            this.Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.ShouldClip || !this.IsTemplateApplied)
            {
                return;
            }

            this.layoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };
        }
    }
}
