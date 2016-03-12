// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Definition class for properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Construction;

    /// <summary>
    /// Definition class for properties.
    /// </summary>
    public class Property : ModelElementBase<IProperty>, IProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Property(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public IClassifier PropertyType { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise, <c>false</c>.
        /// </value>
        public bool CanWrite { get; internal set; }
    }
}