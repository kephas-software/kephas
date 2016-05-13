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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Resources;
    using Kephas.Reflection;
    using Kephas.Services.Transitioning;

    /// <summary>
    /// Base class for named elements.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract (the interface).</typeparam>
    public abstract class NamedElementBase<TModelContract> : Expando, INamedElement, IWritableNamedElement
        where TModelContract : INamedElement
    {
        /// <summary>
        /// The underlying element infos.
        /// </summary>
        private readonly IList<object> parts;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementBase{TModelContract}" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        protected NamedElementBase(IModelConstructionContext constructionContext, string name)
            : base(isThreadSafe: true)
        {
            Contract.Requires(constructionContext != null);
            Contract.Requires(name != null);

            if (constructionContext.ModelSpace == null)
            {
                var thisModelSpace = this as IModelSpace;
                if (thisModelSpace == null)
                {
                    throw new InvalidOperationException(Strings.NamedElementBase_MissingModelSpaceInConstructionContext_Exception);
                }

                constructionContext[nameof(IModelConstructionContext.ModelSpace)] = thisModelSpace;
            }

            this.Name = name;
            this.ModelSpace = constructionContext.ModelSpace;
            this.QualifiedName = typeof(TModelContract).GetMemberNameDiscriminator() + name;

            this.parts = new List<object>();

            this.ConstructionMonitor = new InitializationMonitor<TModelContract>(this.GetType());
            this.ConstructionMonitor.Start();
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the annotations of this model element.
        /// </summary>
        /// <value>
        /// The model element annotations.
        /// </value>
        public abstract IEnumerable<IAnnotation> Annotations { get; }

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        IElementInfo IElementInfo.DeclaringContainer => this.Container;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.Annotations;

        /// <summary>
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        IEnumerable<object> IAggregatedElementInfo.Parts => this.parts;

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
        public string FullName { get; private set; }

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
        public virtual IModelSpace ModelSpace { get; }

        /// <summary>
        /// Gets the construction monitor.
        /// </summary>
        /// <value>
        /// The construction monitor.
        /// </value>
        protected InitializationMonitor<TModelContract> ConstructionMonitor { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.QualifiedName;
        }

        /// <summary>
        /// Sets the element container.
        /// </summary>
        /// <param name="container">The element container.</param>
        void IWritableNamedElement.SetContainer(IModelElement container)
        {
            this.ConstructionMonitor.AssertIsInProgress();
            this.Container = container;
        }

        /// <summary>
        /// Sets the full name.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        void IWritableNamedElement.SetFullName(string fullName)
        {
            this.ConstructionMonitor.AssertIsInProgress();
            this.FullName = fullName;
        }

        /// <summary>
        /// Completes the construction of the element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        void IWritableNamedElement.CompleteConstruction(IModelConstructionContext constructionContext)
        {
            this.ConstructionMonitor.AssertIsInProgress();
            try
            {
                this.OnCompleteConstruction(constructionContext);
                this.ConstructionMonitor.Complete();
            }
            catch (Exception exception)
            {
                this.ConstructionMonitor.Fault(exception);
            }
        }

        /// <summary>
        /// Adds the member to the members list.
        /// </summary>
        /// <param name="member">The member.</param>
        void IWritableNamedElement.AddMember(INamedElement member)
        {
            this.ConstructionMonitor.AssertIsInProgress();
            this.AddMember(member);
        }

        /// <summary>
        /// Adds a part to the aggregated element.
        /// </summary>
        /// <param name="part">The part to be added.</param>
        void IWritableNamedElement.AddPart(object part)
        {
            this.ConstructionMonitor.AssertIsInProgress();
            this.AddPart(part);
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected virtual void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            Contract.Requires(constructionContext != null);
        }

        /// <summary>
        /// Adds a part to the aggregated element.
        /// </summary>
        /// <param name="part">The part to be added.</param>
        protected virtual void AddPart(object part)
        {
            this.parts.Add(part);
        }

        /// <summary>
        /// Adds the member to the members list.
        /// </summary>
        /// <param name="member">The member.</param>
        protected virtual void AddMember(INamedElement member)
        {
            Contract.Requires(member != null);
        }
    }
}