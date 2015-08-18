// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeAnnotationInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime information class for constructing annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Base runtime information class for constructing annotations.
    /// </summary>
    public abstract class RuntimeAnnotationInfoBase<TAttribute> : RuntimeNamedElementInfo<TAttribute>
        where TAttribute : Attribute
    {
        /// <summary>
        /// The annotation discriminator.
        /// </summary>
        public const string AnnotationDiscriminator = "Attribute";

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeAnnotationInfoBase{TRuntimeElement}"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        protected RuntimeAnnotationInfoBase(TAttribute runtimeElement)
            : base(runtimeElement)
        {
        }

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
            var attrTypeInfo = runtimeElement.GetType().GetTypeInfo();
            var usage = attrTypeInfo.GetCustomAttribute<AttributeUsageAttribute>();
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