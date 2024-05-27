// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyEnumerableTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Metadata;

namespace Kephas.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Services;
    using NSubstitute;
    using Xunit;

        public class LazyEnumerableTest
    {
        [Fact]
        public void LazyEnumerable_null()
        {
            var ordered = new LazyEnumerable<IInstance, AppServiceMetadata>(null);

            Assert.Equal(0, ordered.Count());
        }

        [Fact]
        public void LazyEnumerable_proper_order_same_override_priority()
        {
            var instance1 = Substitute.For<IInstance>();
            var instance2 = Substitute.For<IInstance>();
            var orderedList = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                    {
                        new Lazy<IInstance, AppServiceMetadata>(
                            () => instance1,
                            new AppServiceMetadata(priority: (Priority)1)),
                        new Lazy<IInstance, AppServiceMetadata>(
                            () => instance2,
                            new AppServiceMetadata(priority: (Priority)(-1))),
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
            var orderedList = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                    {
                        new Lazy<IInstance, AppServiceMetadata>(
                            () => instance1,
                            new AppServiceMetadata(priority: (Priority)1)),
                        new Lazy<IInstance, AppServiceMetadata>(
                            () => instance2,
                            new AppServiceMetadata(priority: (Priority)2)),
                        new Lazy<IInstance, AppServiceMetadata>(
                            () => instance3,
                            new AppServiceMetadata(priority: (Priority)3)),
                        new Lazy<IInstance, AppServiceMetadata>(
                            () => instance4,
                            new AppServiceMetadata(priority: (Priority)1)),
                    }).ToList();


            Assert.Equal(4, orderedList.Count);
            Assert.Same(instance4, orderedList[0].Value);
            Assert.Same(instance2, orderedList[1].Value);
            Assert.Same(instance3, orderedList[2].Value);
            Assert.Same(instance1, orderedList[3].Value);
        }

        [Fact]
        public void LazyEnumerable_proper_order_with_override()
        {
            var instance1 = Substitute.For<TestDerived>();
            var instance2 = Substitute.For<Test>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<TestMostDerived>();
            var orderedList = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                {
                    new Lazy<IInstance, AppServiceMetadata>(
                        () => instance1,
                        new AppServiceMetadata(priority: (Priority)1)
                            {
                                ServiceType = typeof(TestDerived),
                            }),
                    new Lazy<IInstance, AppServiceMetadata>(
                        () => instance2,
                        new AppServiceMetadata(priority: (Priority)2)
                            {
                                ServiceType = typeof(Test),
                            }),
                    new Lazy<IInstance, AppServiceMetadata>(
                        () => instance3,
                        new AppServiceMetadata(priority: (Priority)3)),
                    new Lazy<IInstance, AppServiceMetadata>(
                        () => instance4,
                        new AppServiceMetadata(priority: (Priority)1, isOverride: true)
                            {
                                ServiceType = typeof(TestMostDerived),
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
            var orderedList = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                {
                    new (
                        () => instance1,
                        new AppServiceMetadata(priority: (Priority)1, isOverride: true)
                            {
                                ServiceType = typeof(TestDerived),
                            }),
                    new (
                        () => instance2,
                        new AppServiceMetadata(priority: (Priority)2, isOverride: true)
                            {
                                ServiceType = typeof(Test),
                            }),
                    new (
                        () => instance3,
                        new AppServiceMetadata(priority: (Priority)3)),
                    new (
                        () => instance4,
                        new AppServiceMetadata(priority: (Priority)1, isOverride: true)
                            {
                                ServiceType = typeof(TestMostDerived),
                            }),
                }).ToList();


            Assert.Equal(2, orderedList.Count);
            Assert.Same(instance4, orderedList[0].Value);
            Assert.Same(instance3, orderedList[1].Value);
        }

        [Fact]
        public void TryGetService_not_found()
        {
            var lazyEnumerable = new LazyEnumerable<IInstance, AppServiceMetadata>(
                    new List<Lazy<IInstance, AppServiceMetadata>>
                    {
                        new (
                            () => Substitute.For<IInstance>(),
                            new AppServiceMetadata(priority: Priority.High)),
                    });

            Assert.Null(((ILazyEnumerable<IInstance, AppServiceMetadata>)lazyEnumerable).TryGetService(m => m.Priority == Priority.Normal));
        }

        [Fact]
        public void GetService_not_found()
        {
            var lazyEnumerable = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                {
                    new (
                        () => Substitute.For<IInstance>(),
                        new AppServiceMetadata(priority: Priority.High)),
                });

            Assert.Throws<ArgumentException>(() => ((ILazyEnumerable<IInstance, AppServiceMetadata>)lazyEnumerable).GetService(m => m.Priority == Priority.Normal));
        }

        [Fact]
        public void TryGetService_found()
        {
            var lazyEnumerable = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                {
                    new (
                        () => Substitute.For<IInstance>(),
                        new AppServiceMetadata(priority: Priority.High)),
                });

            Assert.NotNull(((ILazyEnumerable<IInstance, AppServiceMetadata>)lazyEnumerable).TryGetService(m => m.Priority == Priority.High));
        }

        [Fact]
        public void GetService_found()
        {
            var lazyEnumerable = new LazyEnumerable<IInstance, AppServiceMetadata>(
                new List<Lazy<IInstance, AppServiceMetadata>>
                {
                    new (
                        () => Substitute.For<IInstance>(),
                        new AppServiceMetadata(priority: Priority.High)),
                });

            Assert.NotNull(((ILazyEnumerable<IInstance, AppServiceMetadata>)lazyEnumerable).GetService(m => m.Priority == Priority.High));
        }

        [Fact]
        public void TryGetService_matches_order()
        {
            var instance1 = Substitute.For<IInstance>();
            var instance2 = Substitute.For<IInstance>();
            var instance3 = Substitute.For<IInstance>();
            var instance4 = Substitute.For<IInstance>();
            var instance = ((ILazyEnumerable<IInstance, AppServiceMetadata>)new LazyEnumerable<IInstance, AppServiceMetadata>(
                    new List<Lazy<IInstance, AppServiceMetadata>>
                    {
                        new (
                            () => instance1,
                            new AppServiceMetadata(priority: (Priority)1)),
                        new (
                            () => instance2,
                            new AppServiceMetadata(priority: (Priority)2)),
                        new (
                            () => instance3,
                            new AppServiceMetadata(priority: (Priority)3)),
                        new (
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
            var instance = ((ILazyEnumerable<IInstance, AppServiceMetadata>)new LazyEnumerable<IInstance, AppServiceMetadata>(
                    new List<Lazy<IInstance, AppServiceMetadata>>
                    {
                        new (
                            () => instance1,
                            new AppServiceMetadata(priority: (Priority)1)),
                        new (
                            () => instance2,
                            new AppServiceMetadata(priority: (Priority)2)),
                        new (
                            () => instance3,
                            new AppServiceMetadata(priority: (Priority)3)),
                        new (
                            () => instance4,
                            new AppServiceMetadata(priority: (Priority)1)),
                    }))
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
            var orderedList = ((ILazyEnumerable<IInstance, AppServiceMetadata>)new LazyEnumerable<IInstance, AppServiceMetadata>(
                    new List<Lazy<IInstance, AppServiceMetadata>>
                        {
                            new Lazy<IInstance, AppServiceMetadata>(
                                () => instance1,
                                new AppServiceMetadata(priority: (Priority)1)),
                            new Lazy<IInstance, AppServiceMetadata>(
                                () => instance2,
                                new AppServiceMetadata(priority: (Priority)2)),
                            new Lazy<IInstance, AppServiceMetadata>(
                                () => instance3,
                                new AppServiceMetadata(priority: (Priority)3)),
                            new Lazy<IInstance, AppServiceMetadata>(
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
}
