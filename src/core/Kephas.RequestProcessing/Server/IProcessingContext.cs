// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for contexts when processing requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Contract for contexts when processing requests.
    /// </summary>
    public interface IProcessingContext : IContext
    {
        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        IRequestHandler Handler { get; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        IRequest Request { get; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        IResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; set; }
    }

    /// <summary>
    /// Contract class for <see cref="IProcessingContext"/>.
    /// </summary>
    [ContractClassFor(typeof(IProcessingContext))]
    internal abstract class ProcessingContextContractClass : IProcessingContext
    {
        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public IRequestHandler Handler
        {
            get
            {
                Contract.Requires(Contract.Result<IRequestHandler>() != null);
                return Contract.Result<IRequestHandler>();
            }
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public IRequest Request
        {
            get
            {
                Contract.Requires(Contract.Result<IRequest>() != null);
                return Contract.Result<IRequest>();
            }
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public IResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        public IIdentity AuthenticatedUser { get; set; }

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