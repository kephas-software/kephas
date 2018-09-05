// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the attribute annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Annotations
{
    using System;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Runtime;

    /// <summary>
    /// An annotation holding a runtime attribute.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    public class AttributeAnnotation<TAttribute> : Annotation, IAttributeAnnotation, IAttributeProvider
        where TAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeAnnotation{TAttribute}"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        /// <param name="attribute">The attribute.</param>
        public AttributeAnnotation(IModelConstructionContext constructionContext, string name, TAttribute attribute)
            : base(constructionContext, name)
        {
            Requires.NotNull(attribute, nameof(attribute));

            this.Attribute = attribute;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public TAttribute Attribute { get; }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        Attribute IAttributeAnnotation.Attribute => this.Attribute;

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TRuntimeAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        public override IEnumerable<TRuntimeAttribute> GetAttributes<TRuntimeAttribute>()
        {
            if (!(this.Attribute is TRuntimeAttribute runtimeAttribute))
            {
                return new TRuntimeAttribute[0];
            }

            return new[] { runtimeAttribute };
        }
    }
}