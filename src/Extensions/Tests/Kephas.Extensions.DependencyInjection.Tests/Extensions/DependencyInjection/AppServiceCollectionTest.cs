// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.ServiceProviders;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Tests.Extensions.DependencyInjection;

public class AppServiceCollectionTest
{
    [Fact]
    public void AppServiceCollection_singleton()
    {
        var services = new ServiceCollection();
        var builder = new ServiceCollectionBuilder();
        var provider = services
            .AddApplicationServices(builder.Build())
            .BuildServiceProvider();
        var otherAppServices = provider.GetRequiredService<IServiceDeclarationInfoCollection>();

        Assert.Same(appServices, otherAppServices);
    }
}
