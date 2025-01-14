// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestEnricherTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Claims;
using Kephas.Context;
using Kephas.Diagnostics;
using Kephas.Web.Logging;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Kephas.Web.Tests.Logging;

public class CorrelationIdEnricherTest
{
    [Fact]
    public void Enrich_when_called_then_sets_proper_correlationId()
    {
        //Arrange

        var mockContextAccessor = Substitute.For<IContextAccessor>();
        var correlationId = new CorrelationId();
        mockContextAccessor.GetOrAddValue(Arg.Any<Func<CorrelationId>>()).Returns(correlationId);

        var enricher = new CorrelationIdEnricher(mockContextAccessor);
        var mockILogEventPropertyFactory = Substitute.For<ILogEventPropertyFactory>();

        var logEventLevel= new LogEventLevel();
        var exception = new Exception();          
             
        var correlationIdProperty = new LogEventProperty("CorrelationId", new ScalarValue(correlationId));
        mockILogEventPropertyFactory.CreateProperty("CorrelationId", correlationId.ToString(), false).Returns(correlationIdProperty);

        var mockLogEventPropertyValue = Substitute.For<LogEventPropertyValue>();
        var logEventProperties = new List<LogEventProperty> { new("Test", mockLogEventPropertyValue) };
        var messageTemplate = new MessageTemplate(new List<Serilog.Parsing.MessageTemplateToken>());

        var logEvent = new LogEvent(DateTimeOffset.Now, logEventLevel, exception, messageTemplate, logEventProperties);

        //Act
        enricher.Enrich(logEvent, mockILogEventPropertyFactory);

        //Assert
        mockILogEventPropertyFactory.Received(1).CreateProperty("CorrelationId", correlationId.ToString(), false);
    }
}