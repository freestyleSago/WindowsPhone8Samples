﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.DataBoundListBox
{
    /// <summary>
    /// Represents the visual element that is displayed on top of the scrollable content in <see cref="RadDataBoundListBox"/>
    /// that indicates the user to pull down to refresh the content.
    /// </summary>
    public class PullToRefreshIndicatorControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="RefreshTimeLabelFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RefreshTimeLabelFormatProperty =
            DependencyProperty.Register("RefreshTimeLabelFormat", typeof(string), typeof(PullToRefreshIndicatorControl), new PropertyMetadata(null, OnRefreshTimeLabelFormatChanged));

		/// <summary>
		/// Identifies the <see cref="Orientation"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(PullToRefreshIndicatorControl), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        internal ContentPresenter indicator;
        internal TextBlock refreshLabel;
        internal TextBlock refreshTime;
        internal RadBusyIndicator busyIndicator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PullToRefreshIndicatorControl" /> class.
        /// </summary>
        public PullToRefreshIndicatorControl()
        {
            this.DefaultStyleKey = typeof(PullToRefreshIndicatorControl);
        }

        /// <summary>
        /// Gets or sets a string representing the format of the time displayed
        /// in the lower label of the <see cref="PullToRefreshIndicatorControl"/>.
        /// </summary>
        public string RefreshTimeLabelFormat
        {
            get
            {
                return this.GetValue(RefreshTimeLabelFormatProperty) as string;
            }
            set
            {
                this.SetValue(RefreshTimeLabelFormatProperty, value);
            }
        }

		/// <summary>
		/// Gets or sets the current orientation of the control.
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				return (Orientation)this.GetValue(OrientationProperty);
			}
			set
			{
				this.SetValue(OrientationProperty, value);
			}
		}


        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.indicator = this.GetTemplateChild("PART_Indicator") as ContentPresenter;
            applied = applied && this.indicator != null;

            this.busyIndicator = this.GetTemplateChild("BusyIndicator") as RadBusyIndicator;
            applied = applied && this.busyIndicator != null;

            this.refreshLabel = this.GetTemplateChild("PART_RefreshInfoLabel") as TextBlock;
            applied = applied && this.refreshLabel != null;

            this.refreshTime = this.GetTemplateChild("PART_RefreshTimeLabel") as TextBlock;
            applied = applied && this.refreshTime != null;

            return applied;
        }

        //TODO:Is this correct?
        /// <summary>
        /// Gets a boolen value that indicates whether the control template parts
        /// have been successfully acquired after the OnApplyTemplate call.
        /// </summary>
        /// <value></value>
        //protected override bool IsProperlyTemplated
        //{
        //    get
        //    {
        //        return this.indicator != null && this.refreshLabel != null && this.refreshTime != null;
        //    }
        //}

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate" />. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            this.refreshLabel.Text = PrimitivesLocalizationManager.Instance.GetString("ListPullToRefreshString");

            if (this.ReadLocalValue(RefreshTimeLabelFormatProperty) == DependencyProperty.UnsetValue)
            {
                if (this.RefreshTimeLabelFormat == null)
                {
                    this.RefreshTimeLabelFormat = PrimitivesLocalizationManager.Instance.GetString("ListPullToRefreshTimeString");
                }
            }

			if (this.Orientation == Orientation.Horizontal)
			{
				VisualStateManager.GoToState(this, "Horizontal", false);
			}
			else
			{
				VisualStateManager.GoToState(this, "Vertical", false);
			}
        }

        /// <summary>
        /// Updates the lower label that displays the last refresh time.
        /// </summary>
        /// <param name="time">A <see cref="System.DateTime"/> value that represents the last refresh time.</param>
        public void UpdateRefreshTime(DateTime time)
        {
            if (!this.ApplyTemplateCore())
            {
                return;
            }
            this.refreshTime.Text = string.Format(this.RefreshTimeLabelFormat, time);
        }
     
        /// <summary>
        /// Starts the loading progress by displaying a progress bar in the control.
        /// </summary>
        public void StartLoading()
        {
            if (this.ApplyTemplateCore())
            {
                this.refreshLabel.Text = PrimitivesLocalizationManager.Instance.GetString("ListPullToRefreshLoadingString");
            }
            VisualStateManager.GoToState(this, "Refreshing", false);
        }

        public void StartRelase()
        {
            if (this.ApplyTemplateCore())
            {
                this.refreshLabel.Text = PrimitivesLocalizationManager.Instance.GetString("ReleaseToRefreshString");
            }
            VisualStateManager.GoToState(this, "ReleaseToRefresh", true);
        }

        /// <summary>
        /// Stops the loading progress.
        /// </summary>
        public void StopLoading()
        {
            if (this.ApplyTemplateCore())
            {
                this.refreshLabel.Text = PrimitivesLocalizationManager.Instance.GetString("ListPullToRefreshString");
            }
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private static void OnRefreshTimeLabelFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as PullToRefreshIndicatorControl).UpdateRefreshTime(DateTime.Now);
        }

		private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PullToRefreshIndicatorControl indicator = d as PullToRefreshIndicatorControl;
			if (indicator.Orientation == Orientation.Horizontal)
			{
				VisualStateManager.GoToState(indicator, "Horizontal", false);
			}
			else
			{
				VisualStateManager.GoToState(indicator, "Vertical", false); 
			}
		}
    }
}
