
namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Define some ImageButton types that come out-of-the-box.
    /// </summary>
    public enum ImageButtonType
    {
        /// <summary>
        /// Custom. This means that the images for the button will have to be specified by the relevant properties. This is the default ImageButtonType.
        /// </summary>
        Custom,

        /// <summary>
        /// Search. With this button type an opacity mask which is suitable for a search button will be used.
        /// </summary>
        Search,

        /// <summary>
        /// Clear. With this button type an opacity mask with an X will be used.
        /// </summary>
        Clear,

        /// <summary>
        /// Arrow. With this button type an opacity mask with an arrow will be used.
        /// </summary>
        Arrow,

        /// <summary>
        /// Peek. With this button type an opacity mask with an eye will be used.
        /// </summary>
        Peek
    }
}
