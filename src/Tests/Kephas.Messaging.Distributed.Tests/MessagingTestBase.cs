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
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Testing.Application;
    using NSubstitute;

    public class MessagingTestBase : ApplicationTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteCompositionContainerBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0])
            {
                typeof(IMessageProcessor).GetTypeInfo().Assembly, /* Kephas.Messaging */
            };
            
            return base.CreateContainer(ambientServices, assemblyList, parts, config, logManager, appRuntime);
        }

        protected virtual ICompositionContext CreateMessagingContainerMock()
        {
            var container = Substitute.For<ICompositionContext>();

            Func<object[], DispatchingContext> ctxCreator = args =>
                                    new DispatchingContext(
                                        container,
                                        Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                                        Substitute.For<IMessageBroker>(),
                                        Substitute.For<IAppRuntime>(),
                                        Substitute.For<IAuthenticationService>(),
                                        args.Length > 0 ? args[0] : null);

            container.GetExport(typeof(IExportFactoryImporter<IContextFactory>), Arg.Any<string>())
                .Returns(ci =>
                    new ExportFactoryImporter<IContextFactory>(
                        new ExportFactory<IContextFactory>(
                            () =>
                            {
                                return this.CreateContextFactoryMock(ctxCreator);
                            })));

            container.GetExport(typeof(IContextFactory), Arg.Any<string>())
                .Returns(ci => this.CreateContextFactoryMock(ctxCreator));

            container.GetExport<IContextFactory>(Arg.Any<string>())
                .Returns(ci => this.CreateContextFactoryMock(ctxCreator));

            return container;
        }
    }
}
