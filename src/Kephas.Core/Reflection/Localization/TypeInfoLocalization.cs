// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfoLocalization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type information localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;

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
            protected internal set
            {
                Requires.NotNull(value, nameof(value));

                this.members = value;
            }
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