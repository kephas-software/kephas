// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnnotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for model annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Contract for model annotations.
    /// </summary>
    /// <remarks>
    /// Annotations have names starting with @ (the at sign).
    /// </remarks>
    [MemberNameDiscriminator("@")]
    [ContractClass(typeof(AnnotationContractClass))]
    public interface IAnnotation : INamedElement
    {
        /// <summary>
        /// Gets a value indicating whether multiple annotations of the same kind are allowed to be placed the same model element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple annotations of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple annotations of the same kind are allowed, the qualified name will have a generated suffix 
        /// to allow the annotation to be unique within the members collection.
        /// </remarks>
        bool AllowMultiple { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IAnnotation" />.
    /// </summary>
    [ContractClassFor(typeof(IAnnotation))]
    internal abstract class AnnotationContractClass : IAnnotation
    {
        /// <summary>
        /// Gets a value indicating whether multiple annotations of the same kind are allowed to be placed the same model element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple annotations of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple annotations of the same kind are allowed, the qualified name will have a generated suffix 
        /// to allow the annotation to be unique within the members collection.
        /// </remarks>
        public abstract bool AllowMultiple { get; }

        /// <summary>
        /// Gets the friendly name of the element.
        /// </summary>
        /// <value>
        /// The element name.
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
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.
        /// </returns>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}