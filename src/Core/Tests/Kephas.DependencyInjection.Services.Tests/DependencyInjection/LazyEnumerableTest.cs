// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyEnumerableTest.cs" company="Kephas Software SRL">
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

public class LazyEnumerableTest
{
    [Fact]
    public void LazyEnumerable_null()
    {
        var ordered = new LazyEnumerable<IInstance, ServiceMetadata>(null);
        ordered.Should().BeEmpty();
    }

    [Fact]
    public void LazyEnumerable_proper_order_same_override_priority()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var orderedList = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new(
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1)),
                new(
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)(-1))),
            }).ToList();


        Assert.Equal(2, orderedList.Count);
        Assert.Same(instance2, orderedList[0].Value);
        Assert.Same(instance1, orderedList[1].Value);
    }

    [Fact]
    public void LazyEnumerable_proper_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var orderedList = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new(() => instance1, new ServiceMetadata(priority: (Priority)1)),
                new(() => instance2, new ServiceMetadata(priority: (Priority)2)),
                new(() => instance3, new ServiceMetadata(priority: (Priority)3)),
                new(() => instance4, new ServiceMetadata(priority: (Priority)1)),
            }).ToList();


        Assert.Equal(4, orderedList.Count);
        Assert.Same(instance1, orderedList[0].Value);
        Assert.Same(instance4, orderedList[1].Value);
        Assert.Same(instance2, orderedList[2].Value);
        Assert.Same(instance3, orderedList[3].Value);
    }

    [Fact]
    public void LazyEnumerable_proper_order_with_override()
    {
        var instance1 = Substitute.For<TestDerived>();
        var instance2 = Substitute.For<Test>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<TestMostDerived>();
        var orderedList = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new Lazy<IInstance, ServiceMetadata>(
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1)
                    {
                        ImplementationType = typeof(TestDerived),
                    }),
                new Lazy<IInstance, ServiceMetadata>(
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)2)
                    {
                        ImplementationType = typeof(Test),
                    }),
                new Lazy<IInstance, ServiceMetadata>(
                    () => instance3,
                    new ServiceMetadata(priority: (Priority)3)),
                new Lazy<IInstance, ServiceMetadata>(
                    () => instance4,
                    new ServiceMetadata(priority: (Priority)1, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(TestMostDerived),
                    }),
            }).ToList();

        Assert.Equal(3, orderedList.Count);
        Assert.Same(instance4, orderedList[0].Value);
        Assert.Same(instance2, orderedList[1].Value);
        Assert.Same(instance3, orderedList[2].Value);
    }

    [Fact]
    public void LazyEnumerable_proper_order_with_override_chain()
    {
        var instance1 = Substitute.For<TestDerived>();
        var instance2 = Substitute.For<Test>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<TestMostDerived>();
        var orderedList = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new (
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(TestDerived),
                    }),
                new (
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)2, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(Test),
                    }),
                new (
                    () => instance3,
                    new ServiceMetadata(priority: (Priority)3)),
                new (
                    () => instance4,
                    new ServiceMetadata(priority: (Priority)1, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.Equal(2, orderedList.Count);
        Assert.Same(instance4, orderedList[0].Value);
        Assert.Same(instance3, orderedList[1].Value);
    }

    [Fact]
    public void GetService_not_found()
    {
        var lazyEnumerable = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.Null(lazyEnumerable.GetService(m => m.Priority == Priority.Normal));
    }

    [Fact]
    public void GetRequiredService_not_found()
    {
        var lazyEnumerable = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.Throws<InvalidOperationException>(() => lazyEnumerable.GetRequiredService(m => m.Priority == Priority.Normal));
    }

    [Fact]
    public void GetService_found()
    {
        var lazyEnumerable = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.NotNull(lazyEnumerable.GetService(m => m.Priority == Priority.High));
    }

    [Fact]
    public void GetRequiedService_found()
    {
        var lazyEnumerable = new LazyEnumerable<IInstance, ServiceMetadata>(
            new List<Lazy<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.NotNull(lazyEnumerable.GetRequiredService(m => m.Priority == Priority.High));
    }

    [Fact]
    public void TryGetService_matches_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var instance = ((ILazyEnumerable<IInstance, ServiceMetadata>)new LazyEnumerable<IInstance, ServiceMetadata>(
                new List<Lazy<IInstance, ServiceMetadata>>
                {
                    new (
                        () => instance1,
                        new ServiceMetadata(priority: (Priority)1)),
                    new (
                        () => instance2,
                        new ServiceMetadata(priority: (Priority)2)),
                    new (
                        () => instance3,
                        new ServiceMetadata(priority: (Priority)3)),
                    new (
                        () => instance4,
                        new ServiceMetadata(priority: (Priority)1)),
                }))
            .GetService(m => m.Priority == (Priority)1);


        Assert.Same(instance4, instance);
    }

    [Fact]
    public void GetService_matches_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var instance = new LazyEnumerable<IInstance, ServiceMetadata>(
                new List<Lazy<IInstance, ServiceMetadata>>
                {
                    new (() => instance1, new ServiceMetadata(priority: (Priority)1)),
                    new (() => instance2, new ServiceMetadata(priority: (Priority)2)),
                    new (() => instance3, new ServiceMetadata(priority: (Priority)3)),
                    new (() => instance4, new ServiceMetadata(priority: (Priority)1)),
                })
            .GetRequiredService(m => m.Priority == (Priority)1);

        Assert.Same(instance4, instance);
    }

    [Fact]
    public void SelectServices_filter()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var orderedList = new LazyEnumerable<IInstance, ServiceMetadata>(
                new List<Lazy<IInstance, ServiceMetadata>>
                {
                    new(() => instance1, new ServiceMetadata(priority: (Priority)1)),
                    new(() => instance2, new ServiceMetadata(priority: (Priority)2)),
                    new(() => instance3, new ServiceMetadata(priority: (Priority)3)),
                    new(() => instance4, new ServiceMetadata(priority: (Priority)1)),
                })
            .SelectServices(m => m.Priority < (Priority)3)
            .ToList();

        Assert.Equal(3, orderedList.Count);
        Assert.Same(instance1, orderedList[0]);
        Assert.Same(instance4, orderedList[1]);
        Assert.Same(instance2, orderedList[2]);
    }

    public class TestMostDerived : TestDerived {}

    public class TestDerived : Test
    {
    }

    public interface IInstance
    {
    }

    public class Test : IInstance
    {
    }
}