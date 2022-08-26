// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerableTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services;

using System.Collections.Generic;
using System.Linq;

using Kephas.Injection;
using Kephas.Services;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class FactoryEnumerableTest
{
    [Test]
    public void FactoryEnumerable_null()
    {
        var ordered = new FactoryEnumerable<IInstance, AppServiceMetadata>(null);

        Assert.AreEqual(0, ordered.Count());
    }

    [Test]
    public void FactoryEnumerable_proper_order_same_override_priority()
    {
        var instance1 = Substitute.For<IInstance>();
        var instance2 = Substitute.For<IInstance>();
        var orderedList = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<IExportFactory<IInstance, AppServiceMetadata>>
            {
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance1,
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)(-1))),
            }).ToList();


        Assert.AreEqual(2, orderedList.Count);
        Assert.AreSame(instance2, orderedList[0].CreateExportedValue());
        Assert.AreSame(instance1, orderedList[1].CreateExportedValue());
    }

    [Test]
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
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)2)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance3,
                    new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)3)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance4,
                    new AppServiceMetadata(overridePriority: (Priority)1, processingPriority: (Priority)1)),
            }).ToList();


        Assert.AreEqual(4, orderedList.Count);
        Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
        Assert.AreSame(instance2, orderedList[1].CreateExportedValue());
        Assert.AreSame(instance3, orderedList[2].CreateExportedValue());
        Assert.AreSame(instance1, orderedList[3].CreateExportedValue());
    }

    [Test]
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
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1)
                    {
                        ServiceType = typeof(TestDerived),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)2)
                    {
                        ServiceType = typeof(Test),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance3,
                    new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)3)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance4,
                    new AppServiceMetadata(overridePriority: (Priority)1, processingPriority: (Priority)1, isOverride: true)
                    {
                        ServiceType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.AreEqual(3, orderedList.Count);
        Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
        Assert.AreSame(instance2, orderedList[1].CreateExportedValue());
        Assert.AreSame(instance3, orderedList[2].CreateExportedValue());
    }

    [Test]
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
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1, isOverride: true)
                    {
                        ServiceType = typeof(TestDerived),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance2,
                    new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)2, isOverride: true)
                    {
                        ServiceType = typeof(Test),
                    }),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance3,
                    new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)3)),
                new ExportFactory<IInstance, AppServiceMetadata>(
                    () => instance4,
                    new AppServiceMetadata(overridePriority: (Priority)1, processingPriority: (Priority)1, isOverride: true)
                    {
                        ServiceType = typeof(TestMostDerived),
                    }),
            }).ToList();


        Assert.AreEqual(2, orderedList.Count);
        Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
        Assert.AreSame(instance3, orderedList[1].CreateExportedValue());
    }

    [Test]
    public void TryGetService_not_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: Priority.High)),
            });

        Assert.IsNull(((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).TryGetService(m => m.ProcessingPriority == Priority.Normal));
    }

    [Test]
    public void GetService_not_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: Priority.High)),
            });

        Assert.Throws<ArgumentException>(() => ((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).GetService(m => m.ProcessingPriority == Priority.Normal));
    }

    [Test]
    public void TryGetService_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: Priority.High)),
            });

        Assert.IsNotNull(((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).TryGetService(m => m.ProcessingPriority == Priority.High));
    }

    [Test]
    public void GetService_found()
    {
        var factoryEnumerable = new FactoryEnumerable<IInstance, AppServiceMetadata>(
            new List<ExportFactory<IInstance, AppServiceMetadata>>
            {
                new (
                    () => Substitute.For<IInstance>(),
                    new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: Priority.High)),
            });

        Assert.IsNotNull(((IFactoryEnumerable<IInstance, AppServiceMetadata>)factoryEnumerable).TryGetService(m => m.ProcessingPriority == Priority.High));
    }

    [Test]
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
                        new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)2)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(overridePriority: (Priority)1, processingPriority: (Priority)1)),
                }))
            .TryGetService(m => m.ProcessingPriority == (Priority)1);

        Assert.AreSame(instance4, instance);
    }

    [Test]
    public void GetService_matches_order()
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
                        new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)2)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(overridePriority: (Priority)1, processingPriority: (Priority)1)),
                }))
            .GetService(m => m.ProcessingPriority == (Priority)1);

        Assert.AreSame(instance4, instance);
    }

    [Test]
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
                        new AppServiceMetadata(overridePriority: (Priority)3, processingPriority: (Priority)1)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)2)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(overridePriority: (Priority)2, processingPriority: (Priority)3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(overridePriority: (Priority)1, processingPriority: (Priority)1)),
                }))
            .SelectServices(f => f.Metadata.OverridePriority < (Priority)3)
            .ToList();


        Assert.AreEqual(3, orderedList.Count);
        Assert.AreSame(instance4, orderedList[0]);
        Assert.AreSame(instance2, orderedList[1]);
        Assert.AreSame(instance3, orderedList[2]);
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