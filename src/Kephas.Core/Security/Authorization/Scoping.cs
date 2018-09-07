// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeKind.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scope kind class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    /// <summary>
    /// Values that represent permission scoping.
    /// </summary>
    public enum Scoping
    {
        /// <summary>
        /// No scoping required for this permission type.
        /// </summary>
        None,

        /// <summary>
        /// The scoping is allowed, but not required.
        /// </summary>
        Allowed,

        /// <summary>
        /// The scoping is required.
        /// </summary>
        Required,
    }
}