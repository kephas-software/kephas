// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for model attributes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Diagnostics.Contracts;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Contract for model attributes.
    /// </summary>
    /// <remarks>
    /// Attributes have names starting with @ (the at sign).
    /// </remarks>
    [MemberNameDiscriminator("@")]
    [ContractClass(typeof(ModelAttributeContractClass))]
    public interface IModelAttribute : INamedElement
    {
        /// <summary>
        /// Gets a value indicating whether multiple attributes of the same kind are allowed to annotate the same model element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple attributes of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple attributes of the same kind are allowed, the qualified name will have a generated suffix 
        /// to allow the attribute to be unique within the members collection.
        /// </remarks>
        bool AllowMultiple { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IModelAttribute"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelAttribute))]
    internal abstract class ModelAttributeContractClass : IModelAttribute
    {
        /// <summary>
        /// Gets a value indicating whether multiple attributes of the same kind are allowed to annotate the same model element.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple attributes of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple attributes of the same kind are allowed, the qualified name will have a generated suffix
        /// to allow the attribute to be unique within the members collection.
        /// </remarks>
        public bool AllowMultiple { get; private set; }

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
        /// For example, attributes use the "@" discriminator, dimensions use "^", and projections use ":".
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
    }
}