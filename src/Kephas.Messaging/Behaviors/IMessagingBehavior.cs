// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessagingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application service for message processing interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Behaviors.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Application service for message processing interception.
    /// </summary>
    public interface IMessagingBehavior
    {
        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task BeforeProcessAsync(IMessagingContext context, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        /// <remarks>
        /// The context will contain the response returned by the handler.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        Task AfterProcessAsync(IMessagingContext context, CancellationToken token);
    }

    /// <summary>
    /// Application service for message processing interception.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    [SingletonAppServiceContract(AllowMultiple = true, ContractType = typeof(IMessagingBehavior))]
    public interface IMessagingBehavior<TMessage> : IMessagingBehavior
        where TMessage : IMessage
    {
    }
}