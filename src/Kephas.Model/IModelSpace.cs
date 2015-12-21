// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelSpace.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The model space is the root model element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// The model space is the root model element.
    /// </summary>
    [ContractClass(typeof(ModelSpaceContractClass))]
    public interface IModelSpace : IModelElement
    {
        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        IModelDimension[] Dimensions { get; }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        IEnumerable<IModelProjection> Projections { get; }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        IEnumerable<IClassifier> Classifiers { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IModelSpace"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelSpace))]
    internal abstract class ModelSpaceContractClass : IModelSpace
    {
        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        public IModelDimension[] Dimensions
        {
            get
            {
                Contract.Ensures(Contract.Result<IModelDimension[]>() != null);
                return Contract.Result<IModelDimension[]>();
            }
        }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        public IEnumerable<IModelProjection> Projections
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IModelProjection>>() != null);
                return Contract.Result<IEnumerable<IModelProjection>>();
            }
        }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        public IEnumerable<IClassifier> Classifiers
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IClassifier>>() != null);
                return Contract.Result<IEnumerable<IClassifier>>();
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
        /// For example, annotations use the "@" discriminator, dimensions use "^", and projections use ":".
        /// </remarks>
        public abstract string QualifiedName { get; }

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
        ///   <para>
        /// /: identifies the root model space.
        /// </para>
        ///   <para>
        /// /^AppLayer: identifies the AppLayer dimension.
        /// </para>
        ///   <para>
        /// /:Primitives:Kephas:Core:Main:Global/String: identifies the String classifier within the :Primitives:Kephas:Core:Main:Global projection.
        /// </para>
        ///   <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name: identifies the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection.
        /// </para>
        ///   <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name/@Required: identifies the Required attribute of the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection.
        /// </para>
        /// </example>
        public abstract string FullyQualifiedName { get; }

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
        /// Gets the annotations of this model element.
        /// </summary>
        /// <value>
        /// The model element annotations.
        /// </value>
        public abstract IEnumerable<IAnnotation> Annotations { get; }

        /// <summary>
        /// Gets the base model element.
        /// </summary>
        /// <value>
        /// The base model element.
        /// </value>
        public abstract IModelElement Base { get; }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>
        /// The member with the provided qualified name or <c>null</c>.
        /// </returns>
        public abstract INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true);

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.
        /// </returns>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}