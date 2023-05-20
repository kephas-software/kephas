// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the default implementation of the <see cref="IMessageProcessor" /> application service contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Logging;
using Kephas.Pipelines;
using Kephas.Services;
using Kephas.Threading.Tasks;

namespace Kephas.Messaging;

/// <summary>
/// Provides the default implementation of the <see cref="IMessageProcessor"/> application service contract.
/// </summary>
[OverridePriority(Priority.Low)]
public class DefaultMessageProcessor : Loggable, IMessageProcessor
{
    private readonly IMessageHandlerRegistry handlerRegistry;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMessageProcessor" /> class.
    /// </summary>
    /// <param name="injectableFactory">The injectable factory.</param>
    /// <param name="handlerRegistry">The handler registry.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public DefaultMessageProcessor(
        IInjectableFactory injectableFactory,
        IMessageHandlerRegistry handlerRegistry,
        IServiceProvider serviceProvider)
        : base(injectableFactory)
    {
        this.InjectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
        this.handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Gets the injectable factory.
    /// </summary>
    /// <value>
    /// The injectable factory.
    /// </value>
    protected IInjectableFactory InjectableFactory { get; }

    /// <summary>
    /// Processes the specified message asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message to process.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <param name="token">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the response message.
    /// </returns>
    public async Task<TResponse> ProcessAsync<TMessage, TResponse>(
        TMessage message,
        Action<IMessagingContext>? optionsConfig = null,
        CancellationToken token = default)
        where TMessage : IMessage<TResponse>
    {
        message = message ?? throw new ArgumentNullException(nameof(message));

        var exceptions = new List<Exception>();
        TResponse result = default!;
        foreach (var messageHandler in this.handlerRegistry.ResolveMessageHandlers<TMessage, TResponse>(message))
        {
            try
            {
                using var context = this.CreateProcessingContext(message, optionsConfig);
                var pipeline = this.serviceProvider.GetRequiredService<IPipeline<IMessageProcessor, TMessage, TResponse>>();
                result = await pipeline.ProcessAsync(this, message, context, () => messageHandler.ProcessAsync(message, context, token), token)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        return exceptions.Count switch
        {
            0 => result,
            1 => throw exceptions[0],
            _ => throw new AggregateException(exceptions)
        };
    }

    /// <summary>
    /// Creates the processing context.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="optionsConfig">The options configuration.</param>
    /// <returns>
    /// The processing context.
    /// </returns>
    protected virtual IMessagingContext CreateProcessingContext(
        IMessageBase message,
        Action<IMessagingContext>? optionsConfig)
    {
        var context = this.InjectableFactory.Create<MessagingContext>(message);
        optionsConfig?.Invoke(context);
        return context;
    }
}