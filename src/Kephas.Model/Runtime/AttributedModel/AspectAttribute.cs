// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectAttribute.cs" company="Quartz Software SRL">
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
    /// Marks a mixin classifier as being an aspect of an other classifiers matching some condition.
    /// </summary>
    /// <remarks>
    /// By using this attribute, aspects can be added to existing classifiers without altering their existing definition.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class AspectAttribute : MixinAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspectAttribute"/> class.
        /// </summary>
        /// <param name="classifierFilter">The classifier filter.</param>
        public AspectAttribute(Func<IClassifier, bool> classifierFilter)
        {
            Contract.Requires(classifierFilter != null);

            this.ClassifierFilter = classifierFilter;
        }

        /// <summary>
        /// Gets the classifier filter.
        /// </summary>
        public Func<IClassifier, bool> ClassifierFilter { get; }
    }
}