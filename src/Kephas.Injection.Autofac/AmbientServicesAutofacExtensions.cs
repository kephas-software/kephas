// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAutofacExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Autofac.Hosting;
    using Kephas.Injection.Hosting;

    /// <summary>
    /// Autofac related ambient services builder extensions.
    /// </summary>
    public static class AmbientServicesAutofacExtensions
    {
        /// <summary>
        /// Builds the injector with Autofac and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="injectorBuilderConfig">The injector builder configuration.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices BuildWithAutofac(this IAmbientServices ambientServices, Action<AutofacInjectorBuilder>? injectorBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var injectorBuilder = new AutofacInjectorBuilder(new InjectionRegistrationContext(ambientServices));

            injectorBuilderConfig?.Invoke(injectorBuilder);

            var container = injectorBuilder.Build();
            return ambientServices.WithInjector(container);
        }
    }
}