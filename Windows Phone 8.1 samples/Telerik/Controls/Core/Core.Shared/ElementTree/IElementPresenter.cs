using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Core
{
    /// <summary>
    /// Represents an instance that may visualize a <see cref="Element"/> instance on the screen. Typically this interface is implemented by platform-specific types like the XAML Control class.
    /// </summary>
    public interface IElementPresenter
    {
        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        bool IsVisible
        {
            get;
        }

        /// <summary>
        /// Invalidates the visual representation of the specified logical node.
        /// </summary>
        /// <param name="node"></param>
        void RefreshNode(object node);

        /// <summary>
        /// Retrieves the desired size of the specified logical node's content.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="content"></param>
        RadSize MeasureContent(object owner, object content);
    }
}
