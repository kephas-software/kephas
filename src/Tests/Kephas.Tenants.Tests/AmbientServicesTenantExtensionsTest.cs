// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTenantExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tenants.Tests;

using Kephas.Application;
using NUnit.Framework;

[TestFixture]
public class AmbientServicesTenantExtensionsTest
{
    [Test]
    public void WithTenantSupport_no_tenant()
    {
        var ambientServices = new AmbientServices()
            .WithTenantSupport(new AppArgs("--no tenant"))
            .WithStaticAppRuntime();
        var configLocations = ambientServices.GetAppRuntime()!.GetAppConfigLocations();
        var licenseLocations = ambientServices.GetAppRuntime()!.GetAppLicenseLocations();

        Assert.IsTrue(configLocations.All(l => l.EndsWith("config")));
        Assert.IsTrue(licenseLocations.All(l => l.EndsWith("licenses")));
    }

    [Test]
    public void WithTenantSupport_with_tenant()
    {
        var ambientServices = new AmbientServices()
            .WithTenantSupport(new AppArgs("--tenant my"))
            .WithStaticAppRuntime();
        var configLocations = ambientServices.GetAppRuntime()!.GetAppConfigLocations();
        var licenseLocations = ambientServices.GetAppRuntime()!.GetAppLicenseLocations();

        Assert.IsTrue(configLocations.All(l => l.EndsWith(Path.Combine("config", ".my"))));
        Assert.IsTrue(licenseLocations.All(l => l.EndsWith(Path.Combine("licenses", ".my"))));
    }
}