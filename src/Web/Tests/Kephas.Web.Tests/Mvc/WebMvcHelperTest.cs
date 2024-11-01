// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Testing;
using Kephas.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.Mvc;

public class WebMvcHelperTest : IntegrationTestBase<WebMvcHelperTest>
{
    public WebMvcHelperTest()
    {
        Services.ConfigureControllers();
    }
    
    [Fact]
    public void ConfigureControllers_AddsApplicationPartsProvider()
    {
        var partsProvider = ServiceProvider.GetService<IApplicationPartsProvider>();

        partsProvider.Should().NotBeNull();
    }

    [Fact]
    public void ConfigureControllers_AddsCacheProfileWhenNoAllowAnonymousFilter()
    {
        var mvcOptions = ServiceProvider.GetRequiredService<IConfigureOptions<MvcOptions>>();
        var options = new MvcOptions();
        mvcOptions.Configure(options);

        options.CacheProfiles.Should().ContainKey("Default");
        options.CacheProfiles["Default"].Duration.Should().Be(WebMvcHelper.CacheProfileDuration);
    }

    [Fact]
    public void ConfigureApplicationParts_AddsApplicationParts()
    {
        var services = new ServiceCollection();
        var mvcBuilder = services.AddControllers();
        var partsProvider = Substitute.For<IApplicationPartsProvider>();
        partsProvider.GetApplicationParts().Returns(new List<ApplicationPart> { new AssemblyPart(typeof(WebMvcHelper).Assembly) });

        services.AddSingleton(partsProvider);
        mvcBuilder.ConfigureApplicationParts();

        mvcBuilder.PartManager.ApplicationParts.Should().ContainSingle(part => part is AssemblyPart);
    }
}