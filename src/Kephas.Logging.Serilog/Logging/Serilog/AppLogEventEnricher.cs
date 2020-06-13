// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLogEventEnricher.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using global::Serilog.Core;
    using global::Serilog.Events;
    using Kephas.Application;

    /// <summary>
    /// Log event enricher adding the application ID and instance ID to the properties. 
    /// </summary>
    public class AppLogEventEnricher : ILogEventEnricher
    {
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLogEventEnricher"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        public AppLogEventEnricher(IAppRuntime appRuntime)
        {
            this.appRuntime = appRuntime;
        }

        /// <summary>
        /// Enriches the log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="propertyFactory">The property factory.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "AppId", this.appRuntime.GetAppId()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "AppInstanceId", this.appRuntime.GetAppInstanceId()));
        }
    }
}