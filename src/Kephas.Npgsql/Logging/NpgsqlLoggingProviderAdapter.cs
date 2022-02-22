// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlLoggingProviderAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the Npgsql logging provider adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Npgsql.Logging;

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
    /// <param name="logManager">The log manager.</param>
    public NpgsqlLoggingProviderAdapter(ILogManager logManager)
    {
        this.logManager = logManager;
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
