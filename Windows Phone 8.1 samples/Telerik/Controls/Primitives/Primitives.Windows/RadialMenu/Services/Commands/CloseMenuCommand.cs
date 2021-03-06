﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu.Commands
{
    internal class CloseMenuCommand : RadialMenuCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            this.Owner.IsOpen = false;
        }
    }
}
