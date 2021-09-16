// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesSystemCompositionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Hosting;

    /// <summary>
    /// MEF extensions for the ambient services.
    /// </summary>
    public static class AmbientServicesSystemCompositionExtensions
    {
        /// <summary>
        /// Builds the composition container using System.Composition and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="injectorBuilderConfig">The injector builder configuration.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices BuildWithSystemComposition(this IAmbientServices ambientServices, Action<SystemCompositionInjectorBuilder>? injectorBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new SystemCompositionInjectorBuilder(new InjectionRegistrationContext(ambientServices));

            injectorBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.Build();
            return ambientServices.WithInjector(container);
        }
    }
}