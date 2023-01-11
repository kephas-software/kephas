// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using Kephas.Application;
using Kephas.Cryptography;
using Kephas.Licensing;
using Kephas.Services.Builder;

/// <summary>
/// Extension methods for <see cref="IAppServiceCollectionBuilder"/> in the licensing area.
/// </summary>
public static class LicensingServicesExtensions
{
    /// <summary>
    /// Sets the licensing manager to The application services.
    /// </summary>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <param name="licensingManager">The licensing manager.</param>
    /// <returns>
    /// This <paramref name="servicesBuilder"/>.
    /// </returns>
    public static IAppServiceCollectionBuilder WithLicensingManager(
        this IAppServiceCollectionBuilder servicesBuilder,
        ILicensingManager licensingManager)
    {
        servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));
        licensingManager = licensingManager ?? throw new ArgumentNullException(nameof(licensingManager));

        var appServices = servicesBuilder.AppServices;
        appServices.Add(licensingManager);
        appServices.GetAppRuntime()
            .OnCheckLicense((appid, context) => licensingManager.CheckLicense(appid, context));

        return servicesBuilder;
    }

    /// <summary>
    /// Sets the default licensing manager to The application services.
    /// </summary>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <param name="encryptionService">The encryption service.</param>
    /// <returns>
    /// This <paramref name="servicesBuilder"/>.
    /// </returns>
    public static IAppServiceCollectionBuilder WithDefaultLicensingManager(
        this IAppServiceCollectionBuilder servicesBuilder,
        IEncryptionService encryptionService)
    {
        servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));
        encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));

        const string LicenseRepositoryKey = "__LicenseRepository";
        var appServices = servicesBuilder.AppServices;
        var licenseRepository = (appServices[LicenseRepositoryKey] as ILicenseStore)
                                ?? (ILicenseStore)(appServices[LicenseRepositoryKey] =
                                    new LicenseStore(appServices.GetAppRuntime(), encryptionService));
        var licenseManager = new DefaultLicensingManager(appid => licenseRepository.GetLicenseData(appid));

        return WithLicensingManager(servicesBuilder, licenseManager);
    }
}