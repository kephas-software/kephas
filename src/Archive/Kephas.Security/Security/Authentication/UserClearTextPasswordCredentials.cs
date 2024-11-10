// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClearTextPasswordCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authentication;

using Kephas.Dynamic;

/// <summary>
/// User and clear text password based credentials.
/// </summary>
public class UserClearTextPasswordCredentials : Expando, IUserClearTextPasswordCredentials
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserClearTextPasswordCredentials"/> class.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="clearTextPassword">The user clear text password.</param>
    public UserClearTextPasswordCredentials(string userName, string clearTextPassword)
    {
        this.UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        this.ClearTextPassword = clearTextPassword ?? throw new ArgumentNullException(nameof(clearTextPassword));
    }

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; }

    /// <summary>
    /// Gets the clear text password.
    /// </summary>
    /// <value>
    /// The clear text password.
    /// </value>
    public string ClearTextPassword { get; }
}