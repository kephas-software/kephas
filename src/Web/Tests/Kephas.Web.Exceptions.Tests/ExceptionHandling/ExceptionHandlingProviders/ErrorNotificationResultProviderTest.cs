// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorNotificationResultProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.ExceptionHandling;
using Kephas.Web.ExceptionHandling;
using Kephas.Web.ExceptionHandling.ExceptionResultProviders;
using NSubstitute;
using Xunit;

namespace Kephas.Web.Tests.ExceptionHandling.ExceptionHandlingProviders;

public class ErrorNotificationResultProviderTest
{
    [Fact]
    public void TryGetExceptionResult_ReturnsExceptionResult_WhenErrorNotificationIsProvided()
    {
        var errorNotification = Substitute.For<IErrorNotification>();
        var exceptionContext = Substitute.For<IExceptionContext>();
        var provider = new ErrorNotificationResultProvider();

        var result = provider.TryGetExceptionResult(errorNotification, exceptionContext);

        result.Should().NotBeNull();
        result.Should().BeOfType<ExceptionResult>();
    }

    [Fact]
    public void TryGetExceptionResult_Throws_WhenErrorNotificationIsNull()
    {
        IErrorNotification errorNotification = null!;
        var exceptionContext = Substitute.For<IExceptionContext>();
        var provider = new ErrorNotificationResultProvider();

        var action = () => provider.TryGetExceptionResult(errorNotification, exceptionContext);

        action.Should().Throw<ArgumentNullException>();
    }
}