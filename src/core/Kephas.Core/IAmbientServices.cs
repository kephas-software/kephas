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
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;

    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    [ContractClass(typeof(AmbientServicesContractClass))]
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

    /// <summary>
    /// Contract class for <see cref="IAmbientServices"/>.
    /// </summary>
    [ContractClassFor(typeof(IAmbientServices))]
    internal abstract class AmbientServicesContractClass : IAmbientServices
    {
        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public abstract ICompositionContext CompositionContainer { get; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public abstract IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the application configuration provider.
        /// </summary>
        /// <value>
        /// The application configuration provider.
        /// </value>
        public abstract IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>
        /// The logger factory.
        /// </value>
        public abstract ILogManager LogManager { get; }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        public ILogger GetLogger(string loggerName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(loggerName));

            return Contract.Result<ILogger>();
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public ILogger GetLogger(Type type)
        {
            Contract.Requires(type != null);

            return Contract.Result<ILogger>();
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public abstract ILogger<T> GetLogger<T>();

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}