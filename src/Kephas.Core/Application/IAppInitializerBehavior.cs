// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppInitializerBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAppInitializerBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services.Composition;

    /// <summary>
    /// Behavior for intercepting application initialization.
    /// </summary>
    public interface IAppInitializerBehavior
    {
        /// <summary>
        /// Interceptor called before an application initializer starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt initialization, simply throw <see cref="OperationCanceledException"/>.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The application initializer service metadata.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task BeforeInitializeAsync(IAppContext appContext, AppServiceMetadata serviceMetadata, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Interceptor called after an application initializer completes its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt initialization, simply throw <see cref="OperationCanceledException"/>.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The application initializer service metadata.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task AfterInitializeAsync(IAppContext appContext, AppServiceMetadata serviceMetadata, CancellationToken cancellationToken = default(CancellationToken));
    }
}