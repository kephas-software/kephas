// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppBootstrapperTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application bootstrapper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Services.Composition;
    using Kephas.Testing.Core.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppBootstrapperTest
    {
        [Test]
        public async Task StartAsync_right_initializer_order()
        {
            var order = new List<int>();

            var initializer1 = Substitute.For<IAppInitializer>();
            initializer1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var initializer2 = Substitute.For<IAppInitializer>();
            initializer2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var bootstrapper = new DefaultAppBootstrapper(
                Substitute.For<IAmbientServices>(),
                Substitute.For<ICompositionContext>(),
                new[]
                    {
                        new TestExportFactory<IAppInitializer, AppServiceMetadata>(() => initializer1, new AppServiceMetadata(processingPriority: 2)),
                        new TestExportFactory<IAppInitializer, AppServiceMetadata>(() => initializer2, new AppServiceMetadata(processingPriority: 1)),
                    },
                null);

            await bootstrapper.StartAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
        }

        [Test]
        public async Task StartAsync_right_behavior_order()
        {
            var order = new List<string>();

            var initializer1 = Substitute.For<IAppInitializer>();
            initializer1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var initializer2 = Substitute.For<IAppInitializer>();
            initializer2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var behavior1 = Substitute.For<IAppInitializerBehavior>();
            behavior1.BeforeInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before1"));
            behavior1.AfterInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After1"));

            var behavior2 = Substitute.For<IAppInitializerBehavior>();
            behavior2.BeforeInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before2"));
            behavior2.AfterInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After2"));

            var bootstrapper = new DefaultAppBootstrapper(
                Substitute.For<IAmbientServices>(),
                Substitute.For<ICompositionContext>(),
                new[]
                    {
                        new TestExportFactory<IAppInitializer, AppServiceMetadata>(() => initializer1, new AppServiceMetadata()),
                        new TestExportFactory<IAppInitializer, AppServiceMetadata>(() => initializer2, new AppServiceMetadata()),
                    },
                new[]
                    {
                        new TestExportFactory<IAppInitializerBehavior, AppServiceMetadata>(() => behavior1, new AppServiceMetadata(processingPriority: 2)),
                        new TestExportFactory<IAppInitializerBehavior, AppServiceMetadata>(() => behavior2, new AppServiceMetadata(processingPriority: 1)),
                    });

            await bootstrapper.StartAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(8, order.Count);
            Assert.AreEqual("Before2", order[0]);
            Assert.AreEqual("Before1", order[1]);
            Assert.AreEqual("After1", order[2]);
            Assert.AreEqual("After2", order[3]);
            Assert.AreEqual("Before2", order[4]);
            Assert.AreEqual("Before1", order[5]);
            Assert.AreEqual("After1", order[6]);
            Assert.AreEqual("After2", order[7]);
        }
    }
}