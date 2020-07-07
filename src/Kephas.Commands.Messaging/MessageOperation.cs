// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Operation class based on a message.
    /// </summary>
    public class MessageOperation : IOperation
#if NETSTANDARD2_1
#else
        , IAsyncOperation
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOperation"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lazyMessageProcessor">The lazy message processor.</param>
        protected internal MessageOperation(object message, Lazy<IMessageProcessor> lazyMessageProcessor)
        {
            this.Message = message;
            this.LazyMessageProcessor = lazyMessageProcessor;
        }

        /// <summary>
        /// Gets the message to process.
        /// </summary>
        protected object Message { get; }

        /// <summary>
        /// Gets the lazy message processor.
        /// </summary>
        protected Lazy<IMessageProcessor> LazyMessageProcessor { get; }

        /// <summary>
        /// Executes the operation in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public virtual object? Execute(IContext? context = null)
        {
            var result = this.LazyMessageProcessor.Value.ProcessAsync(this.Message, ctx => ctx.Merge(context))
                .GetResultNonLocking();
            return result?.GetContent();
        }

        /// <summary>
        /// Executes the operation asynchronously in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public virtual async Task<object?> ExecuteAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            var result = await this.LazyMessageProcessor.Value.ProcessAsync(
                    this.Message,
                    ctx => ctx.Merge(context),
                    cancellationToken).PreserveThreadContext();
            return result?.GetContent();
        }
    }
}