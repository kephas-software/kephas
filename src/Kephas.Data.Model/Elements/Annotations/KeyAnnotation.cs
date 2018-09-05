// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the key annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements.Annotations
{
    using System;

    using Kephas.Data.Model.AttributedModel;
    using Kephas.Model;
    using Kephas.Model.Configuration;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Model.Elements.Annotations;

    /// <summary>
    /// Defines annotations for entity keys.
    /// </summary>
    public class KeyAnnotation : AttributeAnnotation<KeyAttribute>, IElementConfigurator
    {
        /// <summary>
        /// Name of the key.
        /// </summary>
        private readonly string baseKeyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        /// <param name="attribute">The attribute.</param>
        public KeyAnnotation(IModelConstructionContext constructionContext, string name, KeyAttribute attribute)
            : base(constructionContext, $"{name}_{typeof(KeyAnnotation).Name}", attribute)
        {
            this.baseKeyName = name;
        }

        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        public void Configure(IModelConstructionContext constructionContext, INamedElement element)
        {
            if (!(element is IConstructibleElement constructibleElement))
            {
                throw new NonConstructibleElementException(element);
            }

            var keyName = typeof(IKey).GetMemberNameDiscriminator() + this.baseKeyName;
            var key = new Key(constructionContext, keyName, this.Attribute.Kind, this.Attribute.KeyProperties);
            constructibleElement.AddMember(key);
        }
    }
}