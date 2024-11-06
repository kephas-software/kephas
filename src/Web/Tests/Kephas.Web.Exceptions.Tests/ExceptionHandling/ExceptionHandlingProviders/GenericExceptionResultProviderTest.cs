// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericExceptionResultProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Web.ExceptionHandling;
using Kephas.Web.ExceptionHandling.ExceptionResultProviders;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.ExceptionHandling.ExceptionHandlingProviders;

public class GenericExceptionResultProviderTest
{
    [Fact]
    public void TryGetExceptionResult_ReturnsExceptionResult_WithProvidedException()
    {
        var exception = new Exception("Test exception");
        var exceptionContext = Substitute.For<IExceptionContext>();
        var provider = new GenericExceptionResultProvider();

        var result = provider.TryGetExceptionResult(exception, exceptionContext)!;

        result.Should().NotBeNull();
        result.ErrorType.Should().Be("Exception");
        result.Message.Should().Be("Test exception");
    }

    [Fact]
    public void TryGetExceptionResult_Throws_WhenExceptionIsNull()
    {
        Exception exception = null!;
        var exceptionContext = Substitute.For<IExceptionContext>();
        var provider = new GenericExceptionResultProvider();

        var action = () => provider.TryGetExceptionResult(exception, exceptionContext);

        action.Should().Throw<ArgumentNullException>();
    }
}