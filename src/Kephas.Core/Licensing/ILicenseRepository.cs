// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILicenseRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILicenseRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System.Collections.Generic;

    using Kephas.Application;

    /// <summary>
    /// Interface for license repository.
    /// </summary>
    public interface ILicenseRepository
    {
        /// <summary>
        /// Gets the matching license information from the store.
        /// </summary>
        /// <param name="appIdentity">The app identity requesting the license.</param>
        /// <returns>
        /// An enumeration of possibly matching license data.
        /// </returns>
        IEnumerable<LicenseData> GetLicenseData(AppIdentity appIdentity);

        /// <summary>
        /// Stores the license data, making it persistable among multiple application runs.
        /// </summary>
        /// <param name="appIdentity">The app identity being associated the license.</param>
        /// <param name="rawLicenseData">Raw information describing the license.</param>
        void StoreRawLicenseData(AppIdentity appIdentity, string rawLicenseData);
    }
}
