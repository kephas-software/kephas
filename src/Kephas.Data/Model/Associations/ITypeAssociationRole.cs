// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeAssociationRole.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using Kephas.Reflection;

    /// <summary>
    /// Interface for type association role.
    /// </summary>
    public interface ITypeAssociationRole : IElementInfo
    {
        /// <summary>
        /// Gets the type participating in the association.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        ITypeInfo Type { get; }

        /// <summary>
        /// Gets the property in the type connecting the two associated types.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        IPropertyInfo? Property { get; }

        /// <summary>
        /// Gets the referenced type.
        /// </summary>
        /// <value>
        /// The referenced type.
        /// </value>
        ITypeInfo RefType { get; }

        /// <summary>
        /// Gets the role kind.
        /// </summary>
        /// <value>
        /// The role kind.
        /// </value>
        TypeAssociationKind RoleKind { get; }

        /// <summary>
        /// Gets the multiplicity value.
        /// </summary>
        TypeAssociationMultiplicity Multiplicity { get; }

        /// <summary>
        /// Gets a value indicating whether this role indicates a collection (multiplicity > 1).
        /// </summary>
        /// <returns>
        /// True if this role indicates a collection, false if not.
        /// </returns>
        bool IsCollection() => this.Multiplicity == TypeAssociationMultiplicity.Many;
    }
}