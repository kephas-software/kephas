// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteWriteRepositoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Entities.Tests.Models;
using Kephas.Entities.Tests.Security;
using Kephas.Threading;
using Kephas.Validation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Entities.Tests;

public class SqliteWriteRepositoryTest() : SqliteIntegrationTestBase<SqliteWriteRepositoryTest>(includeTestAssembly: true)
{
    [Theory]
    [InlineData("John", "Doe")]
    public async Task SaveChangesAsync_CRUD_interceptors(string firstName, string lastName)
    {
        var repository = DefaultScope.ServiceProvider.GetRequiredService<ISqlWriteRepository>();
        EnsureAuthenticated();

        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName
        };
        repository.Add(person);
        await repository.SaveChangesAsync();

        person.Id.Should().NotBeEmpty();
        var storagePeople = repository.Query<Person>();
        storagePeople.Should().BeEquivalentTo([person]);
        var storagePerson = storagePeople.Single();
        storagePerson.Age.Should().Be(0);

        person.Age = 25;
        repository.Update(person);
        await repository.SaveChangesAsync();
        storagePerson = await repository.GetByIdAsync<Person>(person.Id);
        storagePerson.Age.Should().Be(25);
        
        repository.Delete(person);
        await repository.SaveChangesAsync();
        repository.Query<Person>().Should().BeEmpty();
    }

    [Theory]
    [InlineData("John", "Doe", "JD", -2, new[] { "The field Age must be between 0 and 150." })]
    [InlineData("John", "Doe", "JD", 200, new[] { "The field Age must be between 0 and 150." })]
    [InlineData("John", null!, null, 20, new[] { "The LastName field is required." })]
    [InlineData("John", null!, "ABCDE", -10,
        new[]
        {
            "The LastName field is required.",
            "The field Initials must be a string or array type with a maximum length of '2'.",
            "The field Age must be between 0 and 150."
        })]
    public async Task SaveChangesAsync_validation_fails(string firstName, string lastName, string? initials, int age, string[] expectedErrors)
    {
        var repository = DefaultScope.ServiceProvider.GetRequiredService<ISqlWriteRepository>();
        EnsureAuthenticated();
        
        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Initials = initials,
            Age = age,
        };
        repository.Add(person);
        Func<Task> act = async () => await repository.SaveChangesAsync();
        (await act.Should().ThrowAsync<ValidationException>())
            .Which.ValidationResult!.Errors.Select(e => e.ErrorMessage)
            .Should().BeEquivalentTo(expectedErrors);
    }
    
    [Theory]
    [InlineData("John", "Doe")]
    public async Task SaveChangesAsync_CRUD_interceptors_with_transaction_commit(string firstName, string lastName)
    {
        var repository = DefaultScope.ServiceProvider.GetRequiredService<ISqlWriteRepository>();
        EnsureAuthenticated();

        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName
        };

        await repository.EnterTransactionAsync(async (r, t) =>
        {
            r.Add(person);
            await r.SaveChangesAsync(t);

            person.Id.Should().NotBeEmpty();
            var storagePeople = r.Query<Person>();
            storagePeople.Should().BeEquivalentTo([person]);
            var storagePerson = storagePeople.Single();
            storagePerson.Age.Should().Be(0);

            person.Age = 25;
            r.Update(person);
            await r.SaveChangesAsync(t);
            storagePerson = await r.GetByIdAsync<Person>(person.Id, t);
            storagePerson.Age.Should().Be(25);
        });
        
        
        repository.Query<Person>().Should().BeEquivalentTo([person]);
    }
    
    [Theory]
    [InlineData("John", "Doe")]
    public async Task SaveChangesAsync_CRUD_interceptors_with_transaction_rollback(string firstName, string lastName)
    {
        var repository = DefaultScope.ServiceProvider.GetRequiredService<ISqlWriteRepository>();
        EnsureAuthenticated();

        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName
        };

        var act = () => repository.EnterTransactionAsync(async (r, t) =>
        {
            r.Add(person);
            await r.SaveChangesAsync(t);

            person.Id.Should().NotBeEmpty();
            var storagePeople = r.Query<Person>();
            storagePeople.Should().BeEquivalentTo([person]);
            var storagePerson = storagePeople.Single();
            storagePerson.Age.Should().Be(0);

            person.Age = 25;
            r.Update(person);
            await r.SaveChangesAsync(t);
            storagePerson = await r.GetByIdAsync<Person>(person.Id, t);
            storagePerson.Age.Should().Be(25);

            throw new InvalidOperationException("Rollback transaction");
        });

        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
        
        repository.Query<Person>().Should().BeEmpty();
    }

    private void EnsureAuthenticated()
    {
        var contextAccessor = DefaultScope.ServiceProvider.GetRequiredService<IAsyncLocalContext>();
        contextAccessor.GetOrAddValue(() => new UserInfo());
    }
}