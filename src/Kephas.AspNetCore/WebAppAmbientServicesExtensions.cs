// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAppAmbientServicesExtensions.cs" company="Kephas Software SRL">
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
    public static class WebAppAmbientServicesExtensions
    {
        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>An enumeration of application assemblies.</returns>
        public static IEnumerable<Assembly> GetAppAssemblies(this IAmbientServices ambientServices)
            => ambientServices!.AppRuntime.GetAppAssemblies();

        /// <summary>
        /// Configures the <see cref="ApplicationPartManager"/> of the <see cref="IMvcBuilder.PartManager"/> using
        /// the given <see cref="IAmbientServices"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="ambientServices">The <see cref="IAmbientServices"/>.</param>
        /// <returns>The provided <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder ConfigureApplicationPartManager(
            this IMvcBuilder builder,
            IAmbientServices ambientServices)
        {
            var appAssemblies = ambientServices.GetAppAssemblies();
            var assemblyParts = appAssemblies.Select(asm => (ApplicationPart)new AssemblyPart(asm));

            return builder
                .ConfigureApplicationPartManager(m => m.ApplicationParts.AddRange(assemblyParts));
        }
    }
}