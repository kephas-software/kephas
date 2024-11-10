// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiControllerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.Controllers;

public class ApiControllerBaseTest
{
    private readonly ApiControllerBase controller;
    private readonly HttpContext httpContext;
    private readonly IServiceProvider serviceProvider;

    public ApiControllerBaseTest()
    {
        controller = Substitute.ForPartsOf<ApiControllerBase>();
        httpContext = Substitute.For<HttpContext>();
        serviceProvider = Substitute.For<IServiceProvider>();
        httpContext.RequestServices.Returns(serviceProvider);
        controller.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public void Logger_ReturnsLoggerInstance()
    {
        var logger = Substitute.For<ILogger<ApiControllerBase>>();
        serviceProvider.GetService(typeof(ILogger<>).MakeGenericType(controller.GetType())).Returns(logger);

        var result = controller.Logger;

        result.Should().Be(logger);
    }

    [Fact]
    public void Mediator_ReturnsMediatorInstance()
    {
        var mediator = Substitute.For<IMediator>();
        serviceProvider.GetService(typeof(IMediator)).Returns(mediator);

        var result = controller.Mediator;

        result.Should().Be(mediator);
    }

    [Fact]
    public void Logger_CachesLoggerInstance()
    {
        var logger = Substitute.For<ILogger<ApiControllerBase>>();
        serviceProvider.GetService(typeof(ILogger<>).MakeGenericType(controller.GetType())).Returns(logger);

        var firstCall = controller.Logger;
        var secondCall = controller.Logger;

        firstCall.Should().BeSameAs(secondCall);
    }

    [Fact]
    public void Mediator_CachesMediatorInstance()
    {
        var mediator = Substitute.For<IMediator>();
        serviceProvider.GetService(typeof(IMediator)).Returns(mediator);

        var firstCall = controller.Mediator;
        var secondCall = controller.Mediator;

        firstCall.Should().BeSameAs(secondCall);
    }
}