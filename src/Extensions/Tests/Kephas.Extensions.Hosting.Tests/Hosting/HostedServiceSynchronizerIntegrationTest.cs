// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostedServiceSynchronizerIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.Extensions.Hosting;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Reflection;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Xunit;

namespace Kephas.Extensions.Tests.Hosting;

public class HostedServiceSynchronizerIntegrationTest
{
    [Fact]
    public async Task EnterStartAsync_CircularDependency()
    {
        var synchronizer = CreateSynchronizer(
            typeof(AneedsBHostedService),
            typeof(BneedsCHostedService),
            typeof(CneedsAHostedService));
        var action = () => synchronizer.EnterStartAsync(new AneedsBHostedService(synchronizer));
        await action.Should().ThrowAsync<CircularDependencyException>().WithMessage("*AneedsBHostedService*");
    }
    
    [Fact]
    public async Task EnterStopAsync_CircularDependency()
    {
        var synchronizer = CreateSynchronizer(
            typeof(AneedsBHostedService),
            typeof(BneedsCHostedService),
            typeof(CneedsAHostedService));
        var action = () => synchronizer.EnterStopAsync(new AneedsBHostedService(synchronizer));
        await action.Should().ThrowAsync<CircularDependencyException>().WithMessage("*AneedsBHostedService*");
    }

    [Fact]
    public async Task EnterStartAsync_MissingRequired()
    {
        var synchronizer = CreateSynchronizer(
            typeof(AneedsBHostedService));
        var action = () => synchronizer.EnterStartAsync(new AneedsBHostedService(synchronizer));
        await action.Should().ThrowAsync<ServiceException>().WithMessage("*AneedsBHostedService*");
    }

    [Fact]
    public async Task EnterStopAsync_MissingRequired()
    {
        var synchronizer = CreateSynchronizer(
            typeof(AneedsBHostedService));
        var action = () => synchronizer.EnterStopAsync(new AneedsBHostedService(synchronizer));
        await action.Should().ThrowAsync<ServiceException>().WithMessage("*AneedsBHostedService*");
    }
    
    [Fact]
    public async Task EnterStartAsync_DefaultFlow()
    {
        var synchronizer = CreateSynchronizer(
            typeof(AneedsBHostedService),
            typeof(BneedsCHostedService),
            typeof(CneedsNoneHostedService));
        var logger = Test.GetTestLogger<AneedsBHostedService>();
        await synchronizer.EnterStartAsync(new AneedsBHostedService(synchronizer, logger));
        logger.ToString().Should().BeEmpty();
        await synchronizer.EnterStartAsync(new CneedsNoneHostedService(synchronizer, logger));
        logger.ToString()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Should().BeEquivalentTo("Information: CNone started");
        await synchronizer.EnterStartAsync(new BneedsCHostedService(synchronizer, logger));
        logger.ToString()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Should().BeEquivalentTo(
                "Information: CNone started",
                "Information: B started",
                "Information: A started");
    }

    [Fact]
    public async Task EnterStopAsync_DefaultFlow()
    {
        var synchronizer = CreateSynchronizer(
            typeof(AneedsBHostedService),
            typeof(BneedsCHostedService),
            typeof(CneedsNoneHostedService));
        var logger = Test.GetTestLogger<AneedsBHostedService>();
        await synchronizer.EnterStopAsync(new CneedsNoneHostedService(synchronizer, logger));
        logger.ToString().Should().BeEmpty();
        await synchronizer.EnterStopAsync(new BneedsCHostedService(synchronizer, logger));
        logger.ToString().Should().BeEmpty();
        await synchronizer.EnterStopAsync(new AneedsBHostedService(synchronizer, logger));
        logger.ToString()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Should().BeEquivalentTo(
                "Information: A stopped",
                "Information: B stopped",
                "Information: CNone stopped");
    }

    private IHostedServiceSynchronizer CreateSynchronizer(params Type[] hostedServiceTypes)
    {
        var services = new ServiceCollection()
            .AddIntegrationLogging(Test.GetIntegrationConfig());
        var serviceProvider = services.BuildTestServiceProvider<HostedServiceSynchronizerIntegrationTest>(
            builderConfig: b => b.ConfigureBuilder = cb => cb.AddTypes(hostedServiceTypes));
        return serviceProvider.GetRequiredService<IHostedServiceSynchronizer>();
    }
}