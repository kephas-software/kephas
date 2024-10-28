// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceRegistrationIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.Mediator;
using Kephas.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Mediator;

public class MediatorServiceRegistrationIntegrationTest()
    : IntegrationTestBase<MediatorServiceRegistrationIntegrationTest>(includeTestAssembly: true)
{
    [Fact]
    public void GetRequiredService()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        mediator.Should().BeOfType<MediatorService>();
    }

    [Fact]
    public async Task Send_from_serviceProvider()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(Substitute.For<IRequest<string>>());
        Assert.Equal("RequestOfStringBehavior -> RequestOfStringHandler", response);
    }

    [Fact]
    public async Task Send_exact_match()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(new TestMessage());
        Assert.Equal("TestInterfaceBehavior -> TestBehavior -> RequestOfStringBehavior -> TestHandler", response);
    }

    [Fact]
    public async Task Send_polymorphic_match()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(new AnotherTestMessage());
        Assert.Equal("TestInterfaceBehavior -> RequestOfStringBehavior -> TestInterfaceHandler", response);
    }

    [Fact]
    public async Task Send_no_match()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await Assert.ThrowsAsync<MediatorServiceResolutionException>(() => mediator.Send(Substitute.For<IRequest<object>>()));
    }
    
    public interface ITestMessage : IRequest<string> {}
    public class TestMessage : ITestMessage {}
    public class AnotherTestMessage : ITestMessage {}

    public class RequestOfStringHandler : RequestHandlerBase<IRequest<string>, string>
    {
        public override Task<string> Handle(IRequest<string> request, CancellationToken cancellationToken) =>
            Task.FromResult(GetType().Name);
    }

    [Priority(Priority.High)]
    internal class TestInterfaceHandler : RequestHandlerBase<ITestMessage, string>
    {
        public override Task<string> Handle(ITestMessage message, CancellationToken cancellationToken) =>
            Task.FromResult(GetType().Name);
    }

    public class TestHandler : RequestHandlerBase<TestMessage, string>
    {
        public override Task<string> Handle(TestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(GetType().Name);
    }

    [Priority(Priority.High)]
    public class TestInterfaceBehavior : PipelineBehaviorBase<ITestMessage, string>
    {
        public override async Task<string> Handle(ITestMessage request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
            $"{GetType().Name} -> {await next()}";
    }

    public class TestBehavior : PipelineBehaviorBase<TestMessage, string>
    {
        public override async Task<string> Handle(TestMessage request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
            $"{GetType().Name} -> {await next()}";
    }

    [Priority(Priority.Lowest)]
    internal class RequestOfStringBehavior : PipelineBehaviorBase<IRequest<string>, string>
    {
        public override async Task<string> Handle(IRequest<string> request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
            $"{GetType().Name} -> {await next()}";
    }
}
