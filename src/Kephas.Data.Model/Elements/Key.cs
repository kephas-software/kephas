// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity key class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements
{
    using Kephas.Model;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Definition class for entity keys.
    /// </summary>
    public class Key : ModelElementBase<IKey>, IKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Key(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
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
        public IProperty[] KeyProperties { get; }
    }
}