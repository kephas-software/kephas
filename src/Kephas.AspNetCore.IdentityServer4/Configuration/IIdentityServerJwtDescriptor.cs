// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityServerJwtDescriptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Service contract for a provider of JWT resource settings.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IIdentityServerJwtDescriptor
    {
        /// <summary>
        /// Gets the resource settings.
        /// </summary>
        /// <returns>A dictionary containing the resource settings.</returns>
        IDictionary<string, ResourceSettings> GetResourceSettings();
    }
}
