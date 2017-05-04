// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IIdentityProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security
{
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Application service contract providing the current identity.
    /// </summary>
    [AppServiceContract]
    public interface IIdentityProvider
    {
        /// <summary>
        /// Gets the current identity.
        /// </summary>
        /// <returns>
        /// The current identity.
        /// </returns>
        IIdentity GetCurrentIdentity();
    }
}