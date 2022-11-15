// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationProfilesPropertyValues.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using global::IdentityServer4.Models;

    /// <summary>
    /// Constants for special values defined for specific <see cref="ApplicationProfilesPropertyNames" /> keys.
    /// </summary>
    public static class ApplicationProfilesPropertyValues
    {
        /// <summary>
        /// The value given to <see cref="ApplicationProfilesPropertyNames.Clients"/> in <see cref="Resource.Properties"/> to indicate that the
        /// resource can be accessed by all configured clients.
        /// </summary>
        public const string AllowAllApplications = "*";

        /// <summary>
        /// The value given to <see cref="ApplicationProfilesPropertyNames.Source"/> in <see cref="Resource.Properties"/> or <see cref="Client.Properties"/>
        /// to indicate that the application was defined in configuration.
        /// </summary>
        public const string Configuration = nameof(Configuration);

        /// <summary>
        /// The value given to <see cref="ApplicationProfilesPropertyNames.Source"/> in <see cref="Resource.Properties"/>
        /// to indicate that the resource was defined as a default identity resource.
        /// </summary>
        public const string Default = nameof(Default);
    }
}