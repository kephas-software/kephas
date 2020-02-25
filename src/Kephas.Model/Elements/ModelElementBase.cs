// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base abstract class for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Configuration;
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
        private readonly Dictionary<string, INamedElement> members = new Dictionary<string, INamedElement>();

        /// <summary>
        /// The member counter.
        /// </summary>
        private int memberCounter = 0;

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
        public virtual INamedElement? GetMember(string qualifiedName, bool throwOnNotFound = true)
        {
            if (this.members.TryGetValue(qualifiedName, out var element))
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
                var memberBuilder = member as IConstructibleElement;
                memberBuilder?.SetDeclaringContainer(this);
            }

            if (this.members.ContainsKey(member.Name))
            {
                this.members.Add($"{member.Name}_{this.memberCounter++}", member);
            }
            else
            {
                this.members.Add(member.Name, member);
            }
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            var declaredMembers = ((IModelElement)this).GetDeclaredMembers();

            // invoke all members which are element configurators
            var configurators = declaredMembers.OfType<IElementConfigurator>().ToList();
            foreach (var configurator in configurators)
            {
                configurator.Configure(constructionContext, this);
            }

            // Complete construction for declared members
            foreach (var declaredMember in declaredMembers)
            {
                (declaredMember as IConstructibleElement)?.CompleteConstruction(constructionContext);
            }

            base.OnCompleteConstruction(constructionContext);
        }
    }
}