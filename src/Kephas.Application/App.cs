// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="AppBase"/>.
    /// </summary>
    public class App : AppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="containerBuilder">Optional. The container builder.</param>
        /// <param name="ambientServices">Optional. The ambient services. If not provided then
        ///                               a new instance of <see cref="Kephas.AmbientServices"/> will be created and used.</param>
        public App(Action<IAmbientServices>? containerBuilder = null, IAmbientServices? ambientServices = null)
            : base(ambientServices, containerBuilder: containerBuilder)
        {
        }
    }
}