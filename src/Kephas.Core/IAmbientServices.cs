// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientServices.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    public interface IAmbientServices : IExpando, IServiceProvider
    {
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
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        IAppConfiguration AppConfiguration { get; }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        ILogManager LogManager { get; }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        IAmbientServices RegisterService(Type serviceType, object service);

        /// <summary>
        /// Registers the provided service factory.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        IAmbientServices RegisterService(Type serviceType, Func<object> serviceFactory);

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