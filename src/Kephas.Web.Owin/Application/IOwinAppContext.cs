// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOwinAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IOwinAppContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Owin.Application
{
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Security.Principal;

    using global::Owin;

    using Kephas.Application;

    /// <summary>
    /// Contract for the OWIN application context.
    /// </summary>
    [ContractClass(typeof(OwinAppContextContractClass))]
    public interface IOwinAppContext : IAppContext
    {
        /// <summary>
        /// Gets the OWIN application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        IAppBuilder AppBuilder { get; }
    }

    /// <summary>
    /// An owin application context contract class.
    /// </summary>
    [ContractClassFor(typeof(IOwinAppContext))]
    internal abstract class OwinAppContextContractClass : IOwinAppContext
    {
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
        /// Gets the OWIN application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        public IAppBuilder AppBuilder
        {
            get
            {
                Contract.Ensures(Contract.Result<IAppBuilder>() != null);
                return Contract.Result<IAppBuilder>();
            }
        }

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

        /// <summary>Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.</summary>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}