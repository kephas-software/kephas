// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for contexts when processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Contract for contexts when processing messages.
    /// </summary>
    [ContractClass(typeof(MessageProcessingContextContractClass))]
    public interface IMessageProcessingContext : IContext
    {
        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        IMessageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        IMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        IMessage Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; set; }
    }

    /// <summary>
    /// Contract class for <see cref="IMessageProcessingContext"/>.
    /// </summary>
    [ContractClassFor(typeof(IMessageProcessingContext))]
    internal abstract class MessageProcessingContextContractClass : IMessageProcessingContext
    {
        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public IMessageHandler Handler
        {
            get
            {
                Contract.Ensures(Contract.Result<IMessageHandler>() != null);
                return Contract.Result<IMessageHandler>();
            }

            set
            {
                Contract.Requires(value != null);
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IMessage Message
        {
            get
            {
                Contract.Ensures(Contract.Result<IMessage>() != null);
                return Contract.Result<IMessage>();
            }

            set
            {
                Contract.Requires(value != null);
            }
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public IMessage Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

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
        public IIdentity Identity { get; set; }

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
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}