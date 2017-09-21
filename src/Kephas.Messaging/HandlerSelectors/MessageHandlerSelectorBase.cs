// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerSelectorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message handler selector base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// Base class for message handler selectors.
    /// </summary>
    public abstract class MessageHandlerSelectorBase : IMessageHandlerSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerSelectorBase"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        protected MessageHandlerSelectorBase(ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            this.CompositionContext = compositionContext;
        }

        /// <summary>
        /// Gets the context for the composition.
        /// </summary>
        public ICompositionContext CompositionContext { get; }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public abstract bool CanHandle(Type messageType);

        /// <summary>
        /// Gets a factory which retrieves the components handling messages of the given type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// A factory of an enumeration of message handlers.
        /// </returns>
        public virtual Func<IEnumerable<IMessageHandler>> GetHandlersFactory(Type messageType)
        {
            var orderedHandlers = this.GetOrderedMessageHandlerFactories(messageType);
            return () => orderedHandlers.Select(f => f.CreateExportedValue()).ToList();
        }

        /// <summary>
        /// Gets the ordered message handler factories.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// The ordered message handler factories.
        /// </returns>
        protected virtual IList<IExportFactory<IMessageHandler, AppServiceMetadata>> GetOrderedMessageHandlerFactories(Type messageType)
        {
            var messageHandlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);
            var untypedExportFactories = this.CompositionContext.GetExportFactories(messageHandlerType, typeof(AppServiceMetadata));
            var exportFactories = (IEnumerable<IExportFactory<IMessageHandler, AppServiceMetadata>>)untypedExportFactories;
            var orderedFactories = exportFactories
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ToList();
            return orderedFactories;
        }
    }
}