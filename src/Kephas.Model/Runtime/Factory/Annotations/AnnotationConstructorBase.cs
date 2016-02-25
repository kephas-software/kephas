// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationConstructorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for runtime annotation information factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory.Annotations
{
    using System;
    using System.Reflection;

    using Kephas.Model.Elements;

    /// <summary>
    /// Base class for runtime annotation information factories.
    /// </summary>
    /// <typeparam name="TAnnotation">Type of the annotation information.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    public abstract class AnnotationConstructorBase<TAnnotation, TAttribute> : NamedElementConstructorBase<TAnnotation, TAttribute>
        where TAnnotation : NamedElementBase<TAnnotation>
        where TAttribute : Attribute
    {
        /// <summary>
        /// The annotation discriminator.
        /// </summary>
        public const string AnnotationDiscriminator = "Attribute";

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This dicriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => AnnotationDiscriminator;

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string ComputeName(object runtimeElement)
        {
            var attrTypeInfo = runtimeElement.GetType().GetDynamicTypeInfo();
            var usage = attrTypeInfo.TypeInfo.GetCustomAttribute<AttributeUsageAttribute>();
            // NOTE: The speciality of the runtime is to prepend the @ sign to the
            // attribute name, because the member name conventions imply it.
            // Other annotation info classes must provide the same convention.
            // This simplifies the member aggregation in the final classifier
            // because the name is already prepared by the info classes and the 
            // classifier must simply create the corresponding members.
            var name = "@" + base.ComputeName(attrTypeInfo);
            if (usage == null || usage.AllowMultiple)
            {
                name = name + "_" + runtimeElement.GetHashCode();
            }

            return name;
        }
    }
}