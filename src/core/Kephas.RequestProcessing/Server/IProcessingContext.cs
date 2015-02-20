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
        /// Gets the custom values.
        /// </summary>
        /// <value>
        /// The custom values.
        /// </value>
        public abstract dynamic Data { get; }
    }
}