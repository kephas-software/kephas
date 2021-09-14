// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesMefExtensions.cs" company="Kephas Software SRL">
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

    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// MEF extensions for the ambient services.
    /// </summary>
    public static class AmbientServicesMefExtensions
    {
        /// <summary>
        /// Builds the composition container using System.Composition and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices BuildWithSystemComposition(this IAmbientServices ambientServices, Action<SystemInjectorBuilder>? containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new SystemInjectorBuilder(new CompositionRegistrationContext(ambientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateInjector();
            return ambientServices.WithCompositionContainer(container);
        }
    }
}