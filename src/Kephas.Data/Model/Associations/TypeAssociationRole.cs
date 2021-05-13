// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssociationRole.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// An entity relationship role.
    /// </summary>
    public class TypeAssociationRole : DynamicElementInfo, ITypeAssociationRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAssociationRole"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="property">Optional. The property.</param>
        public TypeAssociationRole(ITypeInfo entityType, IPropertyInfo? property = null)
        {
            this.Type = entityType;
            this.Property = property;
            this.Name = property == null ? "-" : property.Name;
        }

        /// <summary>
        /// Gets the entity type participating in the association.
        /// </summary>
        /// <value>
        /// The entity type.
        /// </value>
        public ITypeInfo Type { get; }

        /// <summary>
        /// Gets the property in the entity type connecting the two types.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public IPropertyInfo? Property { get; }

        /// <summary>
        /// Gets the referenced entity type.
        /// </summary>
        /// <value>
        /// The referenced entity type.
        /// </value>
        public ITypeInfo RefType { get; internal set; }

        /// <summary>
        /// Gets the role kind.
        /// </summary>
        /// <value>
        /// The role kind.
        /// </value>
        public TypeAssociationKind RoleKind { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this role indicates a collection (multiplicity > 1).
        /// </summary>
        /// <value>
        /// True if this role indicates a collection, false if not.
        /// </value>
        public bool IsCollection { get; internal set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.Type.Name})";
        }
    }
}