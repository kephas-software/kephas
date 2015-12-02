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

    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    public interface IAmbientServices : IExpando
    {
        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        ICompositionContext CompositionContainer { get; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the application configuration provider.
        /// </summary>
        /// <value>
        /// The application configuration provider.
        /// </value>
        IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>
        /// The logger factory.
        /// </value>
        ILogManager LogManager { get; }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        ILogger GetLogger(string loggerName);

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        ILogger GetLogger(Type type);

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        ILogger<T> GetLogger<T>();
    }
}