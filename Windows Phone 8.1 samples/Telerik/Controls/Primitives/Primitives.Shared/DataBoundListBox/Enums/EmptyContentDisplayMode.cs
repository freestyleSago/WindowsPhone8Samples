namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the two modes for displaying the
    /// empty content in <see cref="RadDataBoundListBox"/>.
    /// </summary>
    public enum EmptyContentDisplayMode
    {
        /// <summary>
        /// Displays the content set to the <see cref="RadDataBoundListBox.EmptyContent"/> property
        /// and defined by the <see cref="RadDataBoundListBox.EmptyContentTemplate"/> property
        /// always, i.e. when there is no source or when the source is empty.
        /// </summary>
        Always,

        /// <summary>
        /// Displays the content set to the <see cref="RadDataBoundListBox.EmptyContent"/> property
        /// and defined by the <see cref="RadDataBoundListBox.EmptyContentTemplate"/> property
        /// only when there are no items in the attached source.
        /// </summary>
        DataSourceEmpty,

        /// <summary>
        /// Displays the content set to the <see cref="RadDataBoundListBox.EmptyContent"/> property
        /// and defined by the <see cref="RadDataBoundListBox.EmptyContentTemplate"/> property
        /// only when there is no data source defined.
        /// </summary>
        DataSourceNull
    }
}
