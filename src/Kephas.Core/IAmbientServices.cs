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
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Resources;

    /// <summary>
    /// Contract interface for ambient services.
    /// </summary>
    [ContractClass(typeof(AmbientServicesContractClass))]
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
        /// Gets the application environment.
        /// </summary>
        /// <value>
        /// The application environment.
        /// </value>
        IAppEnvironment AppEnvironment { get; }

        /// <summary>
        /// Gets the application configuration manager.
        /// </summary>
        /// <value>
        /// The application configuration manager.
        /// </value>
        IConfigurationManager ConfigurationManager { get; }

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
        /// Gets the application environment.
        /// </summary>
        /// <value>
        /// The application environment.
        /// </value>
        public abstract IAppEnvironment AppEnvironment { get; }

        /// <summary>
        /// Gets the application configuration manager.
        /// </summary>
        /// <value>
        /// The application configuration manager.
        /// </value>
        public abstract IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
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
        /// The <see cref="object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public IAmbientServices RegisterService(Type serviceType, object service)
        {
            Contract.Requires(serviceType != null);
            Contract.Requires(service != null);
            Contract.Ensures(Contract.Result<IAmbientServices>() != null);

            if (!serviceType.GetTypeInfo().IsAssignableFrom(service.GetType().GetTypeInfo()))
            {
                throw new InvalidOperationException(string.Format(Strings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception, service.GetType(), serviceType));
            }

            return Contract.Result<IAmbientServices>();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param>
        public abstract object GetService(Type serviceType);
    }
}