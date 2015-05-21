using Windows.UI.Xaml.Resources;

namespace Telerik.UI.Xaml.Controls
{
    /// <summary>
    /// Represents a custom implementation of the <see cref="CustomXamlResourceLoader" /> class that allows users to replace the built-in theme resources via the <see cref="UserThemeResources" /> class.
    /// </summary>
    public sealed class UserThemeResourceLoader : CustomXamlResourceLoader
    {
        internal UserThemeResourceLoader()
        {
        }

        /// <summary>
        /// Retrieves the paths to the Dark or Light user resource dictionaries, containing replacements of the built-in theme resources.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="objectType"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            return UserThemeResources.GetUriByPath(resourceId);
        }
    }
}
