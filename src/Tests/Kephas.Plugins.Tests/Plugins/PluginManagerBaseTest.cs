// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlugnManagerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugn manager base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Plugins.Reflection;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class PluginManagerBaseTest
    {
        public class TestPluginManager : PluginManagerBase
        {
            public TestPluginManager(IAppRuntime appRuntime, IContextFactory contextFactory, IEventHub eventHub, IPluginDataProvider pluginDataProvider, ILogManager logManager = null)
                : base(appRuntime, contextFactory, eventHub, pluginDataProvider, logManager)
            {
            }

            public override Task<IOperationResult<IEnumerable<IPluginInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            protected override Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
