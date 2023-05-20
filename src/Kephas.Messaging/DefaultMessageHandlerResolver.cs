// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Pipelines;
using Kephas.Services;

namespace Kephas.Messaging;

/// <summary>
/// The default implementation of the <see cref="IMessageHandlerResolver"/> service.
/// </summary>
[OverridePriority(Priority.Low)]
public class DefaultMessageHandlerResolver : IMessageHandlerResolver
{
    private readonly IFactoryEnumerable<IMessageHandler, MessageHandlerMetadata> handlerFactories;
    private readonly IMessageHandlerRegistry handlerRegistry;
    private readonly IExportFactory<IPipeline<IMessageHandlerResolver, IMessagingContext, IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>>> resolvePipelineFactory;
    private readonly IMessageMatchService messageMatchService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMessageHandlerResolver"/> class.
    /// </summary>
    public DefaultMessageHandlerResolver(
        IFactoryEnumerable<IMessageHandler, MessageHandlerMetadata> handlerFactories,
        IMessageHandlerRegistry handlerRegistry,
        IExportFactory<IPipeline<IMessageHandlerResolver, IMessagingContext, IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>>> resolvePipelineFactory,
        IMessageMatchService messageMatchService)
    {
        this.handlerFactories = handlerFactories ?? throw new ArgumentNullException(nameof(handlerFactories));
        this.handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        this.resolvePipelineFactory = resolvePipelineFactory ?? throw new ArgumentNullException(nameof(resolvePipelineFactory));
        this.messageMatchService = messageMatchService ?? throw new ArgumentNullException(nameof(messageMatchService));
    }
        
    /// <summary>
    /// Resolves the message handlers for the provided messaging context.
    /// </summary>
    /// <param name="context">The messaging context.</param>
    /// <returns>The matching message handlers.</returns>
    public IEnumerable<IMessageHandler<TMessage, TResult>> Resolve<TMessage, TResult>(IMessagingContext context) where TMessage : IMessage<TResult>
    {
        var pipeline = resolvePipelineFactory.CreateExportedValue();
        return pipeline
            .Process(this, context, null, () => this.GetMessageHandlers(context))
            .OfType<IMessageHandler<TMessage, TResult>>();
    }

    private IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>> GetMessageHandlers(IMessagingContext context)
    {
        return this.handlerFactories
            .Union(this.handlerRegistry)
            .Where(f => messageMatchService.IsMatch(f.Metadata.MessageMatch, context))
            .Order();
    }
}