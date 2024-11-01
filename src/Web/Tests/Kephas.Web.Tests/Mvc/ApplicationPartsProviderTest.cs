// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationPartsProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Kephas.Runtime;
using Kephas.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.Mvc;

public class ApplicationPartsProviderTest
{
    [Fact]
    public void GetApplicationParts_ReturnsExpectedParts()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        var assembly = typeof(ApplicationPartsProvider).Assembly;
        var assemblies = new[] { assembly };
        appRuntime.GetAppAssemblies().Returns(assemblies);
        var provider = new ApplicationPartsProvider(appRuntime);

        var parts = provider.GetApplicationParts();

        parts.Should().HaveCount(assemblies.Length);
        parts.Cast<AssemblyPart>().Select(p => p.Assembly).Should().Equal([assembly]);
    }

    [Fact]
    public void GetApplicationParts_ReturnsEmptyWhenNoAssemblies()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        appRuntime.GetAppAssemblies().Returns(Enumerable.Empty<Assembly>());
        var provider = new ApplicationPartsProvider(appRuntime);

        var parts = provider.GetApplicationParts();

        parts.Should().BeEmpty();
    }
}