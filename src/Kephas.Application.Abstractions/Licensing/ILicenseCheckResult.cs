// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILicenseCheckResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILicensingState interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using Kephas.Application;
    using Kephas.Operations;

    /// <summary>
    /// Interface for license check result.
    /// </summary>
    public interface ILicenseCheckResult : IOperationResult<bool>
    {
        /// <summary>
        /// Gets the app identifier.
        /// </summary>
        /// <value>
        /// The app identifier.
        /// </value>
        AppIdentity AppId { get; }

        /// <summary>
        /// Gets a value indicating whether this app is licensed.
        /// </summary>
        /// <value>
        /// True if this app is licensed, false if not.
        /// </value>
        bool IsLicensed { get; }
    }
}
