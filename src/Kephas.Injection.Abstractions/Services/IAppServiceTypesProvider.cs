// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceTypesProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Interface providing the <see cref="GetAppServiceTypes"/> method,
    /// which collects the types implementing application service contracts.
    /// </summary>
    public interface IAppServiceTypesProvider
    {
        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null);
    }
}
