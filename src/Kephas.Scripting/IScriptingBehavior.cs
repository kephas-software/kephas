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

    using Kephas.Scripting.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract responsible for adding behavior to script execution for a specified language.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IScriptingBehavior
    {
        /// <summary>
        /// Interception called before invoking the language service to execute the script.
        /// </summary>
        /// <param name="executionContext">Information describing the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task BeforeExecuteAsync(IScriptingContext executionContext, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the language service to execute the script.
        /// </summary>
        /// <remarks>
        /// The execution data contains the execution result. 
        /// The interceptor may change the result or even replace it with another one.
        /// </remarks>
        /// <param name="executionContext">Information describing the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task AfterExecuteAsync(IScriptingContext executionContext, CancellationToken token);
    }
}