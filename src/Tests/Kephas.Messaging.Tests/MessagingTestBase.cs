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
    using Kephas.Injection.Builder;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Security.Authorization;
    using Kephas.Testing.Injection;
    using NSubstitute;

    public class MessagingTestBase : InjectionTestBase
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
                typeof(IMessageProcessor).Assembly,       /* Kephas.Messaging */
                typeof(IAppLifecycleBehavior).Assembly,   /* Kephas.Application.Abstractions */
                typeof(IAuthorizationService).Assembly,   /* Kephas.Security */
                typeof(IEventHub).Assembly,               /* Kephas.Interaction */
            };

            return base.BuildServiceProvider(ambientServices, assemblyList, parts, config, logManager, appRuntime);
        }

        protected virtual IServiceProvider CreateMessagingContainerMock()
        {
            var container = Substitute.For<IServiceProvider>();

            return container;
        }
    }
}
