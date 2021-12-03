﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedElementBuilderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base abstract builder for runtime named element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;

    /// <summary>
    /// Base abstract builder for runtime named element information.
    /// </summary>
    /// <typeparam name="TModel">Type of the model being built.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    /// <typeparam name="TRuntime">Type of the runtime element.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class NamedElementBuilderBase<TModel, TModelContract, TRuntime, TBuilder>
        where TModel : NamedElementBase<TModelContract>
        where TModelContract : INamedElement
        where TRuntime : class, IElementInfo
        where TBuilder : NamedElementBuilderBase<TModel, TModelContract, TRuntime, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementBuilderBase{TModel, TModelContract, TRuntime, TBuilder}"/>
        /// class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected NamedElementBuilderBase(IModelConstructionContext constructionContext, TRuntime runtimeElement)
        {
            constructionContext = constructionContext ?? throw new System.ArgumentNullException(nameof(constructionContext));
            runtimeElement = runtimeElement ?? throw new System.ArgumentNullException(nameof(runtimeElement));
            if (constructionContext.ModelSpace == null)
            {
                throw new ArgumentException("The model space is not set in the construction context.", nameof(constructionContext));
            }

            this.ConstructionContext = constructionContext;

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            var constructor = this.CreateElementConstructor(constructionContext, runtimeElement);
            this.Element = (TModel)constructor.TryCreateModelElement(constructionContext, runtimeElement);
        }

        /// <summary>
        /// Gets the construction context.
        /// </summary>
        /// <value>
        /// The construction context.
        /// </value>
        public IModelConstructionContext ConstructionContext { get; }

        /// <summary>
        /// Gets the element information to be built.
        /// </summary>
        /// <value>
        /// The element information.
        /// </value>
        public TModel Element { get; }

        /// <summary>
        /// Creates the element information out of the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new instance of <see typeparamref="TNamedElement"/>.
        /// </returns>
        protected abstract IRuntimeModelElementConstructor CreateElementConstructor(IModelConstructionContext constructionContext, TRuntime runtimeElement);
    }
}