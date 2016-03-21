// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBehaviorContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IServiceBehaviorContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Security.Principal;

    /// <summary>
    /// Interface for service behavior context.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    [ContractClass(typeof(ServiceBehaviorContextContractClass<>))]
    public interface IServiceBehaviorContext<out TServiceContract> : IContext
    {
         /// <summary>
         /// Gets the service.
         /// </summary>
         /// <value>
         /// The service.
         /// </value>
         TServiceContract Service { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IServiceBehaviorContext{TServiceContract}"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    [ContractClassFor(typeof(IServiceBehaviorContext<>))]
    internal abstract class ServiceBehaviorContextContractClass<TServiceContract> : IServiceBehaviorContext<TServiceContract>
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public TServiceContract Service
        {
            get
            {
                Contract.Ensures(Contract.Result<TServiceContract>() != null);
                return Contract.Result<TServiceContract>();
            }
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public abstract IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        public abstract IIdentity Identity { get; set; }

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
        /// The <see cref="object" /> identified by the key.
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
    }
}