// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLifecycleBehaviroBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application lifecycle behaviro base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppLifecycleBehaviorBaseTest
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

            IAppLifecycleBehavior behavior = new TestAppLifecycleBehavior();
            await behavior.BeforeAppInitializeAsync(appContext);
            await behavior.AfterAppInitializeAsync(appContext);
            await behavior.BeforeAppFinalizeAsync(appContext);
            await behavior.AfterAppFinalizeAsync(appContext);

            Assert.AreEqual("BeforeAppInitializeAsync.AfterAppInitializeAsync.BeforeAppFinalizeAsync.AfterAppFinalizeAsync.", sb.ToString());
        }

        public class TestAppLifecycleBehavior : AppLifecycleBehaviorBase
        {
            public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("BeforeAppInitializeAsync");
                return base.BeforeAppInitializeAsync(appContext, cancellationToken);
            }

            public override Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("AfterAppInitializeAsync");
                return base.AfterAppInitializeAsync(appContext, cancellationToken);
            }

            public override Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("BeforeAppFinalizeAsync");
                return base.BeforeAppFinalizeAsync(appContext, cancellationToken);
            }

            public override Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
            {
                this.Logger.Info("AfterAppFinalizeAsync");
                return base.AfterAppFinalizeAsync(appContext, cancellationToken);
            }
        }
    }
}
