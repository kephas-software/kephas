// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestExceptionHandlerIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using FluentAssertions;
using Kephas.ExceptionHandling;
using Kephas.Testing;
using Kephas.Web.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.ExceptionHandling;

public class RequestExceptionHandlerIntegrationTest() : IntegrationTestBase<RequestExceptionHandlerIntegrationTest>(includeTestAssembly: true)
{
    [Fact]
    public async Task HandleException_InvalidOperationException()
    {
        var handler = ServiceProvider.GetRequiredService<IRequestExceptionHandler>();

        handler.Should().BeOfType<RequestExceptionHandler>();

        var sb = new StringBuilder(); 
        var contextMock = Substitute.For<HttpContext>();
        await handler.HandleException(
            new HttpExceptionContext(
                new InvalidOperationException("test"),
                "/api/test",
                contextMock),
            (status, response) => Task.FromResult(sb.Append(status).Append(" ").Append(response)));
        
        sb.ToString().Should().StartWith("500 {\"statusCode\":500,\"url\":\"/api/test\",\"correlationId\":\"");
        sb.ToString().Should().EndWith("\",\"errorType\":\"InvalidOperationException\",\"message\":\"test\",\"severity\":4}");
    }

    [Fact]
    public async Task HandleException_UnauthorizedAccessException()
    {
        var handler = ServiceProvider.GetRequiredService<IRequestExceptionHandler>();

        handler.Should().BeOfType<RequestExceptionHandler>();

        var sb = new StringBuilder(); 
        var contextMock = Substitute.For<HttpContext>();
        await handler.HandleException(
            new HttpExceptionContext(
                new UnauthorizedAccessException("test"),
                "/api/test",
                contextMock),
            (status, response) => Task.FromResult(sb.Append(status).Append(" ").Append(response)));

        var expectedStart = "401 {\"statusCode\":401,\"url\":\"/api/test\",\"correlationId\"";
        var expectedEnd = "\",\"message\":\"Unauthorized while accessing \\u0027/api/test\\u0027.\",\"severity\":2}";
        sb.ToString().Should().StartWith(expectedStart);
        sb.ToString().Should().EndWith(expectedEnd);
    }

    [Fact]
    public async Task HandleException_AppException()
    {
        var handler = ServiceProvider.GetRequiredService<IRequestExceptionHandler>();

        handler.Should().BeOfType<RequestExceptionHandler>();

        var sb = new StringBuilder(); 
        var contextMock = Substitute.For<HttpContext>();
        await handler.HandleException(
            new HttpExceptionContext(
                new AppException("test"),
                "/api/test",
                contextMock),
            (status, response) => Task.FromResult(sb.Append(status).Append(" ").Append(response)));

        sb.ToString().Should().StartWith("500 {\"statusCode\":500,\"url\":\"/api/test\",\"correlationId\":\"");
        sb.ToString().Should().EndWith(",\"message\":\"test\",\"severity\":4}");
    }

    [Fact]
    public async Task HandleException_NotImplementedException()
    {
        var handler = ServiceProvider.GetRequiredService<IRequestExceptionHandler>();

        handler.Should().BeOfType<RequestExceptionHandler>();

        var sb = new StringBuilder(); 
        var contextMock = Substitute.For<HttpContext>();
        await handler.HandleException(
            new HttpExceptionContext(
                new NotImplementedException(),
                "/api/test",
                contextMock),
            (status, response) => Task.FromResult(sb.Append(status).Append(" ").Append(response)));

        var expectedStart = "501 {\"url\":\"/api/test\",\"correlationId\":\"";
        var expectedEnd = ",\"message\":\"An internal server error occurred while accessing \\u0027/api/test\\u0027. Please contact the development department to fix the missing functionality.\",\"severity\":4}";
        sb.ToString().Should().EndWith(expectedEnd);
    }
    
    [Fact]
    public async Task HandleException_generic()
    {
        var handler = ServiceProvider.GetRequiredService<IRequestExceptionHandler>();

        var sb = new StringBuilder(); 
        var contextMock = Substitute.For<HttpContext>();
        await handler.HandleException(
            new HttpExceptionContext(
                new InvalidOperationException("test"),
                "/api/test",
                contextMock),
            (status, response) => Task.FromResult(sb.Append(status).Append(" ").Append(response)));
        
        sb.ToString().Should().StartWith("500 {\"statusCode\":500,\"url\":\"/api/test\",\"correlationId\":");
        sb.ToString().Should().EndWith(",\"errorType\":\"InvalidOperationException\",\"message\":\"test\",\"severity\":4}");
    }

    [Fact]
    public async Task HandleException_specific()
    {
        var handler = ServiceProvider.GetRequiredService<IRequestExceptionHandler>();

        var sb = new StringBuilder(); 
        var contextMock = Substitute.For<HttpContext>();
        await handler.HandleException(
            new HttpExceptionContext(
                new InvalidOperationException("test"),
                "/api/test",
                contextMock),
            (status, response) => Task.FromResult(sb.Append(status).Append(" ").Append(response)));
        
        sb.ToString().Should().StartWith("500 {\"statusCode\":500,\"url\":\"/api/test\",\"correlationId\":");
        sb.ToString().Should().EndWith("\",\"errorType\":\"InvalidOperationException\",\"message\":\"test\",\"severity\":4}");
    }

    internal class InvalidOperationExceptionResultProvider : IExceptionResultProvider<InvalidOperationException>
    {
        public ExceptionResult? TryGetExceptionResult(InvalidOperationException exception, IExceptionContext exceptionContext)
        {
            return new ExceptionResult {Message = "Invalid"};
        }
    }
}
