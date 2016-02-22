// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceLifetime.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        Shared,

        /// <summary>
        /// The application service in instanciated on every request.
        /// </summary>
        Instance,

        /// <summary>
        /// The application service is shared within a scope.
        /// </summary>
        ScopeShared,
    }
}