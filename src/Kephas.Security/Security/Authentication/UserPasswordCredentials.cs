﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPasswordCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the password authentication context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication;

using Kephas.Dynamic;

/// <summary>
/// User and encrypted password based credentials.
/// </summary>
public class UserPasswordCredentials : Expando, IUserPasswordCredentials
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserPasswordCredentials"/> class.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The user encrypted password.</param>
    public UserPasswordCredentials(string userName, string password)
    {
        this.UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        this.Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; }

    /// <summary>
    /// Gets the encrypted password.
    /// </summary>
    /// <value>
    /// The encrypted password.
    /// </value>
    public string Password { get; }
}