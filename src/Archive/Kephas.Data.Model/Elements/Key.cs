// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity key class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Data.Model.Resources;
    using Kephas.Model;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;

    /// <summary>
    /// Definition class for entity keys.
    /// </summary>
    public class Key : ModelElementBase<IKey>, IKey
    {
        private string[] keyPropertyNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        /// <param name="kind">The key kind.</param>
        /// <param name="keyProperties">The key properties.</param>
        public Key(IModelConstructionContext constructionContext, string name, KeyKind kind, string[] keyProperties)
            : base(constructionContext, name)
        {
            if (keyProperties == null || keyProperties.Length == 0) throw new System.ArgumentException("Value must not be null or empty.", nameof(keyProperties));

            this.Kind = kind;
            this.keyPropertyNames = keyProperties;
        }

        /// <summary>
        /// Gets the key kind.
        /// </summary>
        /// <value>
        /// The key kind.
        /// </value>
        public KeyKind Kind { get; }

        /// <summary>
        /// Gets the entity properties in the proper order which are part of the key.
        /// </summary>
        /// <value>
        /// The key properties.
        /// </value>
        public IProperty[] KeyProperties { get; private set; } = Array.Empty<IProperty>();

        /// <summary>
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        IEnumerable<object> IAggregatedElementInfo.Parts => this.Parts;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.Annotations;

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            base.OnCompleteConstruction(constructionContext);

            var containerProperties = this.DeclaringContainer?.Members.OfType<IProperty>().ToDictionary(m => m.Name, m => m);
            if (containerProperties == null || containerProperties.Count == 0)
            {
                throw new ModelConstructionException(string.Format(Strings.Key_ContainerPropertiesEmpty_Exception, this, this.DeclaringContainer));
            }

            var missingProperties = this.keyPropertyNames.Where(n => !containerProperties.TryGetValue(n, out _)).ToArray();
            if (missingProperties.Length > 0)
            {
                throw new ModelConstructionException(string.Format(Strings.Key_MissingContainerProperties_Exception, this, this.DeclaringContainer, string.Join("', '", missingProperties)));
            }

            this.KeyProperties = this.keyPropertyNames.Select(n => containerProperties[n]).ToArray();
        }
    }
}