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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default authorization scope service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAuthorizationScopeService : IAuthorizationScopeService
    {
        /// <summary>
        /// The providers.
        /// </summary>
        private readonly IList<IAuthorizationScopeProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorizationScopeService"/> class.
        /// </summary>
        /// <param name="providerFactories">The provider factories.</param>
        public DefaultAuthorizationScopeService(ICollection<IExportFactory<IAuthorizationScopeProvider, AppServiceMetadata>> providerFactories)
        {
            this.providers = providerFactories.Order().GetServices().ToList();
        }

        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        public async Task<object> GetAuthorizationScopeAsync(IContext context, CancellationToken cancellationToken = default)
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
}