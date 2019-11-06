// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAuthorizationScopeService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default authorization scope service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default authorization scope service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAuthorizationScopeService : IAuthorizationScopeService
    {
        private readonly IList<IAuthorizationScopeProvider> providers;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorizationScopeService"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="providerFactories">The provider factories.</param>
        public DefaultAuthorizationScopeService(
            IContextFactory contextFactory,
            ICollection<IExportFactory<IAuthorizationScopeProvider, AppServiceMetadata>> providerFactories)
        {
            this.providers = providerFactories.Order().GetServices().ToList();
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        public async Task<object> GetAuthorizationScopeAsync(Action<IContext> optionsConfig, CancellationToken cancellationToken = default)
        {
            using (var context = this.CreateScopeContext(optionsConfig))
            {
                foreach (var provider in this.providers)
                {
                    var (scope, canResolve) = await provider.GetAuthorizationScopeAsync(context, cancellationToken)
                                         .PreserveThreadContext();
                    if (canResolve)
                    {
                        return scope;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Creates the scope context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new scope context.
        /// </returns>
        protected virtual IContext CreateScopeContext(Action<IContext> optionsConfig = null)
        {
            return this.contextFactory.CreateContext<Context>().Merge(optionsConfig);
        }
    }
}