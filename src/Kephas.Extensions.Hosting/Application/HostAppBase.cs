// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostAppBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the host application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A host application base.
    /// </summary>
    public abstract class HostAppBase : AppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostAppBase"/> class.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        protected HostAppBase(IAmbientServices ambientServices = null)
            : base(ambientServices)
        {
        }

        /// <summary>
        /// Gets the host builder.
        /// </summary>
        /// <value>
        /// The host builder.
        /// </value>
        protected IHostBuilder HostBuilder { get; private set; }

        /// <summary>
        /// Bootstraps the application asynchronously.
        /// </summary>
        /// <param name="rawAppArgs">Optional. The application arguments.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override Task<(IAppContext appContext, AppShutdownInstruction instruction)> BootstrapAsync(
            string[] rawAppArgs = null,
            CancellationToken cancellationToken = default)
        {
            this.HostBuilder = this.CreateHostBuilder(rawAppArgs);

            return base.BootstrapAsync(rawAppArgs, cancellationToken);
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="rawAppArgs">The application arguments.</param>
        /// <returns>
        /// The new host builder.
        /// </returns>
        protected virtual IHostBuilder CreateHostBuilder(string[] rawAppArgs)
        {
            return Host.CreateDefaultBuilder(rawAppArgs);
        }
    }
}
