// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// Classifier constructor for <see cref="Message"/>.
    /// </summary>
    public class MessageConstructor : ClassifierConstructorBase<Message, IMessage>
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
            return base.CanCreateModelElement(constructionContext, runtimeElement);
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
        protected override Message TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            if (!MarkerInterface.IsAssignableFrom(runtimeElement.TypeInfo))
            {
                return null;
            }

            return new Message(constructionContext, this.TryComputeName(constructionContext, runtimeElement));
        }
    }
}