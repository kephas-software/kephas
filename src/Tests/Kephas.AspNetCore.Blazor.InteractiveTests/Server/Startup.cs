// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Server
{
    using Kephas.Application.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Startup : StartupAppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        public Startup(IWebHostEnvironment env, IConfiguration config)
            : base(env, config, containerBuilder: ambientServices => ambientServices.BuildWithAutofac())
        {
        }
    }
}
