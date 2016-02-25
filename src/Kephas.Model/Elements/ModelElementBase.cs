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

    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// Base abstract class for model elements.
    /// </summary>
    /// <typeparam name="TModelElement">The type of the model contract.</typeparam>
    public abstract class ModelElementBase<TModelElement> : NamedElementBase<TModelElement>, IModelElement
    {
        /// <summary>
        /// The members.
        /// </summary>
        private readonly IDictionary<string, INamedElement> members = new Dictionary<string, INamedElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBase{TModelElement}"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        protected ModelElementBase(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        public virtual IEnumerable<INamedElement> Members => this.members.Values;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public override IEnumerable<IAnnotation> Annotations => this.members.OfType<IAnnotation>();

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
        protected override void AddMember(INamedElement member)
        {
            var memberBuilder = member as INamedElementConstructor;
            memberBuilder?.SetContainer(this);

            this.members.Add(member.Name, member);
        }
    }
}