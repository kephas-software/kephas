// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Entities.Interceptors;
using Kephas.Testing;
using NSubstitute;
using Xunit;

namespace Kephas.Entities.Tests.Interceptors;

public class RepositoryOperationPipelineTest
{
    [Fact]
    public async Task HandleAsync_Should_Invoke_Handler_When_No_Interceptors()
    {
        // Arrange
        var factoryInterceptors = Test.GetTestFactoryEnumerable<IRepositoryInterceptor, RepositoryInterceptorMetadata>();
        
        var pipeline = new RepositoryOperationPipeline(factoryInterceptors);
        var operationArgs = Substitute.For<IRepositoryOperation<int>>();
        Func<Task<int>> handler = () => Task.FromResult(42);

        // Act
        var result = await pipeline.HandleAsync(operationArgs, handler);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public async Task HandleAsync_Should_Invoke_Interceptors_In_Order()
    {
        // Arrange
        var interceptor1 = Substitute.For<IRepositoryInterceptor<IRepositoryOperation<int>, int>>();
        var interceptor2 = Substitute.For<IRepositoryInterceptor<IRepositoryOperation<int>, int>>();
        var factoryInterceptors = Test.GetTestFactoryEnumerable<IRepositoryInterceptor, RepositoryInterceptorMetadata>(
            [
                (interceptor1, new RepositoryInterceptorMetadata(new Dictionary<string, object?> { ["OperationType"] = typeof(IRepositoryOperation<int>), ["ResultType"] = typeof(int) })),
                (interceptor2, new RepositoryInterceptorMetadata(new Dictionary<string, object?> { ["OperationType"] = typeof(IRepositoryOperation<int>), ["ResultType"] = typeof(int) }))
            ]);

        var pipeline = new RepositoryOperationPipeline(factoryInterceptors);
        var operationArgs = Substitute.For<IRepositoryOperation<int>>();
        Func<Task<int>> handler = () => Task.FromResult(42);

        interceptor1.HandleAsync(operationArgs, Arg.Any<InterceptNextDelegate<int>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<InterceptNextDelegate<int>>().Invoke());
        interceptor2.HandleAsync(operationArgs, Arg.Any<InterceptNextDelegate<int>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<InterceptNextDelegate<int>>().Invoke());

        // Act
        var result = await pipeline.HandleAsync(operationArgs, handler);

        // Assert
        result.Should().Be(42);
        Received.InOrder(() =>
        {
            interceptor1.HandleAsync(operationArgs, Arg.Any<InterceptNextDelegate<int>>(), Arg.Any<CancellationToken>());
            interceptor2.HandleAsync(operationArgs, Arg.Any<InterceptNextDelegate<int>>(), Arg.Any<CancellationToken>());
        });
    }
}