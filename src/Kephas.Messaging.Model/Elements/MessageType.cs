// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.Elements
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Classifier for DTOs used in messaging.
    /// </summary>
    public class MessageType : ClassifierBase<IMessageType>, IMessageType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public MessageType(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }
    }
}