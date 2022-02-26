// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserPasswordHashCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication;

/// <summary>
/// Credentials for user and password hash.
/// </summary>
/// <seealso cref="ICredentials" />
public interface IUserPasswordHashCredentials : ICredentials
{
    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; }

    /// <summary>
    /// Gets the password hash.
    /// </summary>
    /// <value>
    /// The password hash.
    /// </value>
    public byte[] PasswordHash { get; }
}