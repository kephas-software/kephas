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
    using System.Dynamic;
    using System.Linq.Expressions;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for classifiers.
    /// </summary>
    [ContractClass(typeof(ClassifierContractClass))]
    public interface IClassifier : IModelElement, ITypeInfo
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
        new IEnumerable<IProperty> Properties { get; }

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
        IClassifier BaseClassifier { get; }

        /// <summary>
        /// Gets the base mixins.
        /// </summary>
        /// <value>
        /// The base mixins.
        /// </value>
        IEnumerable<IClassifier> BaseMixins { get; }
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
        public abstract IModelProjection Projection { get; }

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
        /// Gets a value indicating whether this classifier is a mixin.
        /// </summary>
        /// <value>
        /// <c>true</c> if this classifier is a mixin, <c>false</c> if not.
        /// </value>
        public abstract bool IsMixin { get; }

        /// <summary>
        /// Gets the base classifier.
        /// </summary>
        /// <value>
        /// The base classifier.
        /// </value>
        public abstract IClassifier BaseClassifier { get; }

        /// <summary>
        /// Gets the base mixins.
        /// </summary>
        /// <value>
        /// The base mixins.
        /// </value>
        public IEnumerable<IClassifier> BaseMixins
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
        public IElementInfo DeclaringContainer { get; }

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
        /// The full name.
        /// </value>
        public abstract string FullName { get; }

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
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        public abstract IEnumerable<object> Parts { get; }

        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public abstract string Namespace { get; }

        /// <summary>
        /// Gets the bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        public abstract IEnumerable<ITypeInfo> BaseTypes { get; }

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters
        /// of a closed generic type or the type parameters of a generic type definition.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public abstract IReadOnlyList<ITypeInfo> GenericTypeParameters { get; }

        /// <summary>
        /// Gets the generic type arguments.
        /// </summary>
        /// <value>
        /// The generic type arguments.
        /// </value>
        public abstract IReadOnlyList<ITypeInfo> GenericTypeArguments { get; }

        /// <summary>
        /// Gets a <see cref="ITypeInfo"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <value>
        /// The generic type definition.
        /// </value>
        public abstract ITypeInfo GenericTypeDefinition { get; }

        /// <summary>
        /// Gets an enumeration of properties.
        /// </summary>
        IEnumerable<IPropertyInfo> ITypeInfo.Properties => this.Properties;

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
        /// The <see cref="object" />.
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