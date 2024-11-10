// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOResourceService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataIOResourceService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Interface for data I/O resource service.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IDataIOResourceService
    {
        /// <summary>
        /// Gets the resource path for the provided identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// The user's resource path.
        /// </returns>
        string GetResourcePath(IIdentity identity);
    }
}