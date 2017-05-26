// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the type information localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

    /// <summary>
    /// The type info localization.
    /// </summary>
    public class TypeInfoLocalization : ElementInfoLocalization, ITypeInfoLocalization
    {
        /// <summary>
        /// The members' localizations.
        /// </summary>
        private IDictionary<string, IMemberInfoLocalization> members;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoLocalization"/> class.
        /// </summary>
        public TypeInfoLocalization()
        {
            this.members = new Dictionary<string, IMemberInfoLocalization>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoLocalization"/> class.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        public TypeInfoLocalization(ITypeInfo typeInfo)
            : base(typeInfo)
        {
            this.members = new Dictionary<string, IMemberInfoLocalization>();
            foreach (var member in typeInfo.Members)
            {
                // avoid cases of overloaded members
                // in such cases consider only the first member with the provided name.
                if (this.members.ContainsKey(member.Name))
                {
                    continue;
                }

                // ReSharper disable once VirtualMemberCallInConstructor
                this.members.Add(member.Name, this.CreateMemberInfoLocalization(member));
            }
        }

        /// <summary>
        /// Gets or sets a dictionary of members' localizations.
        /// </summary>
        /// <value>
        /// The members' localizations.
        /// </value>
        public IDictionary<string, IMemberInfoLocalization> Members
        {
            get => this.members;
            set
            {
                Requires.NotNull(value, nameof(value));

                this.members = value;
            }
        }

        /// <summary>
        /// Tries to get the display attribute from the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <returns>
        /// A DisplayAttribute or <c>null</c>.
        /// </returns>
        protected override DisplayAttribute TryGetDisplayAttribute(IElementInfo elementInfo)
        {
            var typeAnnotations = elementInfo?.Annotations;
            var typeDisplayAttribute = typeAnnotations?.OfType<TypeDisplayAttribute>().FirstOrDefault();
            if (typeDisplayAttribute == null)
            {
                typeDisplayAttribute = typeAnnotations
                    ?.OfType<IAttributeProvider>()
                    .Select(p => p.GetAttribute<TypeDisplayAttribute>())
                    .FirstOrDefault(a => a != null);
            }

            if (typeDisplayAttribute != null)
            {
                return new DisplayAttribute { ResourceType = typeDisplayAttribute.ResourceType, Name = typeDisplayAttribute.Name, Description = typeDisplayAttribute.Description };
            }


            return null;
        }

        /// <summary>
        /// Creates a member information localization.
        /// </summary>
        /// <param name="memberInfo">Information describing the member.</param>
        /// <returns>
        /// The new member information localization.
        /// </returns>
        protected virtual IMemberInfoLocalization CreateMemberInfoLocalization(IElementInfo memberInfo)
        {
            return new MemberInfoLocalization(memberInfo);
        }
    }
}