// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System.Net;

    using Kephas.Services;

    /// <summary>
    /// Provider for host information.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IHostInfoProvider
    {
        /// <summary>
        /// Gets host address.
        /// </summary>
        /// <returns>
        /// The host address.
        /// </returns>
        public IPAddress GetHostAddress();

        /// <summary>
        /// Gets the name of the host where the application process runs.
        /// </summary>
        /// <returns>
        /// The host name.
        /// </returns>
        public string GetHostName();

        /// <summary>
        /// Gets the application information for the provided runtime.
        /// </summary>
        /// <returns>
        /// The application information.
        /// </returns>
        public IRuntimeAppInfo GetRuntimeAppInfo();
    }
}
