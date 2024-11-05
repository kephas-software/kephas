// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderLoggingExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Web.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.Hosting;

public class WebHostBuilderLoggingExtensionsTest
{
    [Fact]
    public void UseWebLogging_no_further_configuration()
    {
        var hostBuilder = Substitute.For<IHostBuilder>();
        hostBuilder.UseWebLogging();
        
        hostBuilder.Received(3).ConfigureServices(Arg.Any<Action<HostBuilderContext, IServiceCollection>>());
    }
    
    [Fact]
    public void UseWebLogging_additional_configuration()
    {
        var hostBuilder = Substitute.For<IHostBuilder>();
        hostBuilder.UseWebLogging((_, _, _) => { });
        
        hostBuilder.Received(3).ConfigureServices(Arg.Any<Action<HostBuilderContext, IServiceCollection>>());
    }    
}