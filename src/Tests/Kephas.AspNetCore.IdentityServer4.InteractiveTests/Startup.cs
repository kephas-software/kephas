// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests
{
    using Kephas;
    using Kephas.Application.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Startup : StartupAppBase
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
            : base(env, configuration)
        {
        }
    }
}
