// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnauthorizedAccessExceptionResultProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Kephas.Web.ExceptionHandling;
using Kephas.Web.ExceptionHandling.ExceptionResultProviders;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.ExceptionHandling.ExceptionHandlingProviders;

public class UnauthorizedAccessExceptionResultProviderTest
{
    [Fact]
    public void TryGetExceptionResult_ReturnsExceptionResult_WithUnauthorizedStatusCode()
    {
        var exception = new UnauthorizedAccessException();
        var exceptionContext = Substitute.For<IExceptionContext>();
        var provider = new UnauthorizedAccessExceptionResultProvider();

        var result = provider.TryGetExceptionResult(exception, exceptionContext)!;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void TryGetExceptionResult_ReturnsExceptionResult_WithCorrectMessage()
    {
        var exception = new UnauthorizedAccessException();
        var exceptionContext = Substitute.For<IExceptionContext>();
        exceptionContext.RequestPath.Returns("/test-path");
        var provider = new UnauthorizedAccessExceptionResultProvider();

        var result = provider.TryGetExceptionResult(exception, exceptionContext)!;

        result.Should().NotBeNull();
        result.Message.Should().Be("Unauthorized while accessing '/test-path'.");
    }
}