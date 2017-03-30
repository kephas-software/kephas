// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppDisposerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application disposer test class.
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
    public class DefaultAppDisposerTest
    {
        [Test]
        public async Task DisposeAsync_right_initializer_order()
        {
            var order = new List<int>();

            var finalizer1 = Substitute.For<IAppFinalizer>();
            finalizer1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var finalizer2 = Substitute.For<IAppFinalizer>();
            finalizer2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var bootstrapper = new DefaultAppDisposer(
                Substitute.For<IAmbientServices>(),
                Substitute.For<ICompositionContext>(),
                new[]
                    {
                        new TestExportFactory<IAppFinalizer, AppServiceMetadata>(() => finalizer1, new AppServiceMetadata(processingPriority: 2)),
                        new TestExportFactory<IAppFinalizer, AppServiceMetadata>(() => finalizer2, new AppServiceMetadata(processingPriority: 1)),
                    },
                null);

            await bootstrapper.DisposeAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
        }

        [Test]
        public async Task DisposeAsync_right_behavior_order()
        {
            var order = new List<string>();

            var initializer1 = Substitute.For<IAppFinalizer>();
            initializer1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var initializer2 = Substitute.For<IAppFinalizer>();
            initializer2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var behavior1 = Substitute.For<IAppFinalizerBehavior>();
            behavior1.BeforeFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before1"));
            behavior1.AfterFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After1"));

            var behavior2 = Substitute.For<IAppFinalizerBehavior>();
            behavior2.BeforeFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before2"));
            behavior2.AfterFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<AppServiceMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After2"));

            var bootstrapper = new DefaultAppDisposer(
                Substitute.For<IAmbientServices>(),
                Substitute.For<ICompositionContext>(),
                new[]
                    {
                        new TestExportFactory<IAppFinalizer, AppServiceMetadata>(() => initializer1, new AppServiceMetadata()),
                        new TestExportFactory<IAppFinalizer, AppServiceMetadata>(() => initializer2, new AppServiceMetadata()),
                    },
                new[]
                    {
                        new TestExportFactory<IAppFinalizerBehavior, AppServiceMetadata>(() => behavior1, new AppServiceMetadata(processingPriority: 2)),
                        new TestExportFactory<IAppFinalizerBehavior, AppServiceMetadata>(() => behavior2, new AppServiceMetadata(processingPriority: 1)),
                    });

            await bootstrapper.DisposeAsync(Substitute.For<IAppContext>(), CancellationToken.None);

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