// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetFeatureManagerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application initializer base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.AspNetCore.Resources;
    using Kephas.Threading.Tasks;

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
        protected sealed override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return this.InitializeCoreAsync(GetAspNetAppContext(appContext), cancellationToken);
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
            return Task.CompletedTask;
        }

        /// <summary>
        /// Finalizes the OWIN feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected sealed override Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return this.FinalizeCoreAsync(GetAspNetAppContext(appContext), cancellationToken);
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
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAspNetAppContext GetAspNetAppContext(IAppContext appContext)
        {
            return appContext is IAspNetAppContext aspNetAppContext
                ? aspNetAppContext
                : throw new InvalidOperationException(
                    string.Format(
                        Strings.AspNetFeatureManager_InvalidAppContext_Exception,
                        appContext?.GetType().FullName,
                        typeof(IAspNetAppContext).FullName));
        }
    }
}