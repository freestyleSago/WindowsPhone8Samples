
using Telerik.Core;
using Windows.ApplicationModel.Resources.Core;
namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents the localization manager used for components defined in this assembly.
    /// </summary>
    public class PrimitivesLocalizationManager : LocalizationManager
    {
        /// <summary>
        /// Defines the key for looking-up the message box content string when trial is over for the <see cref="RadTrialApplicationReminder"/> control.
        /// </summary>
        public static readonly string AppTrialEndContentKey = "AppTrialEnd_MessageBox_Content";

        /// <summary>
        /// Defines the key for looking-up the string for skipping reminders checkbox when trial is over for the <see cref="RadTrialApplicationReminder"/> control.
        /// </summary>
        public static readonly string AppTrialEndSkipFurtherRemindersMessageKey = "AppTrialEnd_MessageBox_SkipFurtherRemindersMessage";

        /// <summary>
        /// Defines the key for looking-up the message box title string when trial is over for the <see cref="RadTrialApplicationReminder"/> control.
        /// </summary>
        public static readonly string AppTrialEndTitleKey = "AppTrialEnd_MessageBox_Title";

        /// <summary>
        /// Defines the key for looking-up the message box content string before trial is over for the <see cref="RadTrialApplicationReminder"/> control.
        /// </summary>
        public static readonly string AppTrialReminderContentKey = "AppTrialReminder_MessageBox_Content";

        /// <summary>
        /// Defines the key for looking-up the string for skipping reminders checkbox before trial is over for the <see cref="RadTrialApplicationReminder"/> control.
        /// </summary>
        public static readonly string AppTrialReminderSkipFurtherRemindersMessageKey = "AppTrialReminder_MessageBox_SkipFurtherRemindersMessage";

        /// <summary>
        /// Defines the key for looking-up the message box title string before trial is over for the <see cref="RadTrialApplicationReminder"/> control.
        /// </summary>
        public static readonly string AppTrialReminderTitleKey = "AppTrialReminder_MessageBox_Title";

        /// <summary>
        /// Defines the key for looking-up the busy indicator content string for the <see cref="RadBusyIndicator"/> control.
        /// </summary>
        public static readonly string BusyIndicatorContentKey = "BusyIndicatorContent";

        /// <summary>
        /// Defines the key for looking-up the message box content string for the <see cref="RadDiagnostics"/> control.
        /// </summary>
        public static readonly string DiagnosticsContentKey = "Diagnostics_MessageBox_Content";

        /// <summary>
        /// Defines the key for looking-up the message box title string for the <see cref="RadDiagnostics"/> control.
        /// </summary>
        public static readonly string DiagnosticsTitleKey = "Diagnostics_MessageBox_Title";

        /// <summary>
        /// Defines the key for looking-up the message box content string when trial is over for the <see cref="RadTrialFeatureReminder"/> control.
        /// </summary>
        public static readonly string FeatureTrialEndContentKey = "FeatureTrialEnd_MessageBox_Content";

        /// <summary>
        /// Defines the key for looking-up the string for skipping reminders checkbox when trial is over for the <see cref="RadTrialFeatureReminder"/> control.
        /// </summary>
        public static readonly string FeatureTrialEndSkipFurtherRemindersMessageKey = "FeatureTrialEnd_MessageBox_SkipFurtherRemindersMessage";

        /// <summary>
        /// Defines the key for looking-up the message box title string when trial is over for the <see cref="RadTrialFeatureReminder"/> control.
        /// </summary>
        public static readonly string FeatureTrialEndTitleKey = "FeatureTrialEnd_MessageBox_Title";

        /// <summary>
        /// Defines the key for looking-up the message box content string before trial is over for the <see cref="RadTrialFeatureReminder"/> control.
        /// </summary>
        public static readonly string FeatureTrialReminderContentKey = "FeatureTrialReminder_MessageBox_Content";

        /// <summary>
        /// Defines the key for looking-up the string for skipping reminders checkbox before trial is over for the <see cref="RadTrialFeatureReminder"/> control.
        /// </summary>
        public static readonly string FeatureTrialReminderSkipFurtherRemindersMessageKey = "FeatureTrialReminder_MessageBox_SkipFurtherRemindersMessage";

        /// <summary>
        /// Defines the key for looking-up the message box title string before trial is over for the <see cref="RadTrialFeatureReminder"/> control.
        /// </summary>
        public static readonly string FeatureTrialReminderTitleKey = "FeatureTrialReminder_MessageBox_Title";

        /// <summary>
        /// Defines the key for looking-up the empty content string for the <see cref="RadDataBoundListBox"/> control.
        /// </summary>
        public static readonly string DataBoundListBoxEmptyContentKey = "ListBoxEmptyContent";

        /// <summary>
        /// Defines the key for looking-up the list controls' pull-to-refresh time indicator string.
        /// </summary>
        public static readonly string ListPullToRefreshTimeStringKey = "ListPullToRefreshTime";

        /// <summary>
        /// Defines the key for looking-up the list controls' pull-to-refresh loading string.
        /// </summary>
        public static readonly string ListPullToRefreshLoadingStringKey = "ListPullToRefreshLoading";

        /// <summary>
        /// Defines the key for looking-up the list controls' pull-to-refresh normal state string.
        /// </summary>
        public static readonly string ListPullToRefreshStringKey = "ListPullToRefresh";
     
        /// <summary>
        /// Defines the key for looking-up the cancel button content string for the <see cref="RadMessageBox"/> control.
        /// </summary>
        public static readonly string MessageBoxCancelKey = "MBCancelText";

        /// <summary>
        /// Defines the key for looking-up the no button content string for the <see cref="RadMessageBox"/> control.
        /// </summary>
        public static readonly string MessageBoxNoKey = "MBNoText";

        /// <summary>
        /// Defines the key for looking-up the ok button content string for the <see cref="RadMessageBox"/> control.
        /// </summary>
        public static readonly string MessageBoxOkKey = "MBOkText";

        /// <summary>
        /// Defines the key for looking-up the yes button content string for the <see cref="RadMessageBox"/> control.
        /// </summary>
        public static readonly string MessageBoxYesKey = "MBYesText";

        /// <summary>
        /// Defines the key for looking-up the Off string for the <see cref="RadToggleSwitch"/> control.
        /// </summary>
        public static readonly string OffKey = "Off";

        /// <summary>
        /// Defines the key for looking-up the On string for the <see cref="RadToggleSwitch"/> control.
        /// </summary>
        public static readonly string OnKey = "On";

        /// <summary>
        /// Defines the key for looking-up the password revealed text for the <see cref="RadPasswordBox"/> controls.
        /// </summary>
        public static readonly string PasswordBoxShowPasswordStringKey = "PasswordBoxShowPasswordString";

        /// <summary>
        /// Defines the key for looking-up the message box content string for the <see cref="RadRateApplicationReminder"/> control.
        /// </summary>
        public static readonly string RateReminderContentKey = "RateReminder_MessageBox_Content";

        /// <summary>
        /// Defines the key for looking-up the string for skipping reminders checkbox for the <see cref="RadRateApplicationReminder"/> control.
        /// </summary>
        public static readonly string RateReminderSkipFurtherRemindersMessageKey = "RateReminder_MessageBox_SkipFurtherRemindersMessage";

        /// <summary>
        /// Defines the key for looking-up the message box title string for the <see cref="RadRateApplicationReminder"/> control.
        /// </summary>
        public static readonly string RateReminderTitleKey = "RateReminder_MessageBox_Title";

        /// <summary>
        /// Defines the key for looking-up the string for the remaining period before the trial ends in Trial Reminders.
        /// </summary>
        public static readonly string TrialReminderPeriodMessageKey = "TrialReminderPeriodMessage";

        /// <summary>
        /// Defines the key for looking-up the string for the remaining usages before the trial ends in Trial Reminders.
        /// </summary>
        public static readonly string TrialReminderUsageCountMessageKey = "TrialReminderUsageCountMessage";

        private static readonly PrimitivesLocalizationManager InstanceField = new PrimitivesLocalizationManager();

        private PrimitivesLocalizationManager()
            : base()
        {
            //TODO:LOCALIZATION
            //this.DefaultResourceManager = Windows.UI.Xaml.Resources.ResourceManager;
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Primitives/Neutral");
             
        }

        /// <summary>
        /// Gets the only instance of the <see cref="PrimitivesLocalizationManager"/> class.
        /// </summary>
        /// <value>The instance.</value>
        public static PrimitivesLocalizationManager Instance
        {
            get
            {
                return InstanceField;
            }
        }

        /// <summary>
        /// Gets the app trial end content string for <see cref="RadTrialApplicationReminder"/>.
        /// </summary>
        public string AppTrialEndContentString
        {
            get
            {
                return this.GetString(AppTrialEndContentKey);
            }
        }

        /// <summary>
        /// Gets the app trial end skip further reminders message string for <see cref="RadTrialApplicationReminder"/>.
        /// </summary>
        public string AppTrialEndSkipFurtherRemindersMessageString
        {
            get
            {
                return this.GetString(AppTrialEndSkipFurtherRemindersMessageKey);
            }
        }

        /// <summary>
        /// Gets the app trial end title string for <see cref="RadTrialApplicationReminder"/>.
        /// </summary>
        public string AppTrialEndTitleString
        {
            get
            {
                return this.GetString(AppTrialEndTitleKey);
            }
        }

        /// <summary>
        /// Gets the app trial reminder message box content string for <see cref="RadTrialApplicationReminder"/>.
        /// </summary>
        public string AppTrialReminderContentString
        {
            get
            {
                return this.GetString(AppTrialReminderContentKey);
            }
        }

        /// <summary>
        /// Gets the app trial reminder skip further reminders message string for <see cref="RadTrialApplicationReminder"/>.
        /// </summary>
        public string AppTrialReminderSkipFurtherRemindersMessageString
        {
            get
            {
                return this.GetString(AppTrialReminderSkipFurtherRemindersMessageKey);
            }
        }

        /// <summary>
        /// Gets the app trial reminder title string for <see cref="RadTrialApplicationReminder"/>.
        /// </summary>
        public string AppTrialReminderTitleString
        {
            get
            {
                return this.GetString(AppTrialReminderTitleKey);
            }
        }

        /// <summary>
        /// Gets the content string displayed next to the <see cref="RadBusyIndicator"/> animation.
        /// </summary>
        public string BusyIndicatorContentString
        {
            get
            {
                return this.GetString(BusyIndicatorContentKey);
            }
        }

        /// <summary>
        /// Gets the diagnostics content string for <see cref="RadDiagnostics"/>.
        /// </summary>
        public string DiagnosticsContentString
        {
            get
            {
                return this.GetString(DiagnosticsContentKey);
            }
        }

        /// <summary>
        /// Gets the diagnostics title string for <see cref="RadDiagnostics"/>.
        /// </summary>
        public string DiagnosticsTitleString
        {
            get
            {
                return this.GetString(DiagnosticsTitleKey);
            }
        }

        /// <summary>
        /// Gets the feature trial end content string for <see cref="RadTrialFeatureReminder"/>.
        /// </summary>
        public string FeatureTrialEndContentString
        {
            get
            {
                return this.GetString(FeatureTrialEndContentKey);
            }
        }

        /// <summary>
        /// Gets the feature trial end skip further reminders message string for <see cref="RadTrialFeatureReminder"/>.
        /// </summary>
        public string FeatureTrialEndSkipFurtherRemindersMessageString
        {
            get
            {
                return this.GetString(FeatureTrialEndSkipFurtherRemindersMessageKey);
            }
        }

        /// <summary>
        /// Gets the feature trial end title string for <see cref="RadTrialFeatureReminder"/>.
        /// </summary>
        public string FeatureTrialEndTitleString
        {
            get
            {
                return this.GetString(FeatureTrialEndTitleKey);
            }
        }

        /// <summary>
        /// Gets the feature trial reminder content string for <see cref="RadTrialFeatureReminder"/>.
        /// </summary>
        public string FeatureTrialReminderContentString
        {
            get
            {
                return this.GetString(FeatureTrialReminderContentKey);
            }
        }

        /// <summary>
        /// Gets the feature trial reminder skip further reminders message string for <see cref="RadTrialFeatureReminder"/>.
        /// </summary>
        public string FeatureTrialReminderSkipFurtherRemindersMessageString
        {
            get
            {
                return this.GetString(FeatureTrialReminderSkipFurtherRemindersMessageKey);
            }
        }

        /// <summary>
        /// Gets the feature trial reminder title string for <see cref="RadTrialFeatureReminder"/>.
        /// </summary>
        public string FeatureTrialReminderTitleString
        {
            get
            {
                return this.GetString(FeatureTrialReminderTitleKey);
            }
        }

        /// <summary>
        /// Gets the default empty content for the <see cref="RadDataBoundListBox"/> control.
        /// </summary>
        public string DataBoundListBoxEmptyContentString
        {
            get
            {
                return this.GetString(DataBoundListBoxEmptyContentKey);
            }
        }

        /// <summary>
        /// Gets the string displayed in a list control's pull-to-refresh time indicator when in refreshing state.
        /// </summary>
        public string ListPullToRefreshTimeString
        {
            get
            {
                return this.GetString(ListPullToRefreshTimeStringKey);
            }
        }

        /// <summary>
        /// Gets the string displayed in a list control's pull-to-refresh time indicator when in refreshing state.
        /// </summary>
        public string ListPullToRefreshLoadingString
        {
            get
            {
                return this.GetString(ListPullToRefreshLoadingStringKey);
            }
        }

        /// <summary>
        /// Gets the string displayed in a list control's pull-to-refresh time indicator when in normal state.
        /// </summary>
        public string ListPullToRefreshString
        {
            get
            {
                return this.GetString(ListPullToRefreshStringKey);
            }
        }


        /// <summary>
        /// Gets the message box cancel button string for <see cref="RadMessageBox"/>.
        /// </summary>
        public string MessageBoxCancelString
        {
            get
            {
                return this.GetString(MessageBoxCancelKey);
            }
        }

        /// <summary>
        /// Gets the message box no button string for <see cref="RadMessageBox"/>.
        /// </summary>
        public string MessageBoxNoString
        {
            get
            {
                return this.GetString(MessageBoxNoKey);
            }
        }

        /// <summary>
        /// Gets the message box ok button string for <see cref="RadMessageBox"/>.
        /// </summary>
        public string MessageBoxOkString
        {
            get
            {
                return this.GetString(MessageBoxOkKey);
            }
        }

        /// <summary>
        /// Gets the message box yes button string for <see cref="RadMessageBox"/>.
        /// </summary>
        public string MessageBoxYesString
        {
            get
            {
                return this.GetString(MessageBoxYesKey);
            }
        }

        /// <summary>
        /// Gets the toggle switch off string for <see cref="RadToggleSwitch"/>.
        /// </summary>
        public string ToggleSwitchOffString
        {
            get
            {
                return this.GetString(OffKey);
            }
        }

        /// <summary>
        /// Gets the toggle switch on string for <see cref="RadToggleSwitch"/>.
        /// </summary>
        public string ToggleSwitchOnString
        {
            get
            {
                return this.GetString(OnKey);
            }
        }

        /// <summary>
        /// Gets the string displayed in RadPasswordBox's check box for revealing the password.
        /// </summary>
        public string PasswordBoxShowPasswordString
        {
            get
            {
                return this.GetString(PasswordBoxShowPasswordStringKey);
            }
        }

        /// <summary>
        /// Gets the rate reminder content string for <see cref="RadRateApplicationReminder"/>.
        /// </summary>
        public string RateReminderContentString
        {
            get
            {
                return this.GetString(RateReminderContentKey);
            }
        }

        /// <summary>
        /// Gets the rate reminder skip further reminders message string for <see cref="RadRateApplicationReminder"/>.
        /// </summary>
        public string RateReminderSkipFurtherRemindersMessageString
        {
            get
            {
                return this.GetString(RateReminderSkipFurtherRemindersMessageKey);
            }
        }

        /// <summary>
        /// Gets the rate reminder title string for <see cref="RadRateApplicationReminder"/>.
        /// </summary>
        public string RateReminderTitleString
        {
            get
            {
                return this.GetString(RateReminderTitleKey);
            }
        }

        /// <summary>
        /// Gets the trial reminder period message string for Trial Reminders.
        /// </summary>
        public string TrialReminderPeriodMessageString
        {
            get
            {
                return this.GetString(TrialReminderPeriodMessageKey);
            }
        }

        /// <summary>
        /// Gets the trial reminder usage count message string for Trial Reminders.
        /// </summary>
        public string TrialReminderUsageCountMessageString
        {
            get
            {
                return this.GetString(TrialReminderUsageCountMessageKey);
            }
        }
    }
}
