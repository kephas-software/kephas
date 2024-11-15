// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestEnricherTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Claims;
using Kephas.Context;
using Kephas.Web.Logging;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Kephas.Web.Tests.Logging;

public class HttpRequestEnricherTest
{
    [Theory]
    [InlineData("/c", 180, "test")]
    public void Enrich(string path, int address, string userName)
    {
        //Arrange
        var mockIHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var mockHttpContext = Substitute.For<HttpContext>();
        var mockHttpRequest = Substitute.For<HttpRequest>();
        var mockConnectionInfo = Substitute.For<ConnectionInfo>();
        var mockClaimsPrincipal = Substitute.For<ClaimsPrincipal>();

        var mockContextAccessor = Substitute.For<IContextAccessor>();
        var correlationId = new CorrelationId();
        mockContextAccessor.TryGetValue<CorrelationId>().Returns(correlationId);

        var enricher = new HttpRequestEnricher(mockIHttpContextAccessor, mockContextAccessor);
        var mockILogEventPropertyFactory = Substitute.For<ILogEventPropertyFactory>();

        var logEventLevel= new LogEventLevel();
        var exception = new Exception();          
             
        mockHttpRequest.Path.Returns((PathString)path);
        mockHttpContext.Request.Returns(mockHttpRequest);

        mockConnectionInfo.RemoteIpAddress.Returns(new System.Net.IPAddress(address));
        mockHttpContext.Connection.Returns(mockConnectionInfo);

        var lstClaim = new List<Claim>();
        var c = new Claim("username", userName);
        lstClaim.Add(c);
        mockClaimsPrincipal.Claims.Returns(lstClaim);
        mockHttpContext.User.Returns(mockClaimsPrincipal);

        mockIHttpContextAccessor.HttpContext.Returns(mockHttpContext);
                     

        var scalarStringA = new LogEventProperty("a", new ScalarValue("a"));
        mockILogEventPropertyFactory.CreateProperty("Uri", mockHttpRequest.Path.ToString(), true).Returns(scalarStringA);
        var mockClaim = mockClaimsPrincipal.Claims?.FirstOrDefault(x => x.Type == "username")?.Value;
        mockILogEventPropertyFactory.CreateProperty("UserName", mockClaim,true).Returns(scalarStringA);
        var clientIP = mockConnectionInfo.RemoteIpAddress?.ToString();
        mockILogEventPropertyFactory.CreateProperty("ClientIP", clientIP, true).Returns(scalarStringA);
        var correlationIdProperty = new LogEventProperty("CorrelationId", new ScalarValue(correlationId));
        mockILogEventPropertyFactory.CreateProperty("CorrelationId", correlationId, true).Returns(correlationIdProperty);

        var mockLogEventPropertyValue = Substitute.For<LogEventPropertyValue>();
        var logEventProperties = new List<LogEventProperty> { new("Test", mockLogEventPropertyValue) };
        var messageTemplate = new MessageTemplate(new List<global::Serilog.Parsing.MessageTemplateToken>());

        var logEvent = new LogEvent(DateTimeOffset.Now, logEventLevel, exception, messageTemplate, logEventProperties);

        //Act
        enricher.Enrich(logEvent, mockILogEventPropertyFactory);

        //Assert
        mockILogEventPropertyFactory.Received(1).CreateProperty("Uri", path, true);
        mockILogEventPropertyFactory.Received(1).CreateProperty("UserName", userName, true);
        mockILogEventPropertyFactory.Received(1).CreateProperty("ClientIP", $"{address}.0.0.0", true);
        mockILogEventPropertyFactory.Received(1).CreateProperty("CorrelationId", correlationId, true);
    }
}