// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedElementBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime named element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction.Builders
{
    using System.Diagnostics.Contracts;

    using Kephas.Reflection;

    /// <summary>
    /// Base abstract builder for runtime named element information.
    /// </summary>
    /// <typeparam name="TNamedElement">Type of the named element information.</typeparam>
    /// <typeparam name="TElementInfo">Type of the runtime element.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class NamedElementBuilderBase<TNamedElement, TElementInfo, TBuilder>
        where TNamedElement : NamedElementBase<TNamedElement> 
        where TElementInfo : class, IElementInfo
        where TBuilder : NamedElementBuilderBase<TNamedElement, TElementInfo, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementBuilderBase{TNamedElement,TElementInfo,TBuilder}"/>
        /// class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected NamedElementBuilderBase(IModelSpace modelSpace, TElementInfo runtimeElement)
        {
            Contract.Requires(modelSpace != null);
            Contract.Requires(runtimeElement != null);

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.NamedElement = this.CreateNamedElement(runtimeElement, modelSpace);

            // TODO
            // this.NamedElement?.ConstructInfo(modelSpace);
        }

        /// <summary>
        /// Gets the element information to be built.
        /// </summary>
        /// <value>
        /// The element information.
        /// </value>
        public TNamedElement NamedElement { get; }

        /// <summary>
        /// Creates the element information out of the provided runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="modelSpace">The model space.</param>
        /// <returns>
        /// A new instance of <see typeparamref="TNamedElement"/>.
        /// </returns>
        protected abstract TNamedElement CreateNamedElement(TElementInfo runtimeElement, IModelSpace modelSpace);
    }
}