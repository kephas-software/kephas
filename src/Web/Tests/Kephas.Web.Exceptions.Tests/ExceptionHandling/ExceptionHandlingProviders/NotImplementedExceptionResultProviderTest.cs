// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotImplementedExceptionResultProviderTest.cs" company="Kephas Software SRL">
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

public class NotImplementedExceptionResultProviderTest
{
    [Fact]
    public void TryGetExceptionResult_ReturnsExceptionResult_WithNotImplementedStatusCode()
    {
        var exception = new NotImplementedException();
        var exceptionContext = Substitute.For<IExceptionContext>();
        var provider = new NotImplementedExceptionResultProvider();

        var result = provider.TryGetExceptionResult(exception, exceptionContext)!;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotImplemented);
    }

    [Fact]
    public void TryGetExceptionResult_ReturnsExceptionResult_WithCorrectMessage()
    {
        var exception = new NotImplementedException();
        var exceptionContext = Substitute.For<IExceptionContext>();
        exceptionContext.RequestPath.Returns("/test-path");
        var provider = new NotImplementedExceptionResultProvider();

        var result = provider.TryGetExceptionResult(exception, exceptionContext)!;

        result.Should().NotBeNull();
        result.Message.Should().Be("An internal server error occurred while accessing '/test-path'. Please contact the development department to fix the missing functionality.");
    }
}