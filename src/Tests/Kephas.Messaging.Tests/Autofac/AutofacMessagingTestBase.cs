﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Injection.Autofac.Builder;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Testing.Application;
    using NSubstitute;

    public class AutofacMessagingTestBase : AutofacApplicationTestBase
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
                typeof(IMessageProcessor).GetTypeInfo().Assembly, /* Kephas.Messaging */
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
