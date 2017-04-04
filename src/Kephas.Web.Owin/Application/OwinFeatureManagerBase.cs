// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinFeatureManagerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the owin application initializer base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Owin.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Web.Owin.Resources;

    /// <summary>
    /// Base OWIN implementation of a <see cref="IFeatureManager"/>.
    /// </summary>
    public abstract class OwinFeatureManagerBase : FeatureManagerBase
    {
        /// <summary>
        /// Initializes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override sealed Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            var owinAppContext = appContext as IOwinAppContext;
            if (owinAppContext == null)
            {
                throw new InvalidOperationException(string.Format(Strings.OwinFeatureManager_InvalidOwinAppContext_Exception, appContext?.GetType().FullName, typeof(IOwinAppContext).FullName));
            }

            return this.InitializeCoreAsync(owinAppContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task InitializeCoreAsync(IOwinAppContext appContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Finalizes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            var owinAppContext = appContext as IOwinAppContext;
            if (owinAppContext == null)
            {
                throw new InvalidOperationException(string.Format(Strings.OwinFeatureManager_InvalidOwinAppContext_Exception, appContext?.GetType().FullName, typeof(IOwinAppContext).FullName));
            }

            return this.FinalizeCoreAsync(owinAppContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task FinalizeCoreAsync(IOwinAppContext appContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}