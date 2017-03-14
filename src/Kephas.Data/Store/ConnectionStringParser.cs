// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringParser.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the connection string parser class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A connection string parser.
    /// </summary>
    public static class ConnectionStringParser
    {
        /// <summary>
        /// Parses the provided connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// An dictionary of configuration parameters.
        /// </returns>
        public static IDictionary<string, string> Parse(string connectionString)
        {
            Requires.NotNullOrEmpty(connectionString, nameof(connectionString));

            var keyValuePairs = connectionString.Split(';')
                                                    .Where(kvp => kvp.Contains('='))
                                                    .Select(kvp => kvp.Split(new[] { '=' }, 2))
                                                    .ToDictionary(
                                                            kvp => kvp[0].Trim(),
                                                            kvp => kvp[1].Trim(),
                                                            StringComparer.OrdinalIgnoreCase);

            return keyValuePairs;
        }
    }
}