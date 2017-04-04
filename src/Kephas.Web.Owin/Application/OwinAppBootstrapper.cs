// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinAppBootstrapper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the owin application bootstrapper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Owin.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Web.Owin.Resources;

    /// <summary>
    /// An OWIN application bootstrapper.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class OwinAppBootstrapper : DefaultAppManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwinAppBootstrapper"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appLifecycleBehaviorFactories">The application lifecycle behavior factories.</param>
        /// <param name="featureManagerFactories">The feature manager factories.</param>
        /// <param name="featureLifecycleBehaviorFactories">The feature lifecycle behavior factories.</param>
        public OwinAppBootstrapper(IAppManifest appManifest, IAmbientServices ambientServices, ICollection<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>> appLifecycleBehaviorFactories, ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories, ICollection<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>> featureLifecycleBehaviorFactories) 
            : base(appManifest, ambientServices, appLifecycleBehaviorFactories, featureManagerFactories, featureLifecycleBehaviorFactories)
        {
        }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owinAppContext = appContext as IOwinAppContext;
            if (owinAppContext == null)
            {
                throw new InvalidOperationException(string.Format(Strings.OwinFeatureManager_InvalidOwinAppContext_Exception, appContext?.GetType().FullName, typeof(IOwinAppContext).FullName));
            }

            return base.InitializeAppAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Finaliyes the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owinAppContext = appContext as IOwinAppContext;
            if (owinAppContext == null)
            {
                throw new InvalidOperationException(string.Format(Strings.OwinFeatureManager_InvalidOwinAppContext_Exception, appContext?.GetType().FullName, typeof(IOwinAppContext).FullName));
            }

            return base.FinalizeAppAsync(appContext, cancellationToken);
        }
    }
}