// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderLoggingExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Hosting;

public class HostBuilderLoggingExtensionsTest
{
    [Fact]
    public void UseLogging_no_further_configuration()
    {
        var hostBuilder = Substitute.For<IHostBuilder>();
        hostBuilder.UseLogging();
        
        hostBuilder.Received(2).ConfigureServices(Arg.Any<Action<HostBuilderContext, IServiceCollection>>());
    }
    
    [Fact]
    public void UseLogging_additional_configuration()
    {
        var hostBuilder = Substitute.For<IHostBuilder>();
        hostBuilder.UseLogging((_, _, _) => { });
        
        hostBuilder.Received(2).ConfigureServices(Arg.Any<Action<HostBuilderContext, IServiceCollection>>());
    }
}