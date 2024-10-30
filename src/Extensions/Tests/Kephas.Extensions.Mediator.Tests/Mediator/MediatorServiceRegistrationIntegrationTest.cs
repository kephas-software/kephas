// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceRegistrationIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Mediator;
using Kephas.Extensions.Tests.TestServices;
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
    public async Task Send_from_serviceProvider()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(Substitute.For<IRequest<string>>());
        response.Should().Be("OpenGenericBehavior`2 > ReturnStringBehavior > ReturnStringHandler");
    }

    [Fact]
    public async Task Send_exact_match()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(new OneReturnStringTypedMessage());
        response.Should().Be("ReturnStringTypedBehavior > OpenGenericBehavior`2 > OneReturnStringTypedBehavior > ReturnStringBehavior > OneReturnStringTypedHandler");
    }

    [Fact]
    public async Task Send_polymorphic_match()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(new TwoReturnStringTypedMessage());
        response.Should().Be("ReturnStringTypedBehavior > OpenGenericBehavior`2 > ReturnStringBehavior > ReturnStringInterfaceHandler");
    }

    [Fact]
    public async Task Send_no_match()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await FluentActions.Awaiting(() => mediator.Send(Substitute.For<IRequest<object>>()))
            .Should().ThrowAsync<MediatorServiceResolutionException>();
    }
}
