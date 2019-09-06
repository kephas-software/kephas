// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRegistry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;

    internal interface IServiceRegistry : IEnumerable<IServiceInfo>
    {
        IServiceInfo this[Type contractType] { get; }

        IEnumerable<IServiceSource> Sources { get; }

        bool TryGetValue(Type serviceType, out IServiceInfo serviceInfo);

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        bool IsRegistered(Type serviceType);

        ServiceRegistry RegisterService(IServiceInfo serviceInfo);

        ServiceRegistry RegisterSource(IServiceSource serviceSource);
    }
}