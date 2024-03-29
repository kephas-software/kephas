﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marks a classifier as being an aspect of an other classifier.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;


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
            classifierFilter = classifierFilter ?? throw new System.ArgumentNullException(nameof(classifierFilter));

            this.ClassifierFilter = classifierFilter;
        }

        /// <summary>
        /// Gets the classifier filter.
        /// </summary>
        public Func<IClassifier, bool> ClassifierFilter { get; }
    }
}