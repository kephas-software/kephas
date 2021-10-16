// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyAppManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Behaviors;

    [OverridePriority(Priority.Highest)]
    public class DummyAppManager : Loggable, IAppManager
    {
        public DummyAppManager(
            ICollection<IAppRuntime> appRuntime,
            Lazy<IInjector, AppServiceMetadata> injector,
            IServiceBehaviorProvider? serviceBehaviorProvider = null,
            // ICollection<IAppLifecycleBehavior>? appLifecycleBehaviorFactories = null,
            // ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>>? featureManagerFactories = null,
            // ICollection<IExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>>? featureLifecycleBehaviorFactories = null
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.Logger.Debug("Injector metadata: {metadata}", injector.Metadata);
        }

        public Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}