// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultExtensionsTest.cs" company="Kephas Software SRL">
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

public class OperationResultExtensionsTest
{
    [Fact]
    public void Value_SetsReturnValue()
    {
        var result = Substitute.For<IOperationResult>();
        var value = new object();

        var updatedResult = result.Value(value);

        updatedResult.Value.Should().Be(value);
    }

    [Fact]
    public void Value_ThrowsArgumentNullException_WhenResultIsNull()
    {
        IOperationResult result = null;
        Action act = () => result.Value(new object());

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Complete_SetsElapsedAndStatus()
    {
        var result = Substitute.For<IOperationResult>();
        var elapsed = TimeSpan.FromSeconds(1);

        var updatedResult = result.Complete(elapsed);

        updatedResult.Elapsed.Should().Be(elapsed);
        updatedResult.OperationStatus.Should().Be(OperationStatus.Completed);
        updatedResult.PercentCompleted.Should().Be(1);
    }

    [Fact]
    public void Fail_SetsElapsedStatusAndException()
    {
        var result = new OperationResult();
        var exception = new Exception("Test exception");
        var elapsed = TimeSpan.FromSeconds(1);

        var updatedResult = result.Fail(exception, elapsed);

        updatedResult.Elapsed.Should().Be(elapsed);
        updatedResult.OperationStatus.Should().Be(OperationStatus.Failed);
        updatedResult.Messages.Should().ContainSingle(m => m is ExceptionMessage && ((ExceptionMessage)m).Exception.ErrorType == exception.GetType().Name);
    }

    [Fact]
    public void MergeMessage_AddsMessage()
    {
        var result = new OperationResult();
        var message = "Test message";

        var updatedResult = result.MergeMessage(message);

        updatedResult.Messages.Should().ContainSingle(m => m.Message == message);
    }

    [Fact]
    public void MergeMessages_AddsMessagesFromAnotherResult()
    {
        var result = new OperationResult();
        var message = new OperationMessage("Test message");
        var resultToMerge = new OperationResult().MergeMessage(message);

        var updatedResult = result.MergeMessages(resultToMerge);

        updatedResult.Should().BeSameAs(result);
        var same = updatedResult.Messages.Contains(message); 
        updatedResult.Messages.Should().Contain(message);
    }

    [Fact]
    public void HasErrors_ReturnsTrue_WhenMessagesContainErrors()
    {
        var result = Substitute.For<IOperationResult>();
        var errorMessage = Substitute.For<IOperationMessage>();
        errorMessage.Severity.Returns(SeverityLevel.Error);
        result.Messages.Returns(new List<IOperationMessage> { errorMessage });

        var hasErrors = result.HasErrors();

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public void HasWarnings_ReturnsTrue_WhenMessagesContainWarnings()
    {
        var result = Substitute.For<IOperationResult>();
        var warningMessage = Substitute.For<IOperationMessage>();
        warningMessage.Severity.Returns(SeverityLevel.Warning);
        result.Messages.Returns(new List<IOperationMessage> { warningMessage });

        var hasWarnings = result.HasWarnings();

        hasWarnings.Should().BeTrue();
    }

    [Fact]
    public void Errors_ReturnsOnlyErrorMessages()
    {
        var result = Substitute.For<IOperationResult>();
        var errorMessage = Substitute.For<IOperationMessage>();
        errorMessage.Severity.Returns(SeverityLevel.Error);
        var infoMessage = Substitute.For<IOperationMessage>();
        infoMessage.Severity.Returns(SeverityLevel.Information);
        result.Messages.Returns(new List<IOperationMessage> { errorMessage, infoMessage });

        var errors = result.Errors();

        errors.Should().ContainSingle().Which.Should().Be(errorMessage);
    }

    [Fact]
    public void Warnings_ReturnsOnlyWarningMessages()
    {
        var result = Substitute.For<IOperationResult>();
        var warningMessage = Substitute.For<IOperationMessage>();
        warningMessage.Severity.Returns(SeverityLevel.Warning);
        var infoMessage = Substitute.For<IOperationMessage>();
        infoMessage.Severity.Returns(SeverityLevel.Information);
        result.Messages.Returns(new List<IOperationMessage> { warningMessage, infoMessage });

        var warnings = result.Warnings();

        warnings.Should().ContainSingle().Which.Should().Be(warningMessage);
    }

    [Fact]
    public void Infos_ReturnsOnlyInfoMessages()
    {
        var result = Substitute.For<IOperationResult>();
        var infoMessage = Substitute.For<IOperationMessage>();
        infoMessage.Severity.Returns(SeverityLevel.Information);
        var errorMessage = Substitute.For<IOperationMessage>();
        errorMessage.Severity.Returns(SeverityLevel.Error);
        result.Messages.Returns(new List<IOperationMessage> { infoMessage, errorMessage });

        var infos = result.Infos();

        infos.Should().ContainSingle().Which.Should().Be(infoMessage);
    }

    [Fact]
    public void TryGetException_ReturnsException_WhenErrorsExist()
    {
        var result = Substitute.For<IOperationResult>();
        var errorMessage = new ExceptionMessage(new Exception("Test exception"));
        result.Messages.Returns(new List<IOperationMessage> { errorMessage });

        var exception = result.TryGetException();

        exception.Should().BeOfType<AppException>().Which.Message.Should().Be("Test exception");
    }

    [Fact]
    public void TryGetException_ReturnsNull_WhenNoErrorsExist()
    {
        var result = Substitute.For<IOperationResult>();
        result.Messages.Returns(new List<IOperationMessage>());

        var exception = result.TryGetException();

        exception.Should().BeNull();
    }

    [Fact]
    public void ThrowIfHasErrors_ThrowsException_WhenErrorsExist()
    {
        var result = Substitute.For<IOperationResult>();
        var errorMessage = new ExceptionMessage(new Exception("Test exception"));
        result.Messages.Returns(new List<IOperationMessage> { errorMessage });

        Action act = () => result.ThrowIfHasErrors();

        act.Should().Throw<Exception>().WithMessage("Test exception");
    }

    [Fact]
    public void ThrowIfHasErrors_DoesNotThrow_WhenNoErrorsExist()
    {
        var result = Substitute.For<IOperationResult>();
        result.Messages.Returns(new List<IOperationMessage>());

        Action act = () => result.ThrowIfHasErrors();

        act.Should().NotThrow();
    }

    [Fact]
    public void ToOperationResult_ConvertsValueToCompletedOperationResult()
    {
        var value = "Test value";
        var elapsed = TimeSpan.FromSeconds(1);

        var result = value.ToOperationResult(elapsed);

        result.Value.Should().Be(value);
        result.Elapsed.Should().Be(elapsed);
        result.OperationStatus.Should().Be(OperationStatus.Completed);
    }

    [Fact]
    public void ToAsyncOperationResult_ConvertsValueToCompletedAsyncOperationResult()
    {
        var value = "Test value";
        var elapsed = TimeSpan.FromSeconds(1);

        var result = value.ToAsyncOperationResult(elapsed);

        result.AsTask().Result.Should().Be(value);
        result.Elapsed.Should().Be(elapsed);
        result.OperationStatus.Should().Be(OperationStatus.Completed);
    }

    [Fact]
    public void ToOperationResult_ConvertsExceptionToFailedOperationResult()
    {
        var exception = new Exception("Test exception");
        var elapsed = TimeSpan.FromSeconds(1);

        var result = exception.ToOperationResult<string>(elapsed);

        result.Value.Should().BeNull();
        result.Elapsed.Should().Be(elapsed);
        result.OperationStatus.Should().Be(OperationStatus.Failed);
        result.Messages.Should().ContainSingle(m => m is ExceptionMessage && ((ExceptionMessage)m).Exception.ErrorType == exception.GetType().Name);
    }

    [Fact]
    public void ToAsyncOperationResult_ConvertsExceptionToFailedAsyncOperationResult()
    {
        var exception = new Exception("Test exception");
        var elapsed = TimeSpan.FromSeconds(1);

        var result = exception.ToAsyncOperationResult<string>(elapsed);

        result.AsTask().IsFaulted.Should().BeTrue();
        result.Elapsed.Should().Be(elapsed);
        result.OperationStatus.Should().Be(OperationStatus.Failed);
        result.Messages.Should().ContainSingle(m => m is ExceptionMessage && ((ExceptionMessage)m).Exception.ErrorType == exception.GetType().Name);
    }
}