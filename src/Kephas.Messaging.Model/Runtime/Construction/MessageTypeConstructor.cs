// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTypeConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Messaging.Model.Elements;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Runtime;

    /// <summary>
    /// Classifier constructor for <see cref="MessageType"/>.
    /// </summary>
    public class MessageTypeConstructor : ClassifierConstructorBase<MessageType, IMessageType>
    {
        /// <summary>
        /// The marker interface.
        /// </summary>
        private static readonly TypeInfo MarkerInterface = typeof(IMessage).GetTypeInfo();

        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return MarkerInterface.IsAssignableFrom(runtimeElement.TypeInfo);
        }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override MessageType TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return new MessageType(constructionContext, this.TryComputeName(runtimeElement, constructionContext));
        }
    }
}