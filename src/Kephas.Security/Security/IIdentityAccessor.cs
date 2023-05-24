// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityAccessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Principal;
using Kephas.Services;

namespace Kephas.Security
{
    /// <summary>
    /// Accessor service for the scoped identity.
    /// </summary>
    [ScopedAppServiceContract]
    public interface IIdentityAccessor
    {
        /// <summary>
        /// Gets the identity.
        /// </summary>
        IIdentity? Identity { get; }
    }
}