// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppTerminationAwaiter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// The result of a main loop operation.
    /// </summary>
    /// <param name="Result">The operation result.</param>
    /// <param name="Instruction">The shutdown instruction.</param>
    public record MainLoopResult(IOperationResult Result, AppShutdownInstruction Instruction);

    /// <summary>
    /// Singleton application service contract for the service executing the application's main loop.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IAppMainLoop
    {
        /// <summary>
        /// Executes the application's main loop asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The application lifetime token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        Task<MainLoopResult> Main(CancellationToken cancellationToken);
    }
}
