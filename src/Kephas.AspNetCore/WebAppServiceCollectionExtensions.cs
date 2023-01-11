// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Collections;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for web applications.
    /// </summary>
    public static class WebAppServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the <see cref="ApplicationPartManager"/> of the <see cref="IMvcBuilder.PartManager"/> using
        /// the given <see cref="IAppServiceCollection"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="appRuntime">The <see cref="IAppRuntime"/>.</param>
        /// <returns>The provided <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder ConfigureApplicationPartManager(
            this IMvcBuilder builder,
            IAppRuntime appRuntime)
        {
            var appAssemblies = appRuntime.GetAppAssemblies();
            var assemblyParts = appAssemblies.Select(asm => (ApplicationPart)new AssemblyPart(asm));

            return builder
                .ConfigureApplicationPartManager(m => m.ApplicationParts.AddRange(assemblyParts));
        }
    }
}