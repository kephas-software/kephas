// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppSetupHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Singleton service handling one aspect of the application setup.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IAppSetupHandler
    {
        /// <summary>
        /// Performs one step in the application setup.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<IOperationResult> SetupAsync(IContext appContext, CancellationToken cancellationToken = default);
    }
}