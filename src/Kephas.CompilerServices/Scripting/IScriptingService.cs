// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScriptingService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScriptingService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CompilerServices.Scripting
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Shared application service for executing scripts.
    /// </summary>
    [SharedAppServiceContract]
    public interface IScriptingService
    {
        /// <summary>
        /// Executes the expression asynchronously.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="args">The arguments (optional).</param>
        /// <param name="executionContext">The execution context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        Task<object> ExecuteAsync(
            IScript script,
            IExpando args = null,
            IContext executionContext = null,
            CancellationToken cancellationToken = default);
    }
}