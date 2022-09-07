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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Operations;
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
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The licensing state.
        /// </returns>
        public IOperationResult<bool> CheckLicense(AppIdentity appId, IContext? context = null) =>
            new OperationResult<bool>(true);

        /// <summary>
        /// Gets the license for the provided application identity.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        public LicenseData? GetLicense(AppIdentity appIdentity, IContext? context = null) =>
            this.GetLicenseData(appIdentity);

        private LicenseData? GetLicenseData(AppIdentity appIdentity)
        {
            return new LicenseData(
                Guid.NewGuid().ToString(),
                appIdentity.Id,
                "*",
                "<null>",
                "<null>",
                "<null>");
        }
    }
}
