// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScriptingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScriptingBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Contract for application services responsible for adding behavior to script execution for a specified language.
    /// </summary>
    [AppServiceContract(AllowMultiple = true)]
    public interface IScriptingBehavior
    {
        /// <summary>
        /// Interception invoked before the language service executes the script.
        /// </summary>
        /// <param name="executionContext">Information describing the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task BeforeExecuteAsync(IScriptingContext executionContext, CancellationToken token) => Task.CompletedTask;

        /// <summary>
        /// Interception invoked after the language service executed the script.
        /// </summary>
        /// <remarks>
        /// The interceptor may change the result or even replace it with another one.
        /// </remarks>
        /// <param name="executionContext">Information describing the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task AfterExecuteAsync(IScriptingContext executionContext, CancellationToken token) => Task.CompletedTask;
    }
}