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

    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;

    /// <summary>
    /// Base abstract class for model elements.
    /// </summary>
    /// <typeparam name="TModelContract">The type of the model contract (the model interface).</typeparam>
    public abstract class ModelElementBase<TModelContract> : NamedElementBase<TModelContract>, IModelElement
        where TModelContract : IModelElement
    {
        /// <summary>
        /// The members.
        /// </summary>
        private readonly IDictionary<string, INamedElement> members = new Dictionary<string, INamedElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelElementBase{TModelContract}"/> class.
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
        public override IEnumerable<IAnnotation> Annotations => this.Members.OfType<IAnnotation>();

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
            if (member.DeclaringContainer == null)
            {
                var memberBuilder = member as IWritableNamedElement;
                memberBuilder?.SetDeclaringContainer(this);
            }

            this.members.Add(member.Name, member);
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            // Complete construction for declared members
            foreach (var declaredMember in this.GetDeclaredMembers())
            {
                (declaredMember as IWritableNamedElement)?.CompleteConstruction(constructionContext);
            }

            base.OnCompleteConstruction(constructionContext);
        }
    }
}