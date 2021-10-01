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
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Injection.Lite;
    using Kephas.Injection.Lite.Builder;
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
        public IConfigurationStore ConfigurationStore => this.GetService<IConfigurationStore>();

        /// <summary>
        /// Gets the injector.
        /// </summary>
        /// <value>
        /// The injector.
        /// </value>
        public IInjector Injector => this.GetService<IInjector>();

        /// <summary>
        /// Gets the type serviceRegistry.
        /// </summary>
        public IRuntimeTypeRegistry TypeRegistry => this.GetService<IRuntimeTypeRegistry>();

        /// <summary>
        /// Gets the type loader.
        /// </summary>
        /// <value>
        /// The type loader.
        /// </value>
        public ITypeLoader TypeLoader => this.GetService<ITypeLoader>();

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime => this.GetService<IAppRuntime>();

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager => this.GetService<ILogManager>();

        /// <summary>
        /// Gets the manager for licensing.
        /// </summary>
        /// <value>
        /// The licensing manager.
        /// </value>
        public ILicensingManager LicensingManager => this.GetService<ILicensingManager>();

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