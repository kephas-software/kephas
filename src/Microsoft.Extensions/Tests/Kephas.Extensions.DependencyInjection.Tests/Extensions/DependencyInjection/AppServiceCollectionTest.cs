// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Builder;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Tests.Extensions.DependencyInjection;

public class AppServiceCollectionTest
{
    [Fact]
    public void AppServiceCollection_singleton()
    {
        var services = new ServiceCollection();
        var appServices = new AppServiceCollection();
        var provider = services
            .AddAppServices(new AppServiceCollectionBuilder(appServices))
            .BuildServiceProvider();
        var otherAppServices =  provider.GetRequiredService<IAppServiceCollection>();

        Assert.Same(appServices, otherAppServices);
    }
}
