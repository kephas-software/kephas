// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadRepositoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Entities.StoredProcedures;
using NSubstitute;
using Xunit;

namespace Kephas.Entities.Tests;

public class ReadRepositoryTests
{
    [Fact]
    public void GetById_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var expectedEntity = Substitute.For<IIdentifiable>();
        expectedEntity.Id.Returns(1);
        IReadRepository repository = new TestReadRepository(expectedEntity);

        // Act
        var result = repository.GetById<IIdentifiable>(1);

        // Assert
        result.Should().Be(expectedEntity);
    }

    [Fact]
    public void GetById_ShouldThrowException_WhenEntityDoesNotExist()
    {
        // Arrange
        IReadRepository repository = new TestReadRepository();

        // Act
        Action act = () => repository.GetById<IIdentifiable>(1);

        // Assert
        act.Should().Throw<EntityNotFoundException>();
    }

    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var expectedEntity = Substitute.For<IIdentifiable>();
        expectedEntity.Id.Returns(1);
        IReadRepository repository = new TestReadRepository(expectedEntity);

        // Act
        var result = await repository.TryGetByIdAsync<IIdentifiable>(1);

        // Assert
        result.Should().Be(expectedEntity);
    }

    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        IReadRepository repository = new TestReadRepository();

        // Act
        var result = await repository.TryGetByIdAsync<IIdentifiable>(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var expectedEntity = Substitute.For<IIdentifiable>();
        expectedEntity.Id.Returns(1);
        IReadRepository repository = new TestReadRepository(expectedEntity);

        // Act
        var result = await repository.GetAsync<IIdentifiable>(e => e.Id.Equals(1));

        // Assert
        result.Should().Be(expectedEntity);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowException_WhenEntityDoesNotExist()
    {
        // Arrange
        IReadRepository repository = new TestReadRepository();

        // Act
        Func<Task> act = async () => await repository.GetAsync<IIdentifiable>(e => e.Id.Equals(1));

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private class TestReadRepository(params object[] entities) : IReadRepository
    {
        public IEntityContext GetEntityContext(Type entityType)
            => Substitute.For<IEntityContext>();

        public IQueryable<T> Query<T>(bool trackEntities = false) where T : class =>
            entities.OfType<T>().AsQueryable();

        public IQueryable<T> Query<T>(ISpArgs args, bool trackEntities = false) where T : class =>
            entities.OfType<T>().AsQueryable();

        public T? TryGetById<T>(object id) where T : class, IIdentifiable =>
            Query<T>().FirstOrDefault(e => e.Id.Equals(id));

        public Task<T?> TryGetByIdAsync<T>(object id, CancellationToken cancellationToken = default) where T : class, IIdentifiable =>
            Task.FromResult(Query<T>().FirstOrDefault(e => e.Id.Equals(id)));
    } 
}