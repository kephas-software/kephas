// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiresTransactionPipelineBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Entities.Mediator;
using MediatR;
using NSubstitute;
using Xunit;

namespace Kephas.Entities.Tests.Mediator;

public class RequiresTransactionPipelineBehaviorTest
{
    [Fact]
    public async Task Handle_ShouldNotStartTransaction_WhenNoAttribute()
    {
        // Arrange
        var repository = Substitute.For<IWriteRepository>();
        var request = new TestRequest();
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(new TestResponse());
        var behavior = new RequiresTransactionPipelineBehavior<TestRequest, TestResponse>(repository);

        // Act
        var response = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        await repository.DidNotReceive().EnterNewTransactionAsync(Arg.Any<Func<IWriteRepository, CancellationToken, Task<TestResponse>>>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive().EnterTransactionAsync(Arg.Any<Func<IWriteRepository, CancellationToken, Task<TestResponse>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldStartNewTransaction_WhenAttributeIndicatesNewTransaction()
    {
        // Arrange
        var repository = Substitute.For<IWriteRepository>();
        var request = new TestRequestWithNewTransaction();
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(new TestResponse());
        var behavior = new RequiresTransactionPipelineBehavior<TestRequestWithNewTransaction, TestResponse>(repository);

        // Act
        var response = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        await repository.Received(1).EnterNewTransactionAsync(Arg.Any<Func<IWriteRepository, CancellationToken, Task<TestResponse>>>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive().EnterTransactionAsync(Arg.Any<Func<IWriteRepository, CancellationToken, Task<TestResponse>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldEnterTransaction_WhenAttributeIndicatesExistingTransaction()
    {
        // Arrange
        var repository = Substitute.For<IWriteRepository>();
        var request = new TestRequestWithExistingTransaction();
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(new TestResponse());
        var behavior = new RequiresTransactionPipelineBehavior<TestRequestWithExistingTransaction, TestResponse>(repository);

        // Act
        var response = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        await repository.DidNotReceive().EnterNewTransactionAsync(Arg.Any<Func<IWriteRepository, CancellationToken, Task<TestResponse>>>(), Arg.Any<CancellationToken>());
        await repository.Received(1).EnterTransactionAsync(Arg.Any<Func<IWriteRepository, CancellationToken, Task<TestResponse>>>(), Arg.Any<CancellationToken>());
    }

    private class TestRequest : IRequest<TestResponse> { }

    [RequiresTransaction(StartNewTransaction = true)]
    private class TestRequestWithNewTransaction : IRequest<TestResponse> { }

    [RequiresTransaction(StartNewTransaction = false)]
    private class TestRequestWithExistingTransaction : IRequest<TestResponse> { }

    private class TestResponse { }
}