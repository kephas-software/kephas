// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingWithSystemCompositionInjectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.SystemComposition
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Injection;
    using Kephas.Injection.ExportFactories;
    using Kephas.Injection.ExportFactoryImporters;
    using Kephas.Injection.SystemComposition.Builder;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Testing.Application;
    using NSubstitute;

    public class MessagingWithSystemCompositionInjectionTestBase : ApplicationWithSystemCompositionInjectionTestBase
    {
        public override IInjector CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<SystemCompositionInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? Array.Empty<Assembly>())
            {
                typeof(IMessageBroker).GetTypeInfo().Assembly, /* Kephas.Messaging.Distributed */
                typeof(IMessageProcessor).GetTypeInfo().Assembly, /* Kephas.Messaging */
            };

            var oldConfig = config;
            config = b =>
            {
                var appContext = Substitute.For<IAppContext>();
                b.WithFactory(() => appContext);
                oldConfig?.Invoke(b);
            };

            return base.CreateInjector(ambientServices, assemblyList, parts, config, logManager, appRuntime);
        }

        protected virtual IInjector CreateMessagingContainerMock()
        {
            var container = Substitute.For<IInjector>();

            Func<object[], DispatchingContext> ctxCreator = args =>
                                    new DispatchingContext(
                                        container,
                                        Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                                        Substitute.For<IMessageBroker>(),
                                        Substitute.For<IAppRuntime>(),
                                        Substitute.For<IAuthenticationService>(),
                                        args.Length > 0 ? args[0] : null);

            container.Resolve(typeof(IExportFactoryImporter<IContextFactory>), Arg.Any<string>())
                .Returns(ci =>
                    new ExportFactoryImporter<IContextFactory>(
                        new ExportFactory<IContextFactory>(
                            () => this.CreateContextFactoryMock(ctxCreator))));

            container.Resolve(typeof(IContextFactory), Arg.Any<string>())
                .Returns(ci => this.CreateContextFactoryMock(ctxCreator));

            container.Resolve<IContextFactory>(Arg.Any<string>())
                .Returns(ci => this.CreateContextFactoryMock(ctxCreator));

            return container;
        }
    }
}
