// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default processing context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// The default processing context.
    /// </summary>
    public class ProcessingContext : ContextBase, IProcessingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingContext"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="handler">The handler.</param>
        public ProcessingContext(IRequest request, IRequestHandler handler)
        {
            Contract.Requires(request != null);
            Contract.Requires(handler != null);

            this.Request = request;
            this.Handler = handler;
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public IRequestHandler Handler { get; private set; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public IRequest Request { get; private set; }

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
    }
}