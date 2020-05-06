// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServiceFactoryCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class OrderedServiceFactoryCollectionTest
    {
        [Test]
        public void OrderedServiceFactoryCollection_null()
        {
            var ordered = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(null);

            Assert.AreEqual(0, ordered.Count());
        }

        [Test]
        public void OrderedServiceFactoryCollection_proper_order_same_override_priority()
        {
            var instance1 = Substitute.For<IInstance>();
            var instance2 = Substitute.For<IInstance>();
            var orderedList = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                    {
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance1,
                            new AppServiceMetadata(overridePriority: 3, processingPriority: 1)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance2,
                            new AppServiceMetadata(overridePriority: 3, processingPriority: -1)),
                    }).ToList();


            Assert.AreEqual(2, orderedList.Count);
            Assert.AreSame(instance2, orderedList[0].CreateExportedValue());
            Assert.AreSame(instance1, orderedList[1].CreateExportedValue());
        }

        [Test]
        public void OrderedServiceFactoryCollection_proper_order()
        {
            var instance1 = Substitute.For<IInstance>();
            var instance2 = Substitute.For<IInstance>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<IInstance>();
            var orderedList = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                    {
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance1,
                            new AppServiceMetadata(overridePriority: 3, processingPriority: 1)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance2,
                            new AppServiceMetadata(overridePriority: 2, processingPriority: 2)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance3,
                            new AppServiceMetadata(overridePriority: 2, processingPriority: 3)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance4,
                            new AppServiceMetadata(overridePriority: 1, processingPriority: 1)),
                    }).ToList();


            Assert.AreEqual(4, orderedList.Count);
            Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
            Assert.AreSame(instance2, orderedList[1].CreateExportedValue());
            Assert.AreSame(instance3, orderedList[2].CreateExportedValue());
            Assert.AreSame(instance1, orderedList[3].CreateExportedValue());
        }

        [Test]
        public void OrderedServiceFactoryCollection_proper_order_with_override()
        {
            var instance1 = Substitute.For<TestDerived>();
            var instance2 = Substitute.For<Test>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<TestMostDerived>();
            var orderedList = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                {
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance1,
                        new AppServiceMetadata(overridePriority: 3, processingPriority: 1)
                            {
                                AppServiceImplementationType = typeof(TestDerived),
                            }),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(overridePriority: 2, processingPriority: 2)
                            {
                                AppServiceImplementationType = typeof(Test),
                            }),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(overridePriority: 2, processingPriority: 3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(overridePriority: 1, processingPriority: 1, isOverride: true)
                            {
                                AppServiceImplementationType = typeof(TestMostDerived),
                            }),
                }).ToList();


            Assert.AreEqual(3, orderedList.Count);
            Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
            Assert.AreSame(instance2, orderedList[1].CreateExportedValue());
            Assert.AreSame(instance3, orderedList[2].CreateExportedValue());
        }

        [Test]
        public void OrderedServiceFactoryCollection_proper_order_with_override_chain()
        {
            var instance1 = Substitute.For<TestDerived>();
            var instance2 = Substitute.For<Test>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<TestMostDerived>();
            var orderedList = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                {
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance1,
                        new AppServiceMetadata(overridePriority: 3, processingPriority: 1, isOverride: true)
                            {
                                AppServiceImplementationType = typeof(TestDerived),
                            }),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(overridePriority: 2, processingPriority: 2, isOverride: true)
                            {
                                AppServiceImplementationType = typeof(Test),
                            }),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(overridePriority: 2, processingPriority: 3)),
                    new ExportFactory<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(overridePriority: 1, processingPriority: 1, isOverride: true)
                            {
                                AppServiceImplementationType = typeof(TestMostDerived),
                            }),
                }).ToList();


            Assert.AreEqual(2, orderedList.Count);
            Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
            Assert.AreSame(instance3, orderedList[1].CreateExportedValue());
        }

        [Test]
        public void GetServiceFactories_filter()
        {
            var instance1 = Substitute.For<IInstance>();
            var instance2 = Substitute.For<IInstance>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<IInstance>();
            var orderedList = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(
                new List<IExportFactory<IInstance, AppServiceMetadata>>
                    {
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance1,
                            new AppServiceMetadata(overridePriority: 3, processingPriority: 1)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance2,
                            new AppServiceMetadata(overridePriority: 2, processingPriority: 2)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance3,
                            new AppServiceMetadata(overridePriority: 2, processingPriority: 3)),
                        new ExportFactory<IInstance, AppServiceMetadata>(
                            () => instance4,
                            new AppServiceMetadata(overridePriority: 1, processingPriority: 1)),
                    })
                .GetServiceFactories(f => f.Metadata.ProcessingPriority == 1)
                .ToList();


            Assert.AreEqual(2, orderedList.Count);
            Assert.AreSame(instance4, orderedList[0].CreateExportedValue());
            Assert.AreSame(instance1, orderedList[1].CreateExportedValue());
        }

        [Test]
        public void GetServices_filter()
        {
            var instance1 = Substitute.For<IInstance>();
            var instance2 = Substitute.For<IInstance>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<IInstance>();
            var orderedList = new OrderedServiceFactoryCollection<IInstance, AppServiceMetadata>(
                    new List<IExportFactory<IInstance, AppServiceMetadata>>
                        {
                            new ExportFactory<IInstance, AppServiceMetadata>(
                                () => instance1,
                                new AppServiceMetadata(overridePriority: 3, processingPriority: 1)),
                            new ExportFactory<IInstance, AppServiceMetadata>(
                                () => instance2,
                                new AppServiceMetadata(overridePriority: 2, processingPriority: 2)),
                            new ExportFactory<IInstance, AppServiceMetadata>(
                                () => instance3,
                                new AppServiceMetadata(overridePriority: 2, processingPriority: 3)),
                            new ExportFactory<IInstance, AppServiceMetadata>(
                                () => instance4,
                                new AppServiceMetadata(overridePriority: 1, processingPriority: 1)),
                        })
                .GetServices(f => f.Metadata.OverridePriority < 3)
                .ToList();


            Assert.AreEqual(3, orderedList.Count);
            Assert.AreSame(instance4, orderedList[0]);
            Assert.AreSame(instance2, orderedList[1]);
            Assert.AreSame(instance3, orderedList[2]);
        }

        public class TestMostDerived : TestDerived {}

        public class TestDerived : Test {}

        public class Test : IInstance
        {
#if NETSTANDARD2_1
#else
            public ITypeInfo GetTypeInfo() => this.GetType().AsRuntimeTypeInfo();
#endif
        }
    }
}