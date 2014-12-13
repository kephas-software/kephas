// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedElementBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for named elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Diagnostics.Contracts;

    using Kephas.Model.Configuration;

    /// <summary>
    /// Base class for named elements.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract.</typeparam>
    public abstract class NamedElementBase<TModelContract> : INamedElement, IConfigurableElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementBase{TModelContract}"/> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="name">The name.</param>
        protected NamedElementBase(IModelSpace modelSpace, string name)
        {
            Contract.Requires(name != null);
            Contract.Requires(modelSpace != null);

            this.Name = name;
            this.ModelSpace = modelSpace;
            this.QualifiedName = typeof(TModelContract).GetMemberNameDiscriminator() + name;
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the qualified name of the element.
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
        public string QualifiedName { get; protected set; }

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
        public string FullQualifiedName { get; internal set; }

        /// <summary>
        /// Gets the container element.
        /// </summary>
        /// <value>
        /// The container element.
        /// </value>
        public IModelElement Container { get; internal set; }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public IModelSpace ModelSpace { get; private set; }

        /// <summary>
        /// Completes the configuration.
        /// </summary>
        public void CompleteConfiguration()
        {
            this.OnConfigurationComplete();
        }

        /// <summary>
        /// Called when the configuration is complete.
        /// </summary>
        protected virtual void OnConfigurationComplete()
        {
        }
    }
}