// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionForAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the projection for attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    using Kephas.Reflection;

    /// <summary>
    /// Attribute for indicating that a classifier is a projection of another classifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ProjectionForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionForAttribute"/> class.
        /// </summary>
        /// <param name="projectedType">Type of the projected.</param>
        public ProjectionForAttribute(Type projectedType)
        {
            this.ProjectedType = projectedType ?? throw new ArgumentNullException(nameof(projectedType));
            this.ProjectedTypeName = projectedType.GetAssemblyQualifiedShortName();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionForAttribute"/> class.
        /// </summary>
        /// <param name="projectedTypeName">The name of the projected type.</param>
        public ProjectionForAttribute(string projectedTypeName)
        {
            this.ProjectedTypeName = projectedTypeName ?? throw new ArgumentNullException(nameof(projectedTypeName));
        }

        /// <summary>
        /// Gets the projected type.
        /// </summary>
        /// <value>
        /// The projected type.
        /// </value>
        public Type? ProjectedType { get; }

        /// <summary>
        /// Gets the projected type name.
        /// </summary>
        /// <value>
        /// The projected type name.
        /// </value>
        public string ProjectedTypeName { get; }
    }
}