// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for authorization context.
    /// </summary>
    public interface IAuthorizationContext : IContext
    {
        /// <summary>
        /// Gets the required permissions.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        IEnumerable<string> RequiredPermissions { get; }

        /// <summary>
        /// Gets the types of the required permissions.
        /// </summary>
        /// <value>
        /// The types of the required permissions.
        /// </value>
        IEnumerable<Type> RequiredPermissionTypes { get; }

        /// <summary>
        /// Gets the authorization scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        object Scope { get; }

        /// <summary>
        /// Gets a value indicating whether to throw on authorization failure.
        /// If <c>false</c> is indicated, the authorization check will return <c>false</c> upon failure,
        /// otherwise an exception will occur.
        /// </summary>
        /// <value>
        /// True to throw on authorization failure, false to not throw and return <c>false</c>.
        /// </value>
        bool ThrowOnFailure { get; }
    }
}