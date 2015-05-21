using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{

    //TODO:BACKBUTTON no longer exists?
    //public partial class RadDataBoundListBox
    //{
    //    private bool backKeyPressHooked = false;

    //    internal virtual void OnBackKeyPressed(CancelEventArgs e)
    //    {
    //        if (this.isCheckModeActive)
    //        {
    //            this.IsCheckModeActive = false;
    //            e.Cancel = true;
    //        }
    //    }

    //    partial void HookRootVisualBackKeyPress()
    //    {
    //        if (!this.checkModeDeactivatedOnBackButton)
    //        {
    //            return;
    //        }

    //        if (this.backKeyPressHooked)
    //        {
    //            return;
    //        }

    //        Page page = ElementTreeHelper.FindVisualAncestor<Page>(this);
    //        if (page != null)
    //        {
    //            page.BackKeyPress += this.OnPageBackKeyPress;
    //            this.backKeyPressHooked = true;
    //        }
    //    }

    //    partial void UnhookRootVisualBackKeyPress()
    //    {
    //        Page page = ElementTreeHelper.FindVisualAncestor<Page>(this);
    //        if (page != null)
    //        {
    //            page.BackKeyPress -= this.OnPageBackKeyPress;
    //            this.backKeyPressHooked = false;
    //        }
    //    }

    //    private void OnPageBackKeyPress(object sender, CancelEventArgs e)
    //    {
    //        this.OnBackKeyPressed(e);
    //    }
    //}
}
