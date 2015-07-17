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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Dynamic;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Elements.Construction.Internal;

    /// <summary>
    /// Base class for named elements.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract.</typeparam>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    public abstract class NamedElementBase<TModelContract, TElementInfo> : Expando, INamedElement, INamedElementConstructor
        where TElementInfo : class, INamedElementInfo
    {
        /// <summary>
        /// The underlying element infos.
        /// </summary>
        private readonly IList<INamedElementInfo> underlyingElementInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementBase{TModelContract, TElementInfo}" /> class.
        /// </summary>
        /// <param name="elementInfo">The element information.</param>
        /// <param name="modelSpace">The model space.</param>
        protected NamedElementBase(TElementInfo elementInfo, IModelSpace modelSpace)
        {
            Contract.Requires(elementInfo != null);
            Contract.Requires(elementInfo.Name != null);
            Contract.Requires(modelSpace != null);

            this.Name = elementInfo.Name;
            this.ModelSpace = modelSpace;
            this.QualifiedName = typeof(TModelContract).GetMemberNameDiscriminator() + elementInfo.Name;

            this.underlyingElementInfos = new List<INamedElementInfo> { elementInfo };
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
        /// For example, annotations use the "@" discriminator, dimensions use "^", and projections use ":".
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
        public string FullyQualifiedName { get; private set; }

        /// <summary>
        /// Gets the container element.
        /// </summary>
        /// <value>
        /// The container element.
        /// </value>
        public virtual IModelElement Container { get; private set; }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public virtual IModelSpace ModelSpace { get; private set; }

        /// <summary>
        /// Gets the element infos which constructed this element.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        public IEnumerable<INamedElementInfo> UnderlyingElementInfos
        {
            get { return this.underlyingElementInfos; }
        }

        /// <summary>
        /// Sets the element container.
        /// </summary>
        /// <param name="container">The element container.</param>
        void INamedElementConstructor.SetContainer(IModelElement container)
        {
            this.Container = container;
        }

        /// <summary>
        /// Sets the fully qualified name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name.</param>
        void INamedElementConstructor.SetFullyQualifiedName(string fullyQualifiedName)
        {
            this.FullyQualifiedName = fullyQualifiedName;
        }

        /// <summary>
        /// Completes the construction of the element.
        /// </summary>
        void INamedElementConstructor.CompleteConstruction()
        {
            this.OnCompleteConstruction();
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        protected virtual void OnCompleteConstruction()
        {
        }
    }
}