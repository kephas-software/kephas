// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthenticationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication
{
    using Kephas.Services;

    /// <summary>
    /// Interface for authentication context.
    /// </summary>
    public interface IAuthenticationContext : IContext
    {
        /// <summary>
        /// Gets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        bool ThrowOnFailure { get; }
    }
}