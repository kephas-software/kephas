// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullIdentityProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null identity provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security
{
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// A null identity provider.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullIdentityProvider : IIdentityProvider
    {
        /// <summary>
        /// Gets the current identity.
        /// </summary>
        /// <returns>
        /// The current identity.
        /// </returns>
        public IIdentity GetCurrentIdentity()
        {
            return null;
        }
    }
}