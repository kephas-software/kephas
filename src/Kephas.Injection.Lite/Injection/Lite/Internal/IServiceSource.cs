// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceSource interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for service source.
    /// </summary>
    internal interface IServiceSource : IAppServiceSource
    {
        /// <summary>
        /// Gets the service descriptors.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the service descriptors in this
        /// collection.
        /// </returns>
        IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(IServiceProvider parent, Type serviceType);
    }
}