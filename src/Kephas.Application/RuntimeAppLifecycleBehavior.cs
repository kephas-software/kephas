// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A runtime application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class RuntimeAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppRuntime appRuntime;
        private readonly IAppManifest appManifest;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="appManifest">The application manifest.</param>
        public RuntimeAppLifecycleBehavior(IAppRuntime appRuntime, IAppManifest appManifest)
        {
            this.appRuntime = appRuntime;
            this.appManifest = appManifest;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.appRuntime[AppRuntimeBase.AppIdKey] = this.appManifest.AppId;
            this.appRuntime[AppRuntimeBase.AppInstanceIdKey] = this.appManifest.AppInstanceId;
            this.appRuntime[AppRuntimeBase.AppVersionKey] = this.appManifest.AppVersion;
            if (this.appManifest is AppManifestBase writableManifest)
            {
                writableManifest.Features = this.appRuntime.GetFeatures();
            }

            return TaskHelper.CompletedTask;
        }
    }
}
