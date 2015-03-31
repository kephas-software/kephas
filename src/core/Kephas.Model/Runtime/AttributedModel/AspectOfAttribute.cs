// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectOfAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks a classifier as being an aspect of an other classifier.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.AttributedModel
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Marks a classifier as being an aspect of an other classifier.
    /// </summary>
    /// <remarks>
    /// By using this attribute, aspects can be added to an existing type without altering its existing definition.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AspectOfAttribute : MixinAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspectOfAttribute"/> class.
        /// </summary>
        /// <param name="target">The target entity.</param>
        public AspectOfAttribute(Type target)
        {
            Contract.Requires(target != null);

            this.Target = target;
        }

        /// <summary>
        /// Gets the type of the target entity.
        /// </summary>
        /// <value>
        /// The type of the target entity.
        /// </value>
        public Type Target { get; private set; }
    }
}