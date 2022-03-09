// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of the <see cref="IHostInfoProvider"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class HostInfoProvider : IHostInfoProvider
    {
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostInfoProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        public HostInfoProvider(IAppRuntime appRuntime)
        {
            this.appRuntime = appRuntime;
        }

        /// <summary>
        /// Gets host address.
        /// </summary>
        /// <returns>
        /// The host address.
        /// </returns>
        public virtual IPAddress GetHostAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            return hostEntry.AddressList.Last(add => add.AddressFamily == AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Gets the name of the host where the application process runs.
        /// </summary>
        /// <returns>
        /// The host name.
        /// </returns>
        public virtual string GetHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Gets the application information for the provided runtime.
        /// </summary>
        /// <returns>
        /// The application information.
        /// </returns>
        public virtual IRuntimeAppInfo GetRuntimeAppInfo()
        {
            var process = Process.GetCurrentProcess();
            return new RuntimeAppInfo
            {
                AppId = this.appRuntime.GetAppId()!,
                AppInstanceId = this.appRuntime.GetAppInstanceId()!,
                IsRoot = this.appRuntime.IsRoot(),
#if NETSTANDARD2_1
                ProcessId = process.Id,
#else
                ProcessId = System.Environment.ProcessId,
#endif
                PrivateMemorySize = process.PrivateMemorySize64,
                PagedMemorySize = process.PagedMemorySize64,
                VirtualMemorySize = process.VirtualMemorySize64,
                Features = this.appRuntime.GetFeatures().Select(f => f.Name).ToArray(),
                HostName = this.GetHostName(),
                HostAddress = this.GetHostAddress().ToString(),
            };
        }
    }
}
