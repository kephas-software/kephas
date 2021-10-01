// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Interface for service information.
    /// </summary>
    internal interface IServiceInfo : IAppServiceInfo
    {
        /// <summary>
        /// Makes a generic service information with closed generic types.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="genericArgs">The generic arguments.</param>
        /// <returns>
        /// An IServiceInfo.
        /// </returns>
        IServiceInfo MakeGenericServiceInfo(IServiceProvider serviceProvider, Type[] genericArgs);

        /// <summary>
        /// Gets a service.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// The service.
        /// </returns>
        object GetService(IServiceProvider serviceProvider);
    }
}