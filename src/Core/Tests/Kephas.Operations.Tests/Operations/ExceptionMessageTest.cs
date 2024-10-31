// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionMessageTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.ExceptionHandling;
using Kephas.Operations;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Operations;

public class ExceptionMessageTest
{
    [Fact]
    public void Constructor_SetsExceptionAndMessage()
    {
        var exception = new Exception("Test exception");
        var exceptionMessage = new ExceptionMessage(exception);

        exceptionMessage.Exception.Should().Be(exception);
        exceptionMessage.Message.Should().Be("Test exception");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullExceptionForNullException()
    {
        Action act = () => new ExceptionMessage(null);

        act.Should().Throw<ArgumentNullException>().WithMessage("*exception*");
    }

    [Fact]
    public void ExceptionProperty_ReturnsDefaultExceptionWhenNull()
    {
        var exceptionMessage = new ExceptionMessage();

        exceptionMessage.Exception.Should().NotBeNull();
        exceptionMessage.Exception.Should().BeOfType<Exception>();
    }

    [Fact]
    public void ExceptionProperty_SetsSeverityLevelWhenExceptionImplementsIHasSeverityLevel()
    {
        var exception = Substitute.For<IHasSeverityLevel, Exception>();
        exception.SeverityLevel.Returns(SeverityLevel.Critical);
        var exceptionMessage = new ExceptionMessage((Exception)exception);

        exceptionMessage.SeverityLevel.Should().Be(SeverityLevel.Critical);
    }

    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        var exception = new Exception("Test exception");
        var exceptionMessage = new ExceptionMessage(exception);

        var result = exceptionMessage.ToString();

        result.Should().Contain("Test exception");
        result.Should().Contain("[Exception]");
    }
}