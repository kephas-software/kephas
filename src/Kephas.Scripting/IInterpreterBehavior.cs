// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterpreterBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInterpreterBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract responsible for adding behaviors to interpreters for a specified language.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(LanguageAttribute) })]
    public interface IInterpreterBehavior
    {
        /// <summary>
        /// Interception called before invoking the interpreter to execute the script.
        /// </summary>
        /// <param name="executionContext">Information describing the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task BeforeExecuteAsync(IScriptExecutionContext executionContext, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the interpreter to execute the script.
        /// </summary>
        /// <remarks>
        /// The execution data contains the execution result. The interceptor may change the result or even replace it with another one.
        /// </remarks>
        /// <param name="executionContext">Information describing the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task AfterExecuteAsync(IScriptExecutionContext executionContext, CancellationToken token);
    }
}