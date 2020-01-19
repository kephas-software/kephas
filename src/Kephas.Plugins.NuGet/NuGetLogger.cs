// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the nu get logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.NuGet
{
    using System.Threading.Tasks;

    using global::NuGet.Common;

    /// <summary>
    /// A NuGet logger.
    /// </summary>
    public class NuGetLogger : ILogger
    {
        private readonly Kephas.Logging.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NugetLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NuGetLogger(Kephas.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logs the data.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="data">The data.</param>
        public void Log(LogLevel level, string data)
        {
            this.logger.Log(this.GetLogLevel(level), null, data);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(ILogMessage message)
        {
            this.logger.Log(this.GetLogLevel(message.Level), null, message.Message);
        }

        /// <summary>
        /// Logs the data asynchronously.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task LogAsync(LogLevel level, string data)
        {
            this.logger.Log(this.GetLogLevel(level), null, data);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs the message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task LogAsync(ILogMessage message)
        {
            this.logger.Log(this.GetLogLevel(message.Level), null, message.Message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs a debug.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogDebug(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Debug, null, data);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogError(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Error, null, data);
        }

        /// <summary>
        /// Logs an information.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogInformation(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Info, null, data);
        }

        /// <summary>
        /// Logs information summary.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogInformationSummary(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Info, null, data);
        }

        /// <summary>
        /// Logs the data as minimal.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogMinimal(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Trace, null, data);
        }

        /// <summary>
        /// Logs the data as verbose.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogVerbose(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Trace, null, data);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogWarning(string data)
        {
            this.logger.Log(Kephas.Logging.LogLevel.Warning, null, data);
        }

        private Kephas.Logging.LogLevel GetLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    return Kephas.Logging.LogLevel.Error;
                case LogLevel.Warning:
                    return Kephas.Logging.LogLevel.Warning;
                case LogLevel.Information:
                    return Kephas.Logging.LogLevel.Info;
                case LogLevel.Debug:
                    return Kephas.Logging.LogLevel.Debug;
                case LogLevel.Verbose:
                case LogLevel.Minimal:
                    return Kephas.Logging.LogLevel.Trace;
                default:
                    return Kephas.Logging.LogLevel.Trace;
            }
        }
    }
}
