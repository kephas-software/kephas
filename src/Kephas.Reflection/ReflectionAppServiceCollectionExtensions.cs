// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionAppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for <see cref="IAppServiceCollection"/> related to reflection.
    /// </summary>
    public static class ReflectionAppServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the runtime type registry.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <returns>
        /// The runtime type registry.
        /// </returns>
        public static IRuntimeTypeRegistry GetTypeRegistry(this IAppServiceCollection appServices) =>
            (appServices ?? throw new ArgumentNullException(nameof(appServices))).GetServiceInstance<IRuntimeTypeRegistry>();
    }
}