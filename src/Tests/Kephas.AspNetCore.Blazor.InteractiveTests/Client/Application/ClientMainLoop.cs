// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Operations;
    using Kephas.Services;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

    /// <summary>
    /// The application's main loop.
    /// </summary>
    /// <seealso cref="IAppMainLoop" />
    [OverridePriority(Priority.High)]
    public class ClientMainLoop : IAppMainLoop
    {
        private readonly Lazy<WebAssemblyHost> lazyHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMainLoop"/> class.
        /// </summary>
        /// <param name="lazyHost">The lazy host.</param>
        public ClientMainLoop(Lazy<WebAssemblyHost> lazyHost)
        {
            this.lazyHost = lazyHost;
        }

        /// <summary>
        /// Executes the application's main loop asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The application lifetime token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public virtual async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken)
        {
            await this.lazyHost.Value.RunAsync();
            return (true.ToOperationResult(), AppShutdownInstruction.Shutdown);
        }
    }
}
