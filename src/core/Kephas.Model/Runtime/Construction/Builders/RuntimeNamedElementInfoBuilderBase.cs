// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeNamedElementInfoBuilderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract builder for runtime named element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Model.Runtime.Factory;

    /// <summary>
    /// Base abstract builder for runtime named element information.
    /// </summary>
    /// <typeparam name="TNamedElementInfo">Type of the named element information.</typeparam>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    public abstract class RuntimeNamedElementInfoBuilderBase<TNamedElementInfo, TRuntimeElement, TBuilder>
        where TNamedElementInfo : RuntimeNamedElementInfo<TRuntimeElement> 
        where TRuntimeElement : MemberInfo
        where TBuilder : RuntimeNamedElementInfoBuilderBase<TNamedElementInfo, TRuntimeElement, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RuntimeNamedElementInfoBuilderBase{TNamedElementInfo,TRuntimeElement,TBuilder}"/>
        /// class.
        /// </summary>
        /// <param name="runtimeModelInfoFactory">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        protected RuntimeNamedElementInfoBuilderBase(IRuntimeModelInfoFactory runtimeModelInfoFactory, TRuntimeElement runtimeElement)
        {
            Contract.Requires(runtimeElement != null);

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.ElementInfo = this.CreateElementInfo(runtimeElement);
            this.ElementInfo?.ConstructInfo(runtimeModelInfoFactory);
        }

        /// <summary>
        /// Gets the element information to be built.
        /// </summary>
        /// <value>
        /// The element information.
        /// </value>
        public TNamedElementInfo ElementInfo { get; }

        /// <summary>
        /// Creates the element information out of the provided runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new instance of <see cref="TNamedElementInfo"/>.
        /// </returns>
        protected abstract TNamedElementInfo CreateElementInfo(TRuntimeElement runtimeElement);
    }
}