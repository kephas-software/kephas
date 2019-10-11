// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    /// <summary>
    /// A service helper.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncInitializable"/>, the <see cref="IAsyncInitializable.InitializeAsync(IContext, CancellationToken)"/> method is called and its result is returned.
        /// If the service implements <see cref="IInitializable"/>, the <see cref="IInitializable.Initialize(IContext)"/> method is called and a completed task is returned.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task InitializeAsync(object service, IContext context = null, CancellationToken cancellationToken = default)
        {
            if (service is IAsyncInitializable asyncService)
            {
                return asyncService.InitializeAsync(context, cancellationToken);
            }

            if (service is IInitializable syncService)
            {
                syncService.Initialize(context);
                return TaskHelper.CompletedTask;
            }

            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncInitializable"/>, the <see cref="IAsyncInitializable.InitializeAsync(IContext, CancellationToken)"/> method is synchronously called.
        /// If the service implements <see cref="IInitializable"/>, the <see cref="IInitializable.Initialize(IContext)"/> method is called.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        public static void Initialize(object service, IContext context = null)
        {
            if (service is IAsyncInitializable asyncService)
            {
                asyncService.InitializeAsync(context).WaitNonLocking();
            }
            else if (service is IInitializable syncService)
            {
                syncService.Initialize(context);
            }
        }

        /// <summary>
        /// Finalizes the service asynchronously.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncFinalizable"/>, the <see cref="IAsyncFinalizable.FinalizeAsync(IContext, CancellationToken)"/> method is called and its result is returned.
        /// If the service implements <see cref="IFinalizable"/>, the <see cref="IFinalizable.Finalize(IContext)"/> method is called and a completed task is returned.
        /// If the service implements <see cref="IDisposable"/>, the <see cref="IDisposable.Dispose"/> method is called and a completed task is returned.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task FinalizeAsync(object service, IContext context = null, CancellationToken cancellationToken = default)
        {
            if (service is IAsyncFinalizable asyncService)
            {
                return asyncService.FinalizeAsync(context, cancellationToken);
            }

            if (service is IFinalizable syncService)
            {
                syncService.Finalize(context);
                return TaskHelper.CompletedTask;
            }

            if (service is IDisposable disposableService)
            {
                disposableService.Dispose();
                return TaskHelper.CompletedTask;
            }

            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <remarks>
        /// If the service implements <see cref="IAsyncFinalizable"/>, the <see cref="IAsyncFinalizable.FinalizeAsync(IContext, CancellationToken)"/> method is called synchronously.
        /// If the service implements <see cref="IFinalizable"/>, the <see cref="IFinalizable.Finalize(IContext)"/> method is called.
        /// If the service implements <see cref="IDisposable"/>, the <see cref="IDisposable.Dispose"/> method is called.
        /// Otherwise nothing happens.
        /// </remarks>
        /// <param name="service">The service.</param>
        /// <param name="context">Optional. The context.</param>
        public static void Finalize(object service, IContext context = null)
        {
            if (service is IAsyncFinalizable asyncService)
            {
                asyncService.FinalizeAsync(context).WaitNonLocking();
            }
            else if (service is IFinalizable syncService)
            {
                syncService.Finalize(context);
            }
            else if (service is IDisposable disposableService)
            {
                disposableService.Dispose();
            }
        }
    }
}
