﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application initializer for the model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Feature manager for the model.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class ModelAppLifecycleBehavior : IAppLifecycleBehavior
    {
        /// <summary>
        /// The model space provider.
        /// </summary>
        private readonly IModelSpaceProvider modelSpaceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public ModelAppLifecycleBehavior(IModelSpaceProvider modelSpaceProvider)
        {
            this.modelSpaceProvider = modelSpaceProvider ?? throw new ArgumentNullException(nameof(modelSpaceProvider));
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            var result = new OperationResult(true);
            await this.modelSpaceProvider.InitializeAsync(appContext, cancellationToken).PreserveThreadContext();
            return result.Complete();
        }
    }
}