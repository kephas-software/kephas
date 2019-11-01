// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScriptProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScriptingEngine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service for executing scripts.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IScriptProcessor
    {
        /// <summary>
        /// Executes the provided script asynchronously.
        /// </summary>
        /// <param name="script">The script to be interpreted/executed.</param>
        /// <param name="args">Optional. The execution arguments.</param>
        /// <param name="executionContext">Optional. The execution context.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the execution result.
        /// </returns>
        Task<object> ExecuteAsync(
            IScript script,
            IExpando args = null,
            IContext executionContext = null,
            Action<IScriptingContext> optionsConfig = null,
            CancellationToken cancellationToken = default);
    }
}