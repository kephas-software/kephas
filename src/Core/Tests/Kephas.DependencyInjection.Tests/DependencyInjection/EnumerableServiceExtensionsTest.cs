// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
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
        }.ToLazyEnumerable();
            
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
        }.ToLazyEnumerable();
            
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
        }.ToLazyEnumerable();
            
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
        }.ToLazyEnumerable();
            
        Assert.Equal(result, services.GetRequiredService(m => m.StartsWith(start)));
    }

    [Fact]
    public void GetRequiredService_lazy_predicate_notfound()
    {
        var services = Array.Empty<Lazy<string, string>>().ToLazyEnumerable();
            
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
    }

    [Theory]
    [MemberData(nameof(GetServicesWithMetadata))]
    public void GetService_lazy_priority(string[] values, ServiceMetadata[] metadata, Func<ServiceMetadata, bool> filter, string? result)
    {
        var services = values
            .Select((v, i) => new Lazy<string, ServiceMetadata>(() => v, metadata[i]))
            .ToLazyEnumerable();

        services.GetService(filter).Should().Be(result);
    }

    [Fact]
    public void SelectServices_factory_no_predicate()
    {
        var f1 = Substitute.For<IFactory<string, string>>();
        f1.GetService().Returns("a", "b");
        var f2 = Substitute.For<IFactory<string, string>>();
        f2.GetService().Returns("1", "2");

        var services = new[] { f1, f2 }.ToFactoryEnumerable();
            
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
            f.GetService().Returns(v, v + "-2");
            f.Metadata.Returns("m-" + v);
            return f;
        }).ToFactoryEnumerable();
            
        Assert.Equal(result, services.SelectServices(m => m.StartsWith(start)));
        Assert.Equal(result.Select(v => v + "-2"), services.SelectServices(m => m.StartsWith(start)));
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
            f.GetService().Returns(v, v + "-2");
            f.Metadata.Returns("m-" + v);
            return f;
        }).ToFactoryEnumerable();
            
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
            f.GetService().Returns(v, v + "-2");
            f.Metadata.Returns("m-" + v);
            return f;
        }).ToFactoryEnumerable();
            
        services.GetRequiredService(m => m.StartsWith(start))
            .Should().Be(result);
        services.GetRequiredService(m => m.StartsWith(start))
            .Should().Be(result is null ? null : result + "-2");
    }

    [Fact]
    public void GetRequiredService_factory_predicate_notfound()
    {
        var services = Array.Empty<IFactory<string, string>>().ToFactoryEnumerable();
            
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
        Assert.Throws<InvalidOperationException>(() => services.GetRequiredService(m => m.StartsWith("x")));
    }

    [Theory]
    [MemberData(nameof(GetServicesWithMetadata))]
    public void GetService_factory_priority(string[] values, ServiceMetadata[] metadata, Func<ServiceMetadata, bool> filter, string? result)
    {
        var services = values.Select((v, i) =>
        {
            var f = Substitute.For<IFactory<string, ServiceMetadata>>();
            f.GetService().Returns(v);
            f.Metadata.Returns(metadata[i]);
            return f;
        }).ToFactoryEnumerable();

        services.GetService(filter).Should().Be(result);
    }

    public static TheoryData<string[], ServiceMetadata[], Func<ServiceMetadata, bool>, string?> GetServicesWithMetadata()
    {
        return new()
        {
            {
                ["1", "2", "3", "4" ],
                [new(Priority.Normal, "k1"), new(Priority.Low, "k1"), new(Priority.Normal, "k1"), new(Priority.Normal, "k2")],
                m => m.ServiceKey == "k1",
                "3"
            },
            {
                ["1", "2", "3", "4" ],
                [new(Priority.Normal, "k1"), new(Priority.Normal, "k2"), new(Priority.High, "k1"), new(Priority.Normal, "k3")],
                m => m.ServiceKey == "k1",
                "3"
            },
            {
                ["1", "2", "3", "4" ],
                [new(Priority.Normal, "k1"), new(Priority.High, "k1"), new(Priority.High, "k1"), new(Priority.Normal, "k3")],
                m => m.ServiceKey == "k1",
                "3"
            },
            {
                ["1", "2", "3", "4" ],
                [new(Priority.Normal, "k1"), new(Priority.Low, "k1"), new(Priority.Low, "k1"), new(Priority.Normal, "k3")],
                m => m.ServiceKey == "k1",
                "1"
            },
            {
                ["1", "2", "3", "4" ],
                [new(Priority.Normal, "k1"), new(Priority.Low, "k1"), new(Priority.Low, "k1"), new(Priority.Normal, "k1")],
                m => m.ServiceKey == "k1",
                "4"
            },
        };
    }
}