// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionResultFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Kephas.Web.ExceptionHandling;
using NSubstitute;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Web.Tests.ExceptionHandling;

public class ExceptionResultFactoryTest : IntegrationTestBase<ExceptionResultFactoryTest>
{
    [Fact]
    public void GetExceptionResult_ReturnsExceptionResult_WithCorrectProvider()
    {
        var exception = new NotImplementedException();
        var exceptionContext = Substitute.For<IExceptionContext>();
        exceptionContext.Exception.Returns(exception);
        exceptionContext.RequestPath.Returns("/api/test");
        var factory = ServiceProvider.GetRequiredService<IExceptionResultFactory>();

        var result = factory.GetExceptionResult(exceptionContext);
        result.Message.Should().Be("An internal server error occurred while accessing '/api/test'. Please contact the development department to fix the missing functionality.");
        result.ErrorType.Should().Be(null);
        result.StatusCode.Should().Be((int)HttpStatusCode.NotImplemented);
    }

    [Fact]
    public void GetExceptionResult_ReturnsGenericExceptionResult_WhenNoProviderFound()
    {
        var exception = new Exception("Test exception");
        var exceptionContext = Substitute.For<IExceptionContext>();
        exceptionContext.Exception.Returns(exception);
        var factory = ServiceProvider.GetRequiredService<IExceptionResultFactory>();

        var result = factory.GetExceptionResult(exceptionContext);
        result.Message.Should().Be("Test exception");
        result.ErrorType.Should().Be("Exception");
        result.StatusCode.Should().Be(null);
    }
}