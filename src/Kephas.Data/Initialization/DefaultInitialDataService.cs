// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultInitialDataService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default initial data service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Initialization.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default initial data service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultInitialDataService : IInitialDataService
    {
        /// <summary>
        /// The initial data handler factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IInitialDataHandler, InitialDataHandlerMetadata>> initialDataHandlerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultInitialDataService"/> class.
        /// </summary>
        /// <param name="initialDataHandlerFactories">The initial data handler factories.</param>
        public DefaultInitialDataService(ICollection<IExportFactory<IInitialDataHandler, InitialDataHandlerMetadata>> initialDataHandlerFactories)
        {
            Requires.NotNull(initialDataHandlerFactories, nameof(initialDataHandlerFactories));

            this.initialDataHandlerFactories = initialDataHandlerFactories;
        }

        /// <summary>
        /// Creates the initial data asynchronously.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public async Task<object> CreateDataAsync(
            IInitialDataContext initialDataContext,
            CancellationToken cancellationToken = default)
        {
            var initialDataKinds = this.GetInitialDataKinds(initialDataContext);

            var initialDataHandlers = this.initialDataHandlerFactories
                .OrderBy(f => f.Metadata.ProcessingPriority)
                .Where(f => initialDataKinds == null || initialDataKinds.Contains(f.Metadata.InitialDataKind))
                .Select(f => f.CreateExportedValue())
                .ToList();

            var result = new List<object>();
            foreach (var initialDataHandler in initialDataHandlers)
            {
                var handlerResult = await initialDataHandler.CreateDataAsync(initialDataContext, cancellationToken).PreserveThreadContext();
                if (handlerResult != null)
                {
                    result.Add(handlerResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets initial data kinds.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <returns>
        /// The initial data kinds.
        /// </returns>
        protected virtual IList<string> GetInitialDataKinds(IInitialDataContext initialDataContext)
        {
            return initialDataContext?.InitialDataKinds?.ToList();
        }
    }
}