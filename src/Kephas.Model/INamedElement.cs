// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedElement.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for named elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Model
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for named elements.
    /// </summary>
    public interface INamedElement : IAggregatedElementInfo
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
        string QualifiedFullName { get; }

        /// <summary>
        /// Gets the fully qualified name, starting from the root model space.
        /// </summary>
        /// <value>
        /// The fully qualified name.
        /// </value>
        /// <remarks>
        /// The fully qualified name is built up of qualified names separated by "/".
        /// </remarks>
        /// <example>
        /// <para>
        /// /: identifies the root model space.
        /// </para>
        /// <para>
        /// /^AppLayer: identifies the AppLayer dimension. 
        /// </para>
        /// <para>
        /// /:Primitives:Kephas:Core:Main:Global/String: identifies the String classifier within the :Primitives:Kephas:Core:Main:Global projection. 
        /// </para>
        /// <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name: identifies the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection. 
        /// </para>
        /// <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name/@Required: identifies the Required attribute of the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection. 
        /// </para>
        /// </example>
        new string FullName { get; }

        /// <summary>
        /// Gets the annotations of this model element.
        /// </summary>
        /// <value>
        /// The model element annotations.
        /// </value>
        new IEnumerable<IAnnotation> Annotations { get; }

        /// <summary>
        /// Gets the declaring container element.
        /// </summary>
        /// <value>
        /// The declaring container element.
        /// </value>
        new IModelElement? DeclaringContainer { get; }

#if NETSTANDARD2_1
        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.Annotations;

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        IElementInfo? IElementInfo.DeclaringContainer => this.DeclaringContainer;
#endif

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        IModelSpace ModelSpace { get; }

        /// <summary>
        /// Gets a value indicating whether this element is inherited.
        /// </summary>
        /// <value>
        /// <c>true</c> if the model element is inherited, <c>false</c> if not.
        /// </value>
        bool IsInherited { get; }
    }
}