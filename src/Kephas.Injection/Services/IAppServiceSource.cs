// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Source for getting or producing service instances.
    /// </summary>
    public interface IAppServiceSource
    {
        /// <summary>
        /// Gets a value indicating whether the contract type is handled by the source.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// True if the source handles the contract type, false if not.
        /// </returns>
        bool IsMatch(Type contractType);

        /// <summary>
        /// Gets a service.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// The service instance.
        /// </returns>
        object GetService(IServiceProvider parent, Type contractType);
    }
}