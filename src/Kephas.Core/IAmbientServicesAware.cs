// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientServicesAware.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAmbientServicesAware interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    /// <summary>
    /// Interface for components being aware of the ambient services within they live.
    /// </summary>
    public interface IAmbientServicesAware
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        IAmbientServices AmbientServices { get; }
    }
}