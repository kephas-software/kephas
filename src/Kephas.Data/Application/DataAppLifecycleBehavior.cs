// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Data.Runtime;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A data application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class DataAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public DataAppLifecycleBehavior(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.typeRegistry.RegisterFactory(new RuntimeEntityInfoFactory());

            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> AfterAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task<IOperationResult> BeforeAppFinalizeAsync(
            IContext appContext,
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
        /// A Task.
        /// </returns>
        public Task<IOperationResult> AfterAppFinalizeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}