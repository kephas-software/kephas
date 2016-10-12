// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinAppInitializerBase.cs" company="Quartz Software SRL">
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
    /// Base OWIN implementation of an <see cref="IAppInitializer"/>.
    /// </summary>
    public abstract class OwinAppInitializerBase : AppInitializerBase
    {
        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected sealed override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            var owinAppContext = appContext as IOwinAppContext;
            if (owinAppContext == null)
            {
                throw new InvalidOperationException(string.Format(Strings.OwinAppBootstrapper_InvalidOwinAppContext_Exception, appContext?.GetType().FullName, typeof(IOwinAppContext).FullName));
            }

            return this.InitializeCoreAsync(owinAppContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected abstract Task InitializeCoreAsync(IOwinAppContext appContext, CancellationToken cancellationToken);
    }
}