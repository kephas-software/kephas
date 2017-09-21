// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerSelector.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default message handler selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;

    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A default message handler selector.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class DefaultMessageHandlerSelector : SingleMessageHandlerSelectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageHandlerSelector"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public DefaultMessageHandlerSelector(ICompositionContext compositionContext)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(Type messageType)
        {
            return true;
        }
    }
}