// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceLifetime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Enumerates the lifetime values for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Enumerates the lifetime values for application services.
    /// </summary>
    public enum AppServiceLifetime
    {
        /// <summary>
        /// The application service is shared (default).
        /// </summary>
        Singleton,

        /// <summary>
        /// The application service in instantiated with every request.
        /// </summary>
        Transient,

        /// <summary>
        /// The application service is shared within a scope.
        /// </summary>
        Scoped,
    }
}