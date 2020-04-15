// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureLifecycleBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature lifecycle behavior test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Composition;
    using Kephas.Logging;
    using NSubstitute;
    using NUnit.Framework;

    public class FeatureLifecycleBehaviorBaseTest
    {
        [Test]
        public async Task Log()
        {
            var logger = Substitute.For<ILogger>();
            var logManager = Substitute.For<ILogManager>();
            var ambientServices = Substitute.For<IAmbientServices>();
            var appContext = Substitute.For<IAppContext>();
            appContext.AmbientServices.Returns(ambientServices);
            ambientServices.LogManager.Returns(logManager);
            logManager.GetLogger(Arg.Any<string>()).Returns(logger);
            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);

            var sb = new StringBuilder();
            logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>()))
                .Do(ci => sb.Append(ci.Arg<string>()).Append("."));

            IFeatureLifecycleBehavior behavior = new TestFeatureLifecycleBehavior();
            await behavior.BeforeInitializeAsync(appContext, null);
            await behavior.AfterInitializeAsync(appContext, null);
            await behavior.BeforeFinalizeAsync(appContext, null);
            await behavior.AfterFinalizeAsync(appContext, null);

            Assert.AreEqual("BeforeInitializeAsync.AfterInitializeAsync.BeforeFinalizeAsync.AfterFinalizeAsync.", sb.ToString());
        }

        public class TestFeatureLifecycleBehavior : FeatureLifecycleBehaviorBase
        {
            public override Task BeforeInitializeAsync(IAppContext appContext, FeatureManagerMetadata metadata, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("BeforeInitializeAsync");
                return base.BeforeInitializeAsync(appContext, null, cancellationToken);
            }

            public override Task AfterInitializeAsync(IAppContext appContext, FeatureManagerMetadata metadata, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("AfterInitializeAsync");
                return base.AfterInitializeAsync(appContext, null, cancellationToken);
            }

            public override Task BeforeFinalizeAsync(IAppContext appContext, FeatureManagerMetadata metadata, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("BeforeFinalizeAsync");
                return base.BeforeFinalizeAsync(appContext, null, cancellationToken);
            }

            public override Task AfterFinalizeAsync(IAppContext appContext, FeatureManagerMetadata metadata, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("AfterFinalizeAsync");
                return base.AfterFinalizeAsync(appContext, null, cancellationToken);
            }
        }
    }
}
