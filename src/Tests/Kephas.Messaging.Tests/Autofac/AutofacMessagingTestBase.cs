// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacMessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Security.Authorization;
    using Kephas.Testing.Injection;
    using NSubstitute;

    public class AutofacMessagingTestBase : AutofacInjectionTestBase
    {
        public override IInjector CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0])
            {
                typeof(IMessageProcessor).Assembly,       /* Kephas.Messaging */
                typeof(IAppLifecycleBehavior).Assembly,   /* Kephas.Application.Abstractions */
                typeof(IAuthorizationService).Assembly,   /* Kephas.Security */
                typeof(IEventHub).Assembly,               /* Kephas.Interaction */
            };

            return base.CreateInjector(ambientServices, assemblyList, parts, config, logManager, appRuntime);
        }

        protected virtual IInjector CreateMessagingContainerMock()
        {
            var container = Substitute.For<IInjector>();

            return container;
        }
    }
}
