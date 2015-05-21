using System.ComponentModel;
using System.IO;
using System.Reflection;
using Telerik.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Primitives.License
{
    /// <summary>
    /// Defines the model behind the Trial message notification.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class TrialViewModel
    {
        private BitmapImage logoImage;

        /// <summary>
        /// Gets the offset from the top of the screen.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public double OffsetTop
        {
            get
            {
                return 20;
            }
        }

        /// <summary>
        /// Gets the width of the trial message UI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public double Width
        {
            get
            {
                return 415;
            }
        }

        /// <summary>
        /// Gets the height of the trial message UI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public double Height
        {
            get
            {
                return 110;
            }
        }

        /// <summary>
        /// Gets the title part of the trial message UI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string Title
        {
            get
            {
                return "Telerik UI for Universal Windows Apps Trial";
            }
        }

        /// <summary>
        /// Gets the first line of text within the trial message UI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string MessageLine1
        {
            get
            {
                return "To get access to the latest updates across the";
            }
        }

        /// <summary>
        /// Gets the second line of text within the trial message UI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string MessageLine2
        {
            get
            {
                return "Windows Universal Apps suite and Telerik's dedicated";
            }
        }

        /// <summary>
        /// Gets the third line of text within the trial message UI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string MessageLine3
        {
            get
            {
                return "support please purchase a commercial license now.";
            }
        }

        /// <summary>
        /// Gets the source for the Logo Icon within the trial message UI.
        /// </summary>
        public ImageSource Logo
        {
            get
            {
                if (this.logoImage == null)
                {
                    this.LoadLogoImage();
                }

                return this.logoImage;
            }
        }

        private async void LoadLogoImage()
        {
            Assembly assembly = typeof(TrialViewModel).GetTypeInfo().Assembly;
            string path = "Telerik.UI.Xaml.Controls.Primitives.Logo.png";

            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(await stream.AsRandomAccessStreamAsync());
                this.logoImage = image;
            }
        }
    }
}
