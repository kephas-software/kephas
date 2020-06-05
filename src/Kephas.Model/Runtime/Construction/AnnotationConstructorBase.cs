// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationConstructorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for runtime annotation information factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System;
    using System.Reflection;
    using System.Threading;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// Base class for runtime annotation information factories.
    /// </summary>
    /// <typeparam name="TAnnotation">Type of the annotation information.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    public abstract class AnnotationConstructorBase<TAnnotation, TAttribute> : NamedElementConstructorBase<TAnnotation, IAnnotation, TAttribute>
        where TAnnotation : Annotation
        where TAttribute : Attribute
    {
        /// <summary>
        /// The annotation discriminator.
        /// </summary>
        public const string AnnotationDiscriminator = "Attribute";

        /// <summary>
        /// The name counter.
        /// </summary>
        private static int nameCounter = 0;

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This discriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => AnnotationDiscriminator;

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="constructionContext">The construction context.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string? TryComputeNameCore(
            object runtimeElement,
            IModelConstructionContext constructionContext)
        {
            var attrTypeInfo = runtimeElement.GetRuntimeTypeInfo(constructionContext.AmbientServices?.TypeRegistry);
            var usage = attrTypeInfo.TypeInfo.GetCustomAttribute<AttributeUsageAttribute>();
            // NOTE: The speciality of the runtime is to prepend the @ sign to the
            // attribute name, because the member name conventions imply it.
            // Other annotation info classes must provide the same convention.
            // This simplifies the member aggregation in the final classifier
            // because the name is already prepared by the info classes and the 
            // classifier must simply create the corresponding members.
            var name = typeof(IAnnotation).GetMemberNameDiscriminator() + base.TryComputeNameCore(attrTypeInfo, constructionContext);
            if (usage == null || usage.AllowMultiple)
            {
                var counter = Interlocked.Increment(ref nameCounter);
                name = name + "_" + counter;
            }

            return name;
        }

        /// <summary>
        /// Constructs the model element content.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="element">The element being constructed.</param>
        protected override void ConstructModelElementContent(
            IModelConstructionContext constructionContext,
            TAttribute runtimeElement,
            TAnnotation element)
        {
            var attrTypeInfo = runtimeElement.GetRuntimeTypeInfo(constructionContext.AmbientServices?.TypeRegistry);
            var usage = attrTypeInfo.TypeInfo.GetCustomAttribute<AttributeUsageAttribute>();
            if (usage != null)
            {
                element.AllowMultiple = usage.AllowMultiple;
                element.IsInherited = usage.Inherited;
            }
        }
    }
}