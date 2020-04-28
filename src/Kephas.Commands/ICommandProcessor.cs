// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICommandProcessor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for the service processing commands.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ICommandProcessor
    {
        /// <summary>
        /// Processes the provided command with the provided arguments and returns the result.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The asynchronous result returning the output of the command execution.
        /// </returns>
        Task<object?> ProcessAsync(string command, IExpando? args = null, IContext? context = null, CancellationToken cancellationToken = default);
    }
}
