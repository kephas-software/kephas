// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Interface for service information.
    /// </summary>
    internal interface IServiceInfo : IAppServiceInfo
    {
        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        Type ServiceType { get; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IDictionary<string, object>? Metadata { get; }

        /// <summary>
        /// Makes a generic service information with closed generic types.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="genericArgs">The generic arguments.</param>
        /// <returns>
        /// An IServiceInfo.
        /// </returns>
        IServiceInfo MakeGenericServiceInfo(IAmbientServices ambientServices, Type[] genericArgs);

        /// <summary>
        /// Gets a service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The service.
        /// </returns>
        object GetService(IAmbientServices ambientServices);
    }
}