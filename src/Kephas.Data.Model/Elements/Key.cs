// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity key class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements
{
    using System.Linq;
    using Kephas.Collections;
    using Kephas.Data.Model.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Definition class for entity keys.
    /// </summary>
    public class Key : ModelElementBase<IKey>, IKey
    {
        /// <summary>
        /// The empty key properties.
        /// </summary>
        private static readonly IProperty[] EmptyKeyProperties = new IProperty[0];

        /// <summary>
        /// The key properties.
        /// </summary>
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
            Requires.NotNullOrEmpty(keyProperties, nameof(keyProperties));

            this.Kind = kind;
            this.KeyProperties = EmptyKeyProperties;
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
        public IProperty[] KeyProperties { get; private set; }

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

            var missingProperties = this.keyPropertyNames.Where(n => containerProperties.TryGetValue(n) == null).ToArray();
            if (missingProperties.Length > 0)
            {
                throw new ModelConstructionException(string.Format(Strings.Key_MissingContainerProperties_Exception, this, this.DeclaringContainer, string.Join("', '", missingProperties)));
            }

            this.KeyProperties = this.keyPropertyNames.Select(n => containerProperties[n]).ToArray();
        }
    }
}