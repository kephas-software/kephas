// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract interface for ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Lite;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    public interface IAmbientServices : IExpando, IServiceProvider, IDisposable
    {
        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        IConfigurationStore ConfigurationStore { get; }

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        ICompositionContext CompositionContainer { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the runtime type serviceRegistry.
        /// </summary>
        IRuntimeTypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the type loader.
        /// </summary>
        /// <value>
        /// The type loader.
        /// </value>
        ITypeLoader TypeLoader { get; }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        ILogManager LogManager { get; }

        /// <summary>
        /// Gets the manager for licensing.
        /// </summary>
        /// <value>
        /// The licensing manager.
        /// </value>
        ILicensingManager LicensingManager { get; }

        /// <summary>
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        IAmbientServices Register(Type serviceType, Action<IServiceRegistrationBuilder> builder);

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        bool IsRegistered(Type serviceType);
    }
}