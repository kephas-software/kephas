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

public class SaveChangesRepositoryInterceptorTest
{
    [Fact]
    public async Task HandleAsync_Should_Invoke_Next()
    {
        // Arrange
        var repository = Substitute.For<IWriteRepository>();
        var entityContext = Substitute.For<IEntityContext>();
        var logger = Test.GetTestLogger<TestSaveChangesRepositoryInterceptor>();
        ISaveChangesRepositoryInterceptor repositoryInterceptor = new TestSaveChangesRepositoryInterceptor(repository, entityContext, logger);

        var operation = new SaveChangesOperation(repository, entityContext);

        // Act
        var result = await repositoryInterceptor.HandleAsync(operation, () => Task.FromResult(12), CancellationToken.None);

        // Assert
        result.Should().Be(24);
        logger.ToString().Should().StartWith("Information: Hello from the repositoryInterceptor.");
    }

    private class TestSaveChangesRepositoryInterceptor(IWriteRepository testRepository, IEntityContext testEntityContext, ILogger<TestSaveChangesRepositoryInterceptor> logger)
        : ISaveChangesRepositoryInterceptor
    {
        public async Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext,
            Func<Task<int>> next,
            CancellationToken cancellationToken)
        {
            repository.Should().BeSameAs(testRepository);
            entityContext.Should().BeSameAs(testEntityContext);
            logger.LogInformation("Hello from the repositoryInterceptor.");
            return await next() * 2;
        }
    }
}
