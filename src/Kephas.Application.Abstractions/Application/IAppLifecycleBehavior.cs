﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppLifecycleBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Singleton service contract for application lifecycle behavior.
    /// </summary>
    /// <remarks>
    /// An application lifecycle behavior intercepts the initialization and finalization of the application
    /// and reacts to them.
    /// </remarks>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IAppLifecycleBehavior
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> AfterAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> BeforeAppFinalizeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> AfterAppFinalizeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}