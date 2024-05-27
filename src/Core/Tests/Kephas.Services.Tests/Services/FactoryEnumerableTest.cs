// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerableTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Metadata;

namespace Kephas.Tests.Services;

using System.Collections.Generic;
using System.Linq;
using Kephas.Services;
using NSubstitute;
using Xunit;

public class FactoryEnumerableTest
{
    [Fact]
    public void FactoryEnumerable_null()
    {
        var ordered = new FactoryEnumerable<IInstance, AppServiceMetadata>(null);

        Assert.Equal(0, ordered.Count());
    }

    [Fact]
    public void FactoryEnumerable_proper_order_same_override_priority()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var orderedList = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<IExportFactory<IInstance, AppServiceMetadata>>
            {
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance1,
                    new AppServiceMetadata(priority: (Priority)1)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(priority: (Priority)(-1))),
            }).ToList();


        Assert.Equal(2, orderedList.Count);
        Assert.Same(instance2, orderedList[0].CreateExportedValue());
        Assert.Same(instance1, orderedList[1].CreateExportedValue());
    }

    [Fact]
    public void FactoryEnumerable_proper_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var orderedList = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<IExportFactory<IInstance, AppServiceMetadata>>
            {
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance1,
                    new AppServiceMetadata(priority: (Priority)1)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(priority: (Priority)2)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance3,
                    new AppServiceMetadata(priority: (Priority)3)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance4,
                    new AppServiceMetadata(priority: (Priority)1)),
            }).ToList();


        Assert.Equal(4, orderedList.Count);
        Assert.Same(instance4, orderedList[0].CreateExportedValue());
        Assert.Same(instance2, orderedList[1].CreateExportedValue());
        Assert.Same(instance3, orderedList[2].CreateExportedValue());
        Assert.Same(instance1, orderedList[3].CreateExportedValue());
    }

    [Fact]
    public void FactoryEnumerable_proper_order_with_override()
    {
        var instance1 = Substitute.For<TestDerived>();
        var instance2 = Substitute.For<Test>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<TestMostDerived>();
        var orderedList = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<IExportFactory<IInstance, AppServiceMetadata>>
            {
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance1,
                    new AppServiceMetadata(priority: (Priority)1)
                    {
                        ServiceType = typeof(TestDerived),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(priority: (Priority)2)
                    {
                        ServiceType = typeof(Test),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance3,
                    new AppServiceMetadata(priority: (Priority)3)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance4,
                    new AppServiceMetadata(priority: (Priority)1, isOverride: true)
                    {
                        ServiceType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.Equal(3, orderedList.Count);
        Assert.Same(instance4, orderedList[0].CreateExportedValue());
        Assert.Same(instance2, orderedList[1].CreateExportedValue());
        Assert.Same(instance3, orderedList[2].CreateExportedValue());
    }

    [Fact]
    public void FactoryEnumerable_proper_order_with_override_chain()
    {
        var instance1 = Substitute.For<TestDerived>();
        var instance2 = Substitute.For<Test>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<TestMostDerived>();
        var orderedList = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<IExportFactory<IInstance, AppServiceMetadata>>
            {
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance1,
                    new AppServiceMetadata(priority: (Priority)1, isOverride: true)
                    {
                        ServiceType = typeof(TestDerived),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(priority: (Priority)2, isOverride: true)
                    {
                        ServiceType = typeof(Test),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance3,
                    new AppServiceMetadata(priority: (Priority)3)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance4,
                    new AppServiceMetadata(priority: (Priority)1, isOverride: true)
                    {
                        ServiceType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.Equal(2, orderedList.Count);
        Assert.Same(instance4, orderedList[0].CreateExportedValue());
        Assert.Same(instance3, orderedList[1].CreateExportedValue());
    }

    [Fact]
    public void TryGetService_not_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(priority: Priority.High)),
            });

        Assert.Null(((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).TryGetService(m => m.Priority == Priority.Normal));
    }

    [Fact]
    public void GetService_not_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(priority: Priority.High)),
            });

        Assert.Throws<InvalidOperationException>(() => factoryEnumerable.GetService(m => m.Priority == Priority.Normal));
    }

    [Fact]
    public void TryGetService_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(priority: Priority.High)),
            });

        Assert.NotNull(((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).TryGetService(m => m.Priority == Priority.High));
    }

    [Fact]
    public void GetService_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(priority: Priority.High)),
            });

        Assert.NotNull(((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).TryGetService(m => m.Priority == Priority.High));
    }

    [Fact]
    public void TryGetService_matches_order()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var instance3 = Substitute.For<IInstance>();
        var instance4 = Substitute.For<IInstance>();
        var instance = ((IFactoryEnumerable<IInstance, AppServiceMetadata>)new FactoryEnumerable<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                {
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance1,
                        new AppServiceMetadata(priority: (Priority)1)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(priority: (Priority)2)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(priority: (Priority)3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(priority: (Priority)1)),
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
        var instance = new FactoryEnumerable<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                {
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance1,
                        new AppServiceMetadata(priority: (Priority)1)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(priority: (Priority)2)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(priority: (Priority)3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(priority: (Priority)1)),
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
        var orderedList = ((IFactoryEnumerable<IInstance, AppServiceMetadata>)new FactoryEnumerable<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                {
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance1,
                        new AppServiceMetadata(priority: (Priority)1)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(priority: (Priority)2)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(priority: (Priority)3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(priority: (Priority)1)),
                }))
            .SelectServices(f => f.Metadata.Priority < (Priority)3)
            .ToList();


        Assert.Equal(3, orderedList.Count);
        Assert.Same(instance4, orderedList[0]);
        Assert.Same(instance2, orderedList[1]);
        Assert.Same(instance3, orderedList[2]);
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
