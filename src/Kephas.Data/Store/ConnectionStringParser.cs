﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringParser.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Dynamic;

    /// <summary>
    /// A connection string parser.
    /// </summary>
    public static class ConnectionStringParser
    {
        /// <summary>
        /// Parses the provided connection string as <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// An dictionary of configuration parameters.
        /// </returns>
        public static IDictionary<string, string> AsDictionary(string connectionString)
        {
            Requires.NotNullOrEmpty(connectionString, nameof(connectionString));

            var keyValuePairs = GetConnectionStringSplits(connectionString)
                                    .ToDictionary(
                                            kvp => kvp[0].Trim(),
                                            kvp => kvp[1].Trim(),
                                            StringComparer.OrdinalIgnoreCase);

            return keyValuePairs;
        }

        /// <summary>
        /// Parses the provided connection string to an <see cref="IExpandoBase"/>.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// An <see cref="IExpandoBase"/> containing the configuration parameters.
        /// </returns>
        public static IExpandoBase ToExpando(string connectionString)
        {
            Requires.NotNullOrEmpty(connectionString, nameof(connectionString));

            var expando = new Expando(GetConnectionStringSplits(connectionString)
                                    .ToDictionary(
                                        kvp => kvp[0].Trim(),
                                        kvp => (object?)kvp[1].Trim(),
                                        StringComparer.OrdinalIgnoreCase));

            return expando;
        }

        /// <summary>
        /// Gets the connection string splits.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// The connection string splits.
        /// </returns>
        private static IEnumerable<string[]> GetConnectionStringSplits(string connectionString)
        {
            return connectionString.Split(';')
                .Where(kvp => kvp.Contains('='))
                .Select(kvp => kvp.Split(new[] { '=' }, 2));
        }
    }
}