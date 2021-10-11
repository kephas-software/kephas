// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Server
{
    using Kephas.Application.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class StartupApp : StartupAppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupApp" /> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        public StartupApp(IWebHostEnvironment env, IConfiguration config)
            : base(env, config, containerBuilder: ambientServices => ambientServices.BuildWithAutofac())
        {
        }
    }
}
