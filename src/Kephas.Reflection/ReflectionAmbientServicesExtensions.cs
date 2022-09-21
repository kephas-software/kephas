// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionAmbientServicesExtensions.cs" company="Kephas Software SRL">
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
    /// Extension methods for <see cref="IAmbientServices"/> related to reflection.
    /// </summary>
    public static class ReflectionAmbientServicesExtensions
    {
        /// <summary>
        /// Gets the runtime type registry.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The runtime type registry.
        /// </returns>
        public static IRuntimeTypeRegistry GetTypeRegistry(this IAmbientServices ambientServices) =>
            (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices))).GetServiceInstance<IRuntimeTypeRegistry>();
    }
}