// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null licensing manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// A null licensing manager.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullLicensingManager : ILicensingManager
    {
        /// <summary>
        /// Gets the app licensing state.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <returns>
        /// The licensing state.
        /// </returns>
        public ILicensingState GetLicensingState(AppIdentity appId)
        {
            return new LicensingState(appId, true);
        }
    }
}
