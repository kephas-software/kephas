// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppFinalizerBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAppFinalizerBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services.Composition;

    /// <summary>
    /// Behavior for intercepting application finalization.
    /// </summary>
    public interface IAppFinalizerBehavior
    {
        /// <summary>
        /// Interceptor called before an application finalizer starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw <see cref="OperationCanceledException"/>.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The application finalizer service metadata.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task BeforeFinalizeAsync(IAppContext appContext, AppServiceMetadata serviceMetadata, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Interceptor called after an application finalizer completes its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw <see cref="OperationCanceledException"/>.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The application finalizer service metadata.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task AfterFinalizeAsync(IAppContext appContext, AppServiceMetadata serviceMetadata, CancellationToken cancellationToken = default(CancellationToken));
    }
}