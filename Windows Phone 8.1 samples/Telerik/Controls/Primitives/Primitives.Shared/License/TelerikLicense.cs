using System;
using System.Diagnostics;
using System.Reflection;
using Telerik.UI.Xaml.Controls.Primitives.License;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls
{
    internal static class TelerikLicense
    {
        internal const string PurchaseUrl = "http://www.telerik.com/purchase/individual-windows-8.aspx?utm_source=trial&utm_medium=web&utm_campaign=Windows8";

        private static bool isLicenseShown;

        [Conditional("TRIAL")]
        public static void Verify()
        {
            if (!isLicenseShown && Window.Current != null)
            {
                ShowLicenseMessage();
                isLicenseShown = true;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Windows.UI.Xaml.Controls.TextBlock.put_Text(System.String)", Justification = "The trial message will not be localizable")]
        private async static void ShowLicenseMessage()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }
#if WINDOWS_PHONE_APP
            MessageDialog dialog = new MessageDialog("Telerik UI for Universal Windows Apps Trial");
            await dialog.ShowAsync();
            return;
#else
            ToastNotification notification = new ToastNotification();
            notification.Show();
#endif

        }
    }
}
