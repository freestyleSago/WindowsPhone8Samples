using System;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a custom loader that may be used to look-up strings through code.
    /// </summary>
    public interface IStringResourceLoader
    {
        /// <summary>
        /// Retrieves a localized version of the string associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string key);
    }
}
