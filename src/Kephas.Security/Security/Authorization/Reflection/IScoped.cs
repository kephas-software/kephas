// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScoped.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Reflection
{
    /// <summary>
    /// Provides the <see cref="Scoping"/> property.
    /// </summary>
    public interface IScoped
    {
        /// <summary>
        /// Gets the scoping.
        /// </summary>
        /// <value>
        /// The scoping.
        /// </value>
        Scoping Scoping { get; }
    }
}