// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullIdentityAccessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Principal;
using Kephas.Services;

namespace Kephas.Security;

/// <summary>
/// <see cref="IIdentityAccessor"/> getting a <c>null</c> identity.
/// </summary>
[OverridePriority(Priority.Lowest)]
public class NullIdentityAccessor : IIdentityAccessor
{
    /// <summary>
    /// Gets a <c>null</c> identity.
    /// </summary>
    public IIdentity? Identity => null;
}