// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a base contract for context-dependent operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Security.Principal;

    using Kephas.Dynamic;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    [ContractClass(typeof(ContextContractClass))]
    public interface IContext : IExpando
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity Identity { get; set; }
    }

    /// <summary>
    /// Contract class for <see cref="IContext"/>.
    /// </summary>
    [ContractClassFor(typeof(IContext))]
    internal abstract class ContextContractClass : IContext
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices
        {
            get
            {
                Contract.Ensures(Contract.Result<IAmbientServices>() != null);

                return Contract.Result<IAmbientServices>();
            }
        }

        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Gets meta object.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The meta object.
        /// </returns>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}