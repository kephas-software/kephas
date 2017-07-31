// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOResourceService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    [SharedAppServiceContract]
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