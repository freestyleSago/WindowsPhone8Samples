﻿// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="IApplicationSettings.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.Models.Settings
{
    using System.ComponentModel;
    using Microsoft.Devices;

    /// <summary>
    /// Defines global application settings.
    /// </summary>
    public interface IApplicationSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the current camera type.
        /// </summary>
        CameraType CameraType { get; set; }
    }
}
