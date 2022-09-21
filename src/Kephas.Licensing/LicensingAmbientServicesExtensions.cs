// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using Kephas.Application;
using Kephas.Cryptography;
using Kephas.Licensing;

/// <summary>
/// Extension methods for <see cref="IAmbientServices"/> in the licensing area.
/// </summary>
public static class LicensingAmbientServicesExtensions
{
    /// <summary>
    /// Gets the licensing manager.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <returns>
    /// The licensing manager.
    /// </returns>
    public static ILicensingManager GetLicensingManager(this IAmbientServices ambientServices) =>
        (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices)))
        .GetServiceInstance<ILicensingManager>();

    /// <summary>
    /// Sets the licensing manager to the ambient services.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="licensingManager">The licensing manager.</param>
    /// <returns>
    /// This <paramref name="ambientServices"/>.
    /// </returns>
    public static IAmbientServices WithLicensingManager(
        this IAmbientServices ambientServices,
        ILicensingManager licensingManager)
    {
        ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
        licensingManager = licensingManager ?? throw new ArgumentNullException(nameof(licensingManager));

        ambientServices.Register(licensingManager);
        ambientServices.GetAppRuntime()
            .OnCheckLicense((appid, context) => licensingManager.CheckLicense(appid, context));

        return ambientServices;
    }

    /// <summary>
    /// Sets the default licensing manager to the ambient services.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="encryptionService">The encryption service.</param>
    /// <returns>
    /// This <paramref name="ambientServices"/>.
    /// </returns>
    public static IAmbientServices WithDefaultLicensingManager(
        this IAmbientServices ambientServices,
        IEncryptionService encryptionService)
    {
        ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
        encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));

        const string LicenseRepositoryKey = "__LicenseRepository";
        var licenseRepository = (ambientServices[LicenseRepositoryKey] as ILicenseStore)
                                ?? (ILicenseStore)(ambientServices[LicenseRepositoryKey] =
                                    new LicenseStore(ambientServices.GetAppRuntime(), encryptionService));
        var licenseManager = new DefaultLicensingManager(appid => licenseRepository.GetLicenseData(appid));

        return WithLicensingManager(ambientServices, licenseManager);
    }
}