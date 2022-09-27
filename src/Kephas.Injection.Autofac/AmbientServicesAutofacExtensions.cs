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

    using Kephas.Injection;
    using Kephas.Injection.Autofac.Builder;
    using Kephas.Injection.Builder;
    using Kephas.Logging;

    /// <summary>
    /// Autofac related ambient services builder extensions.
    /// </summary>
    public static class AmbientServicesAutofacExtensions
    {
        /// <summary>
        /// Builds the injector with Autofac and adds it to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">The injector builder configuration.</param>
        /// <param name="preserveRegistrationOrder">Optional. Indicates whether to preserve the registration order. Relevant for integration with ASP.NET Core.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>The provided ambient services.</returns>
        public static IServiceProvider BuildWithAutofac(this IAmbientServices ambientServices, Action<AutofacInjectorBuilder>? builderOptions = null, bool preserveRegistrationOrder = true, ILogger? logger = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var buildContext = new InjectionBuildContext(ambientServices);
            buildContext.AddAppServices();
            var injectorBuilder = new AutofacInjectorBuilder(buildContext, preserveRegistrationOrder: preserveRegistrationOrder);

            builderOptions?.Invoke(injectorBuilder);

            return injectorBuilder.Build();
        }
    }
}