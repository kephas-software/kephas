// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Registrar application service for providing runtime elements used in building the model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for providing runtime elements used in building the model space.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IRuntimeModelRegistry
    {
        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding an enumeration of runtime elements.
        /// </returns>
        Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default);
    }
}