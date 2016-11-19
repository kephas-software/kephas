// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionForAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the projection for attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for indicating that a classifier is a projection of another classifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ProjectionForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionForAttribute"/> class.
        /// </summary>
        /// <param name="projectedType">Type of the projected.</param>
        public ProjectionForAttribute(Type projectedType)
        {
            Contract.Requires(projectedType != null);

            this.ProjectedType = projectedType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionForAttribute"/> class.
        /// </summary>
        /// <param name="projectedTypeName">The name of the projected type.</param>
        public ProjectionForAttribute(string projectedTypeName)
        {
            Contract.Requires(!string.IsNullOrEmpty(projectedTypeName));

            this.ProjectedTypeName = projectedTypeName;
        }

        /// <summary>
        /// Gets the projected type.
        /// </summary>
        /// <value>
        /// The projected type.
        /// </value>
        public Type ProjectedType { get; }

        /// <summary>
        /// Gets the projected type name.
        /// </summary>
        /// <value>
        /// The projected type name.
        /// </value>
        public string ProjectedTypeName { get; }
    }
}