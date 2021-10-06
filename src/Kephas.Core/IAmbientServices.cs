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
    using Kephas.Injection.Lite.Builder;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    public interface IAmbientServices : IExpando, IServiceProvider, IDisposable
    {
        /// <summary>
        /// Gets the service registry.
        /// </summary>
        protected internal IAppServiceRegistry ServiceRegistry { get; }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public IConfigurationStore ConfigurationStore => this.GetRequiredService<IConfigurationStore>();

        /// <summary>
        /// Gets the injector.
        /// </summary>
        /// <value>
        /// The injector.
        /// </value>
        public IInjector Injector => this.GetRequiredService<IInjector>();

        /// <summary>
        /// Gets the type serviceRegistry.
        /// </summary>
        public IRuntimeTypeRegistry TypeRegistry => this.GetRequiredService<IRuntimeTypeRegistry>();

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime => this.GetRequiredService<IAppRuntime>();

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager => this.GetRequiredService<ILogManager>();

        /// <summary>
        /// Gets the manager for licensing.
        /// </summary>
        /// <value>
        /// The licensing manager.
        /// </value>
        public ILicensingManager LicensingManager => this.GetRequiredService<ILicensingManager>();

        /// <summary>
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        public IAmbientServices Register(Type contractDeclarationType, Action<IServiceRegistrationBuilder> builder)
        {
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            var serviceBuilder = new ServiceRegistrationBuilder(this.ServiceRegistry, contractDeclarationType);
            builder?.Invoke(serviceBuilder);
            this.ServiceRegistry.RegisterSource(serviceBuilder.Build());

            return this;
        }

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool IsRegistered(Type contractType) =>
            this.ServiceRegistry.IsRegistered(contractType);

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="contractType"/>.-or- null if there is no service object of type <paramref name="contractType"/>.
        /// </returns>
        /// <param name="contractType">The contract type of service to get. </param>
        object? IServiceProvider.GetService(Type contractType) =>
            this.ServiceRegistry.GetService(contractType);
    }
}