// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeAssociation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using Kephas.Reflection;

    /// <summary>
    /// Interface for type association.
    /// </summary>
    public interface ITypeAssociation : IElementInfo
    {
        /// <summary>
        /// Gets the primary role.
        /// </summary>
        /// <value>
        /// The primary role.
        /// </value>
        ITypeAssociationRole PrimaryRole { get; }

        /// <summary>
        /// Gets the dependent role.
        /// </summary>
        /// <value>
        /// The dependent role.
        /// </value>
        ITypeAssociationRole DependentRole { get; }

        /// <summary>
        /// Gets the relationship kind.
        /// </summary>
        /// <value>
        /// The relationship kind.
        /// </value>
        TypeAssociationKind Kind { get; }
    }
}