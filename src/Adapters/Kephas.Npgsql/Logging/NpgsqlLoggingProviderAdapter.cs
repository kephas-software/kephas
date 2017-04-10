// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlLoggingProviderAdapter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the Npgsql logging provider adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Npgsql.Logging
{
    using Kephas;
    using Kephas.Logging;

    using global::Npgsql.Logging;

    /// <summary>
    /// A NpgSql logging provider adapter.
    /// </summary>
    public class NpgsqlLoggingProviderAdapter : INpgsqlLoggingProvider
    {
        /// <summary>
        /// The log manager.
        /// </summary>
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlLoggingProviderAdapter"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public NpgsqlLoggingProviderAdapter(IAmbientServices ambientServices)
        {
            this.logManager = ambientServices.LogManager;
        }

        /// <summary>
        /// Creates a new INpgsqlLogger instance of the given name.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <returns>
        /// The new logger.
        /// </returns>
        public NpgsqlLogger CreateLogger(string name)
        {
            return new NpgsqlLoggerAdapter(this.logManager.GetLogger(name));
        }
    }
}