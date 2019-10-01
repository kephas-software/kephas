// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandShell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICommandShell interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for command shell.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ICommandShell
    {
        /// <summary>
        /// Starts processing commands asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task StartAsync(CancellationToken cancellationToken);
    }
}
