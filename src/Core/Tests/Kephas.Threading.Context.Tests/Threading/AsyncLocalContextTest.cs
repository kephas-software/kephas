// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLocalContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using FluentAssertions;
using Kephas.Threading;
using Xunit;

namespace Kephas.Tests.Threading;

public class AsyncLocalContextTest
{
    [Fact]
    public async Task GetValue_ShouldReturnCorrectValue_WhenValueIsSet()
    {
        // Arrange
        var contextAccessor = new AsyncLocalContext();
        var contextWriter = contextAccessor.GetWriter();
        var expectedValue1 = "testValue1";
        var expectedValue2 = "testValue2";

        contextAccessor.TryGetValue<string>().Should().BeNull();

        // Act
        Task task1;
        using (ExecutionContext.SuppressFlow())
        {    
            task1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(25);
                contextAccessor.TryGetValue<string>().Should().BeNull();
                contextWriter.SetValue(expectedValue1);
                Thread.Sleep(25);
                contextAccessor.GetValue<string>().Should().Be(expectedValue1);
            });
        }        

        Task task2;
        using (ExecutionContext.SuppressFlow())
        {    
            task2 = Task.Factory.StartNew(() =>
            {
                contextAccessor.TryGetValue<string>().Should().BeNull();
                contextWriter.SetValue(expectedValue2);
                Thread.Sleep(60);
                contextAccessor.GetValue<string>().Should().Be(expectedValue2);
            });
        }        

        await Task.WhenAll(task1, task2);
        
        // Assert
        contextAccessor.TryGetValue<string>().Should().BeNull();
    }

    [Fact]
    public void GetValue_ShouldThrowException_WhenValueIsNotSet()
    {
        // Arrange
        var contextAccessor = new AsyncLocalContext();

        // Act
        Action act = () => contextAccessor.GetValue<string>();

        // Assert
        act.Should().Throw<KeyNotFoundException>()
            .WithMessage("No contextual value was set for type System.String.");
    }

    [Fact]
    public void TryGetValue_ShouldReturnNull_WhenValueIsNotSet()
    {
        // Arrange
        var contextAccessor = new AsyncLocalContext();

        // Act
        var actualValue = contextAccessor.TryGetValue<string>();

        // Assert
        actualValue.Should().BeNull();
    }

    [Fact]
    public async Task RemoveValue_ShouldRemoveValueCorrectly()
    {
        // Arrange
        var contextAccessor = new AsyncLocalContext();
        var contextWriter = contextAccessor.GetWriter();
        var expectedValue1 = "testValue1";
        var expectedValue2 = "testValue2";

        contextAccessor.TryGetValue<string>().Should().BeNull();

        // Act
        Task task1;
        using (ExecutionContext.SuppressFlow())
        {
            task1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(30);
                contextWriter.SetValue(expectedValue1);
                Thread.Sleep(25);
                contextWriter.RemoveValue<string>();
                Thread.Sleep(15);
                contextAccessor.TryGetValue<string>().Should().BeNull();
            });
        }

        Task task2;
        using (ExecutionContext.SuppressFlow())
        {
            task2 = Task.Factory.StartNew(() =>
            {
                contextWriter.SetValue(expectedValue2);
                Thread.Sleep(25);
                contextWriter.RemoveValue<string>();
                Thread.Sleep(25);
                contextAccessor.TryGetValue<string>().Should().BeNull();
            });
        }

        await Task.WhenAll(task1, task2);
        
        // Assert
        contextAccessor.TryGetValue<string>().Should().BeNull();
    }

    [Fact]
    public async Task GetContext_ShouldGetValueAndRestoreCorrectly()
    {
        // Arrange
        var contextAccessor = new AsyncLocalContext();
        var contextWriter = contextAccessor.GetWriter();
        var expectedValue1 = "testValue1";
        var expectedValue2 = new StringBuilder("testValue2");

        contextAccessor.TryGetValue<string>().Should().BeNull();

        // Act
        IEnumerable<KeyValuePair<Type, object>> context = null!;

        Task task1;
        using (ExecutionContext.SuppressFlow())
        {
            task1 = Task.Factory.StartNew(() =>
            {
                contextWriter.SetValue(expectedValue1);
                contextWriter.SetValue(expectedValue2);
                context = contextAccessor.GetContext();
                contextWriter.SetValue<object>(expectedValue1);
                context.Should().NotBeNull();
                context.Should().BeEquivalentTo(
                [
                    KeyValuePair.Create(typeof(string), (object)expectedValue1),
                    KeyValuePair.Create(typeof(StringBuilder), (object)expectedValue2)
                ]);
            });
        }

        await task1;

        Task task2;
        using (ExecutionContext.SuppressFlow())
        {
            task2 = Task.Factory.StartNew(() =>
            {
                contextAccessor.GetContext().Should().BeEmpty();
                contextWriter.RestoreContext(context);
                contextAccessor.GetContext().Should().BeEquivalentTo(
                [
                    KeyValuePair.Create(typeof(string), (object)expectedValue1),
                    KeyValuePair.Create(typeof(StringBuilder), (object)expectedValue2)
                ]);
            });
        }

        await task2;
        
        // Assert
        contextAccessor.TryGetValue<string>().Should().BeNull();
    }
}