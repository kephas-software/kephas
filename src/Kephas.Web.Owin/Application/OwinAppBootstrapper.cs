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
    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Web.Owin.Resources;

    /// <summary>
    /// An OWIN application bootstrapper.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class OwinAppBootstrapper : DefaultAppBootstrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwinAppBootstrapper"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="appIntializerFactories">The app intializer factories.</param>
        public OwinAppBootstrapper(IAmbientServices ambientServices, ICompositionContext compositionContext, ICollection<IExportFactory<IAppInitializer, AppServiceMetadata>> appIntializerFactories)
            : base(ambientServices, compositionContext, appIntializerFactories)
        {
        }

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task StartAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owinAppContext = appContext as IOwinAppContext;
            if (owinAppContext == null)
            {
                throw new InvalidOperationException(string.Format(Strings.OwinAppBootstrapper_InvalidOwinAppContext_Exception, appContext?.GetType().FullName, typeof(IOwinAppContext).FullName));
            }

            return base.StartAsync(appContext, cancellationToken);
        }
    }
}