// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssociation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// An entity relationship.
    /// </summary>
    public class TypeAssociation : DynamicElementInfo, ITypeAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAssociation"/> class.
        /// </summary>
        /// <param name="role1">The first role.</param>
        /// <param name="role2">Optional. The second role.</param>
        public TypeAssociation(ITypeAssociationRole role1, ITypeAssociationRole? role2 = null)
        {
            if (role2 == null)
            {
                role2 = new TypeAssociationRole(role1.RefType)
                            {
                                RoleKind = TypeAssociationKind.Simple,
                                Multiplicity = !role1.IsCollection() && role1.RoleKind == TypeAssociationKind.Simple
                                    ? TypeAssociationMultiplicity.Many
                                    : TypeAssociationMultiplicity.One,
                                RefType = role1.Type,
                            };
            }

            if (role1.RoleKind != TypeAssociationKind.Simple || role1.IsCollection())
            {
                this.PrimaryRole = role1;
                this.DependentRole = role2;
            }
            else if (role2.RoleKind != TypeAssociationKind.Simple || role2.IsCollection())
            {
                this.PrimaryRole = role2;
                this.DependentRole = role1;
            }
            else
            {
                this.PrimaryRole = role1;
                this.DependentRole = role2;
            }

            this.Name = $"{this.PrimaryRole.Name}_{this.DependentRole.Name} ({this.DependentRole.Type.Name} -> {this.PrimaryRole.Type.Name})";
            this.Kind = this.PrimaryRole.RoleKind;
        }

        /// <summary>
        /// Gets the primary role.
        /// </summary>
        /// <value>
        /// The primary role.
        /// </value>
        public ITypeAssociationRole PrimaryRole { get; }

        /// <summary>
        /// Gets the dependent role.
        /// </summary>
        /// <value>
        /// The dependent role.
        /// </value>
        public ITypeAssociationRole DependentRole { get; }

        /// <summary>
        /// Gets the association kind.
        /// </summary>
        /// <value>
        /// The association kind.
        /// </value>
        public TypeAssociationKind Kind { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.Kind})";
        }
    }
}