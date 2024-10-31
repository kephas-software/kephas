// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlingExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.ExceptionHandling;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.ExceptionHandling
{
    public class ExceptionHandlingExtensionsTest
    {
        [Theory]
        [InlineData(SeverityLevel.Critical, true)]
        [InlineData(SeverityLevel.Error, true)]
        [InlineData(SeverityLevel.Warning, false)]
        [InlineData(SeverityLevel.Information, false)]
        [InlineData(SeverityLevel.None, false)]
        public void IsError_ReturnsExpectedResult(SeverityLevel severityLevel, bool expected)
        {
            var obj = Substitute.For<IHasSeverityLevel>();
            obj.SeverityLevel.Returns(severityLevel);

            obj.IsError().Should().Be(expected);
        }

        [Theory]
        [InlineData(SeverityLevel.Critical, false)]
        [InlineData(SeverityLevel.Error, false)]
        [InlineData(SeverityLevel.Warning, true)]
        [InlineData(SeverityLevel.Information, false)]
        [InlineData(SeverityLevel.None, false)]
        public void IsWarning_ReturnsExpectedResult(SeverityLevel severityLevel, bool expected)
        {
            var obj = Substitute.For<IHasSeverityLevel>();
            obj.SeverityLevel.Returns(severityLevel);

            obj.IsWarning().Should().Be(expected);
        }

        [Fact]
        public void IsError_ReturnsFalseForNullObject()
        {
            IHasSeverityLevel? obj = null;
            obj!.IsError().Should().BeFalse();
        }

        [Fact]
        public void IsWarning_ReturnsFalseForNullObject()
        {
            IHasSeverityLevel? obj = null;
            obj!.IsWarning().Should().BeFalse();
        }
    }
}