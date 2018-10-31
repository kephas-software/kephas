// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataSourceService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data source service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// A default data source service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataSourceService : IDataSourceService
    {
        /// <summary>
        /// The providers.
        /// </summary>
        private readonly ICollection<IDataSourceProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataSourceService"/> class.
        /// </summary>
        /// <param name="providerFactories">The provider factories.</param>
        public DefaultDataSourceService(ICollection<IExportFactory<IDataSourceProvider, AppServiceMetadata>> providerFactories)
        {
            Requires.NotNull(providerFactories, nameof(providerFactories));

            this.providers = providerFactories
                                        .OrderBy(f => f.Metadata.OverridePriority)
                                        .ThenBy(f => f.Metadata.ProcessingPriority)
                                        .Select(f => f.CreateExportedValue())
                                        .ToList();
        }

        /// <summary>
        /// Gets the data source asynchronously.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the data source.
        /// </returns>
        public Task<IEnumerable<object>> GetDataSourceAsync(
            IPropertyInfo propertyInfo,
            IDataSourceContext context,
            CancellationToken cancellationToken = default)
        {
            foreach (var provider in this.providers)
            {
                if (provider.CanHandle(propertyInfo, context))
                {
                    return provider.GetDataSourceAsync(propertyInfo, context, cancellationToken);
                }
            }

            return Task.FromResult((IEnumerable<object>)null);
        }
    }
}