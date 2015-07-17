// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base abstract class for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Elements.Construction.Internal;

    /// <summary>
    /// Base abstract class for model elements.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract.</typeparam>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    public abstract class ModelElementBase<TModelContract, TElementInfo> : NamedElementBase<TModelContract, TElementInfo>, IModelElement, IModelElementConstructor
        where TElementInfo : class, IModelElementInfo
    {
        /// <summary>
        /// The members.
        /// </summary>
        private readonly IDictionary<string, INamedElement> members = new Dictionary<string, INamedElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBase{TModelContract, TElementInfo}" /> class.
        /// </summary>
        /// <param name="elementInfo">The element information.</param>
        /// <param name="modelSpace">The model space.</param>
        protected ModelElementBase(TElementInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
        {
        }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public virtual IEnumerable<INamedElement> Members
        {
            get
            {
                return this.members.Values;
            }
        }

        /// <summary>
        /// Gets the annotations of this model element.
        /// </summary>
        /// <value>
        /// The model element annotations.
        /// </value>
        public virtual IEnumerable<IAnnotation> Annotations
        {
            get { return this.members.OfType<IAnnotation>(); }
        }

        /// <summary>
        /// Gets the base model element.
        /// </summary>
        /// <value>
        /// The base model element.
        /// </value>
        public virtual IModelElement Base { get; internal set; }

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>
        /// The member with the provided qualified name or <c>null</c>.
        /// </returns>
        public virtual INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true)
        {
            INamedElement element;
            if (this.members.TryGetValue(qualifiedName, out element))
            {
                return element;
            }

            if (throwOnNotFound)
            {
                throw new ElementNotFoundException(qualifiedName, this.Name);
            }

            return null;
        }

        /// <summary>
        /// Adds the member to the members list.
        /// </summary>
        /// <param name="member">The member.</param>
        void IModelElementConstructor.AddMember(INamedElement member)
        {
            var memberBuilder = member as INamedElementConstructor;
            if (memberBuilder != null)
            {
                memberBuilder.SetContainer(this);
            }

            this.members.Add(member.Name, member);
        }
    }
}