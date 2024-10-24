// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerableTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.DependencyInjection;

public class FactoryEnumerableTest
{
    [Fact]
    public void FactoryEnumerable_null()
    {
        var ordered = new FactoryEnumerable<IInstance, ServiceMetadata>(null);

        Assert.Equal(0, ordered.Count());
    }

    [Fact]
    public void FactoryEnumerable_proper_order_same_override_priority()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var orderedList = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<IFactory<IInstance, ServiceMetadata>>
            {
                new Factory<IInstance, ServiceMetadata>(
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1)),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)(-1))),
            }).ToList();


        Assert.Equal(2, orderedList.Count);
        Assert.Same(instance2, orderedList[0].Create());
        Assert.Same(instance1, orderedList[1].Create());
    }

    [Fact]
    public void FactoryEnumerable_proper_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var orderedList = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<IFactory<IInstance, ServiceMetadata>>
            {
                new Factory<IInstance, ServiceMetadata>(
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1)),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)2)),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance3,
                    new ServiceMetadata(priority: (Priority)3)),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance4,
                    new ServiceMetadata(priority: (Priority)1)),
            }).ToList();


        Assert.Equal(4, orderedList.Count);
        Assert.Same(instance1, orderedList[0].Create());
        Assert.Same(instance4, orderedList[1].Create());
        Assert.Same(instance2, orderedList[2].Create());
        Assert.Same(instance3, orderedList[3].Create());
    }

    [Fact]
    public void FactoryEnumerable_proper_order_with_override()
    {
        var instance1 = Substitute.For<TestDerived>();
        var instance2 = Substitute.For<Test>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<TestMostDerived>();
        var orderedList = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<IFactory<IInstance, ServiceMetadata>>
            {
                new Factory<IInstance, ServiceMetadata>(
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1)
                    {
                        ImplementationType = typeof(TestDerived),
                    }),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)2)
                    {
                        ImplementationType = typeof(Test),
                    }),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance3,
                    new ServiceMetadata(priority: (Priority)3)),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance4,
                    new ServiceMetadata(priority: (Priority)1, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.Equal(3, orderedList.Count);
        Assert.Same(instance4, orderedList[0].Create());
        Assert.Same(instance2, orderedList[1].Create());
        Assert.Same(instance3, orderedList[2].Create());
    }

    [Fact]
    public void FactoryEnumerable_proper_order_with_override_chain()
    {
        var instance1 = Substitute.For<TestDerived>();
        var instance2 = Substitute.For<Test>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<TestMostDerived>();
        var orderedList = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<IFactory<IInstance, ServiceMetadata>>
            {
                new Factory<IInstance, ServiceMetadata>(
                    () => instance1,
                    new ServiceMetadata(priority: (Priority)1, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(TestDerived),
                    }),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance2,
                    new ServiceMetadata(priority: (Priority)2, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(Test),
                    }),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance3,
                    new ServiceMetadata(priority: (Priority)3)),
                new Factory<IInstance, ServiceMetadata>(
                    () => instance4,
                    new ServiceMetadata(priority: (Priority)1, overrideBaseImplementation: true)
                    {
                        ImplementationType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.Equal(2, orderedList.Count);
        Assert.Same(instance4, orderedList[0].Create());
        Assert.Same(instance3, orderedList[1].Create());
    }

    [Fact]
    public void TryGetService_not_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<Factory<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.Null(((IFactoryEnumerable<IInstance, ServiceMetadata>)factoryEnumerable).TryGetService(m => m.Priority == Priority.Normal));
    }

    [Fact]
    public void GetService_not_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<Factory<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.Throws<InvalidOperationException>(() => factoryEnumerable.GetService(m => m.Priority == Priority.Normal));
    }

    [Fact]
    public void TryGetService_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<Factory<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.NotNull(((IFactoryEnumerable<IInstance, ServiceMetadata>)factoryEnumerable).TryGetService(m => m.Priority == Priority.High));
    }

    [Fact]
    public void GetService_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, ServiceMetadata>(
            new List<Factory<IInstance, ServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new ServiceMetadata(priority: Priority.High)),
            });

        Assert.NotNull(((IFactoryEnumerable<IInstance, ServiceMetadata>)factoryEnumerable).TryGetService(m => m.Priority == Priority.High));
    }

    [Fact]
    public void TryGetService_matches_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var instance = ((IFactoryEnumerable<IInstance, ServiceMetadata>)new FactoryEnumerable<IInstance, ServiceMetadata>(
                new List<IFactory<IInstance, ServiceMetadata>>
                {
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance1,
                        new ServiceMetadata(priority: (Priority)1)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance2,
                        new ServiceMetadata(priority: (Priority)2)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance3,
                        new ServiceMetadata(priority: (Priority)3)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance4,
                        new ServiceMetadata(priority: (Priority)1)),
                }))
            .TryGetService(m => m.Priority == (Priority)1);

        Assert.Same(instance4, instance);
    }

    [Fact]
    public void GetService_matches_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var instance = new FactoryEnumerable<IInstance, ServiceMetadata>(
                new List<IFactory<IInstance, ServiceMetadata>>
                {
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance1,
                        new ServiceMetadata(priority: (Priority)1)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance2,
                        new ServiceMetadata(priority: (Priority)2)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance3,
                        new ServiceMetadata(priority: (Priority)3)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance4,
                        new ServiceMetadata(priority: (Priority)1)),
                })
            .GetService(m => m.Priority == (Priority)1);

        Assert.Same(instance4, instance);
    }

    [Fact]
    public void SelectServices_filter()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var orderedList = new FactoryEnumerable<IInstance, ServiceMetadata>(
                new List<IFactory<IInstance, ServiceMetadata>>
                {
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance1,
                        new ServiceMetadata(priority: (Priority)1)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance2,
                        new ServiceMetadata(priority: (Priority)2)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance3,
                        new ServiceMetadata(priority: (Priority)3)),
                    new Factory<IInstance, ServiceMetadata>(
                        () => instance4,
                        new ServiceMetadata(priority: (Priority)1)),
                })
            .SelectServices(f => f.Metadata.Priority < (Priority)3)
            .ToList();


        Assert.Equal(3, orderedList.Count);
        Assert.Same(instance1, orderedList[0]);
        Assert.Same(instance4, orderedList[1]);
        Assert.Same(instance2, orderedList[2]);
    }

    public class TestMostDerived : TestDerived {}

    public class TestDerived : Test {}

    public interface IInstance
    {
    }

    public class Test : IInstance
    {
    }
}
