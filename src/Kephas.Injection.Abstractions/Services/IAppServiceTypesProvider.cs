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
    /// Interface providing the <see cref="GetAppServiceTypes()"/> method,
    /// which collects the types implementing application service contracts.
    /// </summary>
    public interface IAppServiceTypesProvider
    {
        /// <summary>
        /// Gets an enumeration of types implementing application service contracts.
        /// </summary>
        /// <returns>
        /// An enumeration of types implementing application service contracts.
        /// </returns>
        IEnumerable<Type> GetAppServiceTypes();
    }
}
