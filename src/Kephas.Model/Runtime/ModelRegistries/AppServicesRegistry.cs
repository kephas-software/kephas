// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractsRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service contracts serviceRegistry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// An application service contracts serviceRegistry.
    /// </summary>
    public class AppServicesRegistry : IRuntimeModelRegistry
    {
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesRegistry"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public AppServicesRegistry(IAmbientServices ambientServices)
        {
            this.ambientServices = ambientServices;
            Requires.NotNull(ambientServices, nameof(ambientServices));
        }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            var appServiceInfos = this.ambientServices.GetAppServiceInfos();
            var types = new HashSet<Type>(appServiceInfos.Select(i => i.contractType));

            return Task.FromResult<IEnumerable<object>>(types);
        }
    }
}