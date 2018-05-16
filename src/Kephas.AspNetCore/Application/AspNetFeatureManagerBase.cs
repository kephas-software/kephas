// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetFeatureManagerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application initializer base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.AspNetCore.Resources;

    /// <summary>
    /// Base OWIN implementation of a <see cref="IFeatureManager"/>.
    /// </summary>
    public abstract class AspNetFeatureManagerBase : FeatureManagerBase
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
            if (!(appContext is IAspNetAppContext aspNetAppContext))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AspNetFeatureManager_InvalidAppContext_Exception,
                        appContext?.GetType().FullName,
                        typeof(IAspNetAppContext).FullName));
            }

            return this.InitializeCoreAsync(aspNetAppContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task InitializeCoreAsync(IAspNetAppContext appContext, CancellationToken cancellationToken)
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
        protected override sealed Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            if (!(appContext is IAspNetAppContext aspNetAppContext))
            {
                throw new InvalidOperationException(string.Format(Strings.AspNetFeatureManager_InvalidAppContext_Exception, appContext?.GetType().FullName, typeof(IAspNetAppContext).FullName));
            }

            return this.FinalizeCoreAsync(aspNetAppContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task FinalizeCoreAsync(IAspNetAppContext appContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}