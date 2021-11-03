﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Security.Runtime;
    using Kephas.Services;

    /// <summary>
    /// An authorization application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class SecurityAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        public SecurityAppLifecycleBehavior(IRuntimeTypeRegistry typeRegistry)
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
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.typeRegistry.RegisterFactory(new SecurityTypeInfoFactory());

            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}