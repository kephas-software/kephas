// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSettingsHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Endpoints.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class UpdateSettingsHandlerTest
    {
        [Test]
        public async Task ProcessAsync_instance()
        {
            var settings = new CoreSettings();
            var config = Substitute.For<IConfiguration<CoreSettings>>();
            config.UpdateSettingsAsync(Arg.Any<CoreSettings>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var success = ci.Arg<CoreSettings>().Task.DefaultTimeout == TimeSpan.FromMinutes(5);
                    return Task.FromResult<IOperationResult<bool>>(
                        new OperationResult<bool>(success)
                        .Complete(operationState: success ? OperationState.Completed : OperationState.Warning));
                });
            var container = Substitute.For<IServiceProvider>();
            container.Resolve(typeof(IConfiguration<CoreSettings>))
                .Returns(config);
            var typeResolver = new DefaultTypeResolver(() => new List<Assembly> { typeof(CoreSettings).Assembly });

            var handler = new UpdateSettingsHandler(container, typeResolver, Substitute.For<ISerializationService>());
            var result = await handler.ProcessAsync(
                new UpdateSettingsMessage { Settings = new CoreSettings { Task = new TaskSettings { DefaultTimeout = TimeSpan.FromMinutes(5) } } },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreEqual(SeverityLevel.Info, result.Severity);
        }

        [Test]
        public async Task ProcessAsync_string()
        {
            var settings = new CoreSettings();
            var config = Substitute.For<IConfiguration<CoreSettings>>();
            config.GetSettings(Arg.Any<IContext?>()).Returns(settings);
            config.UpdateSettingsAsync(Arg.Any<CoreSettings>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var success = ci.Arg<CoreSettings>().Task.DefaultTimeout == TimeSpan.FromMinutes(5);
                    return Task.FromResult<IOperationResult<bool>>(
                        new OperationResult<bool>(success)
                            .Complete(operationState: success ? OperationState.Completed : OperationState.Warning));
                });
            var container = Substitute.For<IServiceProvider>();
            container.Resolve(typeof(IConfiguration<CoreSettings>))
                .Returns(config);
            var typeResolver = new DefaultTypeResolver(() => new List<Assembly> { typeof(CoreSettings).Assembly });

            var settingsString = @"{""task"": {""defaultTimeout"": ""0:5:0""} }";
            var serializationService = Substitute.For<ISerializationService>();
            serializationService.JsonDeserializeAsync(settingsString, Arg.Any<Action<ISerializationContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<object?>(
                    new CoreSettings { Task = new TaskSettings { DefaultTimeout = TimeSpan.FromMinutes(5) } }));
            var handler = new UpdateSettingsHandler(container, typeResolver, serializationService);
            var result = await handler.ProcessAsync(
                new UpdateSettingsMessage { SettingsType = "core", Settings = settingsString },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreEqual(SeverityLevel.Info, result.Severity);
        }
    }
}