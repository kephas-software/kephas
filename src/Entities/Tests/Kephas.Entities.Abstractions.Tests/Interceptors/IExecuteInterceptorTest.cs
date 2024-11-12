// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteRepositoryInterceptorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Entities.Interceptors;
using Kephas.Entities.StoredProcedures;
using Kephas.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Entities.Tests.Interceptors;

public class ExecuteRepositoryInterceptorTest
{
    [Fact]
    public async Task HandleAsync_Should_Invoke_Next()
    {
        // Arrange
        var repository = Substitute.For<IWriteRepository>();
        var spArgs = Substitute.For<ISpArgs>();
        var logger = Test.GetTestLogger<TestExecuteRepositoryInterceptor>();
        IExecuteRepositoryInterceptor<ISpArgs> repositoryInterceptor = new TestExecuteRepositoryInterceptor(repository, spArgs, logger);
        var nextInvoked = false;

        var operation = new ExecuteOperation<ISpArgs>(repository, spArgs);

        // Act
        var result = await repositoryInterceptor.HandleAsync(operation, () =>
        {
            nextInvoked = true;
            return Task.FromResult<object?>(null);
        }, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        logger.ToString().Should().StartWith("Information: Hello from the repositoryInterceptor.");
        nextInvoked.Should().BeTrue();
    }

    private class TestExecuteRepositoryInterceptor(IWriteRepository testRepository, ISpArgs testSpArgs, ILogger<TestExecuteRepositoryInterceptor> logger)
        : IExecuteRepositoryInterceptor<ISpArgs>
    {
        public async Task HandleAsync(IWriteRepository repository, ISpArgs args, Func<Task> next, CancellationToken cancellationToken)
        {
            repository.Should().BeSameAs(testRepository);
            args.Should().BeSameAs(testSpArgs);
            logger.LogInformation("Hello from the repositoryInterceptor.");
            await next();
        }
    }
}
