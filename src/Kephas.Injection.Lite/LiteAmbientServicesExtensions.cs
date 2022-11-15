// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Builder;

    /// <summary>
    /// Lite injection related extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    public static class LiteAmbientServicesExtensions
    {
        /// <summary>
        /// Builds the injector using Lite and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">Optional. The injector builder configuration.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices BuildWithLite(this IAmbientServices ambientServices, Action<LiteInjectorBuilder>? builderOptions = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var injectorBuilder = new LiteInjectorBuilder(new InjectionBuildContext(ambientServices));

            builderOptions?.Invoke(injectorBuilder);

            var container = injectorBuilder.Build();
            return ambientServices.WithInjector(container);
        }
    }
}