// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAutofacExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection.Hosting;

namespace Kephas
{
    using System;

    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Autofac related ambient services builder extensions.
    /// </summary>
    public static class AmbientServicesAutofacExtensions
    {
        /// <summary>
        /// Builds the composition container with Autofac and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="injectorBuilderConfig">The injector builder configuration.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices BuildWithAutofac(this IAmbientServices ambientServices, Action<AutofacInjectorBuilder>? injectorBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new AutofacInjectorBuilder(new InjectionRegistrationContext(ambientServices));

            injectorBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateInjector();
            return ambientServices.WithInjector(container);
        }
    }
}