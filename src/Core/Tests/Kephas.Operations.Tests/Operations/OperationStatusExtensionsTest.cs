// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationStatusExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Operations;
using Xunit;

namespace Kephas.Tests.Operations;

public class OperationStatusExtensionsTests
{
    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, false)]
    [InlineData(OperationStatus.Executing, false)]
    [InlineData(OperationStatus.Finalizing, false)]
    [InlineData(OperationStatus.Paused, false)]
    [InlineData(OperationStatus.Idle, false)]
    [InlineData(OperationStatus.FailedTry, false)]
    [InlineData(OperationStatus.Failed, false)]
    [InlineData(OperationStatus.Canceled, false)]
    [InlineData(OperationStatus.Aborted, false)]
    [InlineData(OperationStatus.TimedOut, false)]
    [InlineData(OperationStatus.Completed, true)]
    [InlineData(OperationStatus.CompletedPartially, true)]
    [InlineData(OperationStatus.CompletedWithWarnings, true)]
    public void IsCompletedPhase_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsCompletedPhase().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, false)]
    [InlineData(OperationStatus.Executing, false)]
    [InlineData(OperationStatus.Finalizing, false)]
    [InlineData(OperationStatus.Paused, false)]
    [InlineData(OperationStatus.Idle, false)]
    [InlineData(OperationStatus.FailedTry, false)]
    [InlineData(OperationStatus.Failed, true)]
    [InlineData(OperationStatus.Canceled, true)]
    [InlineData(OperationStatus.Aborted, true)]
    [InlineData(OperationStatus.TimedOut, true)]
    [InlineData(OperationStatus.Completed, false)]
    [InlineData(OperationStatus.CompletedPartially, false)]
    [InlineData(OperationStatus.CompletedWithWarnings, false)]
    public void IsFailedPhase_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsFailedPhase().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, false)]
    [InlineData(OperationStatus.Executing, false)]
    [InlineData(OperationStatus.Finalizing, false)]
    [InlineData(OperationStatus.Paused, true)]
    [InlineData(OperationStatus.Idle, true)]
    [InlineData(OperationStatus.FailedTry, true)]
    [InlineData(OperationStatus.Failed, false)]
    [InlineData(OperationStatus.Canceled, false)]
    [InlineData(OperationStatus.Aborted, false)]
    [InlineData(OperationStatus.TimedOut, false)]
    [InlineData(OperationStatus.Completed, false)]
    [InlineData(OperationStatus.CompletedPartially, false)]
    [InlineData(OperationStatus.CompletedWithWarnings, false)]
    public void IsWaitingPhase_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsWaitingPhase().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, true)]
    [InlineData(OperationStatus.Executing, true)]
    [InlineData(OperationStatus.Finalizing, true)]
    [InlineData(OperationStatus.Paused, false)]
    [InlineData(OperationStatus.Idle, false)]
    [InlineData(OperationStatus.FailedTry, false)]
    [InlineData(OperationStatus.Failed, false)]
    [InlineData(OperationStatus.Canceled, false)]
    [InlineData(OperationStatus.Aborted, false)]
    [InlineData(OperationStatus.TimedOut, false)]
    [InlineData(OperationStatus.Completed, false)]
    [InlineData(OperationStatus.CompletedPartially, false)]
    [InlineData(OperationStatus.CompletedWithWarnings, false)]
    public void IsExecutingPhase_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsExecutingPhase().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperationStatus.None, true)]
    [InlineData(OperationStatus.Initializing, false)]
    [InlineData(OperationStatus.Executing, false)]
    [InlineData(OperationStatus.Finalizing, false)]
    [InlineData(OperationStatus.Paused, false)]
    [InlineData(OperationStatus.Idle, false)]
    [InlineData(OperationStatus.FailedTry, false)]
    [InlineData(OperationStatus.Failed, false)]
    [InlineData(OperationStatus.Canceled, false)]
    [InlineData(OperationStatus.Aborted, false)]
    [InlineData(OperationStatus.TimedOut, false)]
    [InlineData(OperationStatus.Completed, false)]
    [InlineData(OperationStatus.CompletedPartially, false)]
    [InlineData(OperationStatus.CompletedWithWarnings, false)]
    public void IsNonePhase_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsNonePhase().Should().Be(expected);
    }
 
    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, true)]
    [InlineData(OperationStatus.Executing, true)]
    [InlineData(OperationStatus.Finalizing, true)]
    [InlineData(OperationStatus.Paused, true)]
    [InlineData(OperationStatus.Idle, true)]
    [InlineData(OperationStatus.FailedTry, true)]
    [InlineData(OperationStatus.Failed, true)]
    [InlineData(OperationStatus.Canceled, true)]
    [InlineData(OperationStatus.Aborted, true)]
    [InlineData(OperationStatus.TimedOut, true)]
    [InlineData(OperationStatus.Completed, true)]
    [InlineData(OperationStatus.CompletedPartially, true)]
    [InlineData(OperationStatus.CompletedWithWarnings, true)]
    public void IsStarted_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsStarted().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, true)]
    [InlineData(OperationStatus.Executing, true)]
    [InlineData(OperationStatus.Finalizing, true)]
    [InlineData(OperationStatus.Paused, true)]
    [InlineData(OperationStatus.Idle, true)]
    [InlineData(OperationStatus.FailedTry, true)]
    [InlineData(OperationStatus.Failed, false)]
    [InlineData(OperationStatus.Canceled, false)]
    [InlineData(OperationStatus.Aborted, false)]
    [InlineData(OperationStatus.TimedOut, false)]
    [InlineData(OperationStatus.Completed, false)]
    [InlineData(OperationStatus.CompletedPartially, false)]
    [InlineData(OperationStatus.CompletedWithWarnings, false)]
    public void IsInProgress_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsInProgress().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperationStatus.None, false)]
    [InlineData(OperationStatus.Initializing, false)]
    [InlineData(OperationStatus.Executing, false)]
    [InlineData(OperationStatus.Finalizing, false)]
    [InlineData(OperationStatus.Paused, false)]
    [InlineData(OperationStatus.Idle, false)]
    [InlineData(OperationStatus.FailedTry, false)]
    [InlineData(OperationStatus.Failed, true)]
    [InlineData(OperationStatus.Canceled, true)]
    [InlineData(OperationStatus.Aborted, true)]
    [InlineData(OperationStatus.TimedOut, true)]
    [InlineData(OperationStatus.Completed, true)]
    [InlineData(OperationStatus.CompletedPartially, true)]
    [InlineData(OperationStatus.CompletedWithWarnings, true)]
    public void IsFinalized_ReturnsExpectedResult(OperationStatus status, bool expected)
    {
        status.IsFinalized().Should().Be(expected);
    }
}