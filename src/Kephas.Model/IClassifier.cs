﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClassifier.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Reflection;

    /// <summary>
    /// Contract for classifiers.
    /// </summary>
    public interface IClassifier : IModelElement, ITypeInfo
    {
        /// <summary>
        /// Gets the qualified name of the element.
        /// </summary>
        /// <value>
        /// The qualified name of the element.
        /// </value>
        /// <remarks>
        /// The qualified name is unique within the container's members.
        /// Some elements have the qualified name the same as their name,
        /// but others will use a discriminator prefix to avoid name collisions.
        /// For example, annotations use the "@" discriminator, dimensions use "^", and projections use ":".
        /// </remarks>
        new string QualifiedFullName { get; }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        new IEnumerable<INamedElement> Members { get; }

        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        IModelProjection? Projection { get; }

#if NETSTANDARD2_0
        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        new IEnumerable<IProperty> Properties { get; }

        /// <summary>
        /// Gets the classifier methods.
        /// </summary>
        /// <value>
        /// The classifier methods.
        /// </value>
        IEnumerable<IMethod> Methods { get; }
#else
        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        IEnumerable<IPropertyInfo> ITypeInfo.Properties => this.Members.OfType<IPropertyInfo>();

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        new IEnumerable<IProperty> Properties => this.Members.OfType<IProperty>();

        /// <summary>
        /// Gets the classifier methods.
        /// </summary>
        /// <value>
        /// The classifier methods.
        /// </value>
        IEnumerable<IMethod> Methods => this.Members.OfType<IMethod>();

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> ITypeInfo.Members => this.Members;

        /// <summary>
        /// Gets a member by the provided name.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="throwIfNotFound">True to throw if the requested member is not found.</param>
        /// <returns>
        /// The requested member, or <c>null</c>.
        /// </returns>
        IElementInfo? ITypeInfo.GetMember(string name, bool throwIfNotFound) => this.GetMember(name, throwIfNotFound);
#endif

        /// <summary>
        /// Gets a value indicating whether this classifier is a mixin.
        /// </summary>
        /// <value>
        /// <c>true</c> if this classifier is a mixin, <c>false</c> if not.
        /// </value>
        bool IsMixin { get; }

        /// <summary>
        /// Gets the base classifier.
        /// </summary>
        /// <value>
        /// The base classifier.
        /// </value>
        IClassifier? BaseClassifier { get; }

        /// <summary>
        /// Gets the base mixins.
        /// </summary>
        /// <value>
        /// The base mixins.
        /// </value>
        IEnumerable<IClassifier> BaseMixins { get; }

        /// <summary>
        /// Gets a value indicating whether this classifier is an aspect of other classifiers.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this classifier is an aspect of other classifiers, <c>false</c> if not.
        /// </returns>
        bool IsAspect { get; }

        /// <summary>
        /// Indicates whether this classifier is an aspect of the provided classifier.
        /// </summary>
        /// <param name="classifier">The classifier.</param>
        /// <returns>
        /// <c>true</c> if this classifier is an aspect of the provided classifier, <c>false</c> if not.
        /// </returns>
        bool IsAspectOf(IClassifier classifier);

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>The member with the provided qualified name or <c>null</c>.</returns>
        new INamedElement? GetMember(string qualifiedName, bool throwOnNotFound = true);
    }
}