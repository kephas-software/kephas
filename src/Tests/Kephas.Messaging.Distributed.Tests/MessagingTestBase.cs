// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Behaviors;
    using Kephas.Configuration;
    using Kephas.Cryptography;
    using Kephas.Services;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Security.Authentication;
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using Kephas.Testing.Application;
    using NSubstitute;

    public class MessagingTestBase : ApplicationTestBase
    {
        public override IServiceProvider BuildServiceProvider(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? Array.Empty<Assembly>())
            {
                typeof(IConfiguration<>).Assembly,              // Kephas.Configuration
                typeof(IEncryptionService).Assembly,            // Kephas.Security
                typeof(IBehaviorRule<>).Assembly,               // Kephas.Behaviors
                typeof(IMessageBroker).Assembly,                // Kephas.Messaging.Distributed
                typeof(IMessageProcessor).Assembly,             // Kephas.Messaging
                typeof(JsonSerializer).Assembly,                // Kephas.Serialization.NewtonsoftJson
            };

            return base.BuildServiceProvider(ambientServices, assemblyList, parts, config, logManager, appRuntime);
        }

        protected virtual IServiceProvider CreateMessagingContainerMock()
        {
            var container = Substitute.For<IServiceProvider>();

            Func<object[], DispatchingContext> ctxCreator = args =>
                                    new DispatchingContext(
                                        container,
                                        Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                                        Substitute.For<IMessageBroker>(),
                                        Substitute.For<IAppRuntime>(),
                                        Substitute.For<IAuthenticationService>(),
                                        args.Length > 0 ? args[0] : null);

            container.Resolve(typeof(IExportFactory<IInjectableFactory>))
                .Returns(ci =>
                        new ExportFactory<IInjectableFactory>(
                            () => this.CreateInjectableFactoryMock(ctxCreator)));

            container.Resolve(typeof(Lazy<IInjectableFactory>))
                .Returns(ci =>
                    new Lazy<IInjectableFactory>(
                        () => this.CreateInjectableFactoryMock(ctxCreator)));

            container.Resolve(typeof(IInjectableFactory))
                .Returns(ci => this.CreateInjectableFactoryMock(ctxCreator));

            container.Resolve<IInjectableFactory>()
                .Returns(ci => this.CreateInjectableFactoryMock(ctxCreator));

            return container;
        }
    }
}
