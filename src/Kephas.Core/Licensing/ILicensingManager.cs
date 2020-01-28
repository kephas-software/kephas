// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILicensingManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Interface for licensing manager.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ILicensingManager
    {
        /// <summary>
        /// Gets the app licensing state.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <returns>
        /// The licensing state.
        /// </returns>
        ILicensingState GetLicensingState(AppIdentity appId);
    }
}
