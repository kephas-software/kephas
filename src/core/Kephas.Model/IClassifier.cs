// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClassifier.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Contract for classifiers.
    /// </summary>
    [ContractClass(typeof(ClassifierContractClass))]
    public interface IClassifier : IModelElement
    {
        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        IModelProjection Projection { get; }

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        IEnumerable<IProperty> Properties { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IClassifier"/>.
    /// </summary>
    [ContractClassFor(typeof(IClassifier))]
    internal abstract class ClassifierContractClass : IClassifier
    {
        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public IModelProjection Projection { get; private set; }

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        public IEnumerable<IProperty> Properties
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IProperty>>() != null);
                return Contract.Result<IEnumerable<IProperty>>();
            }
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public abstract string Name { get; }

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
        /// For example, attributes use the "@" discriminator, dimensions use "^", and projections use ":".
        /// </remarks>
        public abstract string QualifiedName { get; }

        /// <summary>
        /// Gets the full name, starting from the root model space.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public abstract string FullQualifiedName { get; }

        /// <summary>
        /// Gets the container element.
        /// </summary>
        /// <value>
        /// The container element.
        /// </value>
        public abstract IModelElement Container { get; }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public abstract IModelSpace ModelSpace { get; }

        /// <summary>
        /// Gets the element infos which constructed this element.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        public abstract IEnumerable<INamedElementInfo> UnderlyingElementInfos { get; }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public abstract IEnumerable<INamedElement> Members { get; }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        public abstract IEnumerable<IModelAttribute> Attributes { get; }

        /// <summary>
        /// Gets the base model element.
        /// </summary>
        /// <value>
        /// The base model element.
        /// </value>
        public abstract IModelElement Base { get; }

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>
        /// The member with the provided qualified name or <c>null</c>.
        /// </returns>
        public abstract INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true);
    }
}