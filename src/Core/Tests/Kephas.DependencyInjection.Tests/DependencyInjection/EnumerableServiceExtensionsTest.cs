// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.DependencyInjection;

public class EnumerableServiceExtensionsTest
{
    [Fact]
    public void SelectServices_lazy_no_predicate()
    {
        var services = new Lazy<string, string>[]
        {
            new(() => "abc", "x"),
            new(() => "123", "y"),
        };
            
        Assert.Equal(["abc", "123"], services.SelectServices());
    }

    [Theory]
    [InlineData("a", new[] { "x", "y" })]
    [InlineData("ab", new[] { "x" })]
    [InlineData("x", new string[0])]
    public void SelectServices_lazy_predicate(string start, string[] result)
    {
        var services = new Lazy<string, string>[]
        {
            new(() => "x", "abc"),
            new(() => "y", "a123"),
        };
            
        services.SelectServices(m => m.StartsWith(start)).Should().BeEquivalentTo(result);
    }

    [Theory]
    [InlineData("m-ab", "abc")]
    [InlineData("m-a", "a123")]
    [InlineData("m-a1", "a123")]
    [InlineData("x", null)]
    public void GetService_lazy_predicate(string start, string? result)
    {
        var services = new Lazy<string, string>[]
        {
            new(() => "abc", "m-abc"),
            new(() => "a123", "m-a123"),
        };
            
        Assert.Equal(result, services.GetService(m => m.StartsWith(start)));
    }

    [Theory]
    [InlineData("m-ab", "abc")]
    [InlineData("m-a", "a123")]
    [InlineData("m-a1", "a123")]
    public void GetRequiredService_lazy_predicate_found(string start, string? result)
    {
        var services = new Lazy<string, string>[]
        {
            new(() => "abc", "m-abc"),
            new(() => "a123", "m-a123"),
        };
            
        Assert.Equal(result, services.GetRequiredService(m => m.StartsWith(start)));
    }

    [Fact]
    public void GetRequiredService_lazy_predicate_notfound()
    {
        var services = Array.Empty<Lazy<string, string>>();
            
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
    }

    [Fact]
    public void SelectServices_factory_no_predicate()
    {
        var f1 = Substitute.For<IFactory<string, string>>();
        f1.Create().Returns("a", "b");
        var f2 = Substitute.For<IFactory<string, string>>();
        f2.Create().Returns("1", "2");

        var services = new[] { f1, f2 };
            
        Assert.Equal(new[] { "a", "1" }, services.SelectServices());
        Assert.Equal(new[] { "b", "2" }, services.SelectServices());
    }

    [Theory]
    [InlineData(new[] { "abc", "a123" }, "m-a", new[] { "abc", "a123" })]
    [InlineData(new[] { "abc", "a123" }, "m-ab", new[] { "abc" })]
    [InlineData(new[] { "abc", "a123" }, "m-x", new string[0])]
    public void SelectServices_factory_predicate(string[] values, string start, string[] result)
    {
        var services = values.Select(v =>
        {
            var f = Substitute.For<IFactory<string, string>>();
            f.Create().Returns(v, v + "-2");
            f.Metadata.Returns("m-" + v);
            return f;
        }).ToList();
            
        Assert.Equal(result, services.SelectServices(s => s.Metadata.StartsWith(start)));
        Assert.Equal(result.Select(v => v + "-2"), services.SelectServices(s => s.Metadata.StartsWith(start)));
    }

    [Theory]
    [InlineData(new[] { "a123", "abc" }, "m-a", "abc")]
    [InlineData(new[] { "abc", "a123" }, "m-a", "a123")]
    [InlineData(new[] { "abc", "a123" }, "m-a1", "a123")]
    [InlineData(new[] { "abc", "a123" }, "m-x", null)]
    public void GetService_factory_predicate(string[] values, string start, string? result)
    {
        var services = values.Select(v =>
        {
            var f = Substitute.For<IFactory<string, string>>();
            f.Create().Returns(v, v + "-2");
            f.Metadata.Returns("m-" + v);
            return f;
        }).ToList();
            
        Assert.Equal(result, services.GetService(m => m.StartsWith(start)));
        Assert.Equal(result is null ? null : result + "-2", services.GetService(m => m.StartsWith(start)));
    }

    [Theory]
    [InlineData(new[] { "a123", "abc" }, "m-a", "abc")]
    [InlineData(new[] { "abc", "a123" }, "m-a", "a123")]
    [InlineData(new[] { "abc", "a123" }, "m-a1", "a123")]
    public void GetRequiredService_factory_predicate_found(string[] values, string start, string? result)
    {
        var services = values.Select(v =>
        {
            var f = Substitute.For<IFactory<string, string>>();
            f.Create().Returns(v, v + "-2");
            f.Metadata.Returns("m-" + v);
            return f;
        }).ToList();
            
        services.GetRequiredService(m => m.StartsWith(start))
            .Should().Be(result);
        services.GetRequiredService(m => m.StartsWith(start))
            .Should().Be(result is null ? null : result + "-2");
    }

    [Fact]
    public void GetRequiredService_factory_predicate_notfound()
    {
        var services = Array.Empty<IFactory<string, string>>();
            
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
    }
}