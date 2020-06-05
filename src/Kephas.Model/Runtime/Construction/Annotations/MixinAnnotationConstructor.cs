// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixinAnnotationConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mixin annotation constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Annotations
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements.Annotations;

    /// <summary>
    /// A mixin annotation constructor.
    /// </summary>
    public class MixinAnnotationConstructor : AnnotationConstructorBase<MixinAnnotation, MixinAttribute>
    {
        /// <summary>
        /// The static instance of the <see cref="MixinAnnotationConstructor"/>.
        /// </summary>
        public static readonly MixinAnnotationConstructor Instance = new MixinAnnotationConstructor();

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override MixinAnnotation TryCreateModelElementCore(IModelConstructionContext constructionContext, MixinAttribute runtimeElement)
        {
            return new MixinAnnotation(constructionContext, this.TryComputeName(runtimeElement, constructionContext));
        }
    }
}