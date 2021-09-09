// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredLogEntryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the structured log entry provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Log4Net.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A structured log entry provider.
    /// </summary>
    internal class StructuredLogEntryProvider
    {
        private readonly Regex structuredMessage = new Regex("\\{[^\\{]*\\}", RegexOptions.Compiled);

        /// <summary>
        /// Gets the log entry.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The log entry.
        /// </returns>
        public (string message, object[]? positionalArgs, IDictionary<string, object>? namedArgs) GetLogEntry(string? messageFormat, params object[] args)
        {
            if (messageFormat == null || args == null || args.Length == 0)
            {
                return (messageFormat ?? string.Empty, args, null);
            }

            if (messageFormat.IndexOf('{') < 0)
            {
                return (messageFormat, args, null);
            }

            if (messageFormat.IndexOf("{0") >= 0)
            {
                return (messageFormat, args, null);
            }

            var match = this.structuredMessage.Match(messageFormat);
            var i = 0;
            var offset = 0;
            var messageBuilder = new StringBuilder(messageFormat);
            var namedArgs = new Dictionary<string, object>();
            while (match.Success)
            {
                var name = match.Value.Substring(1, match.Value.Length - 2);
                var formatIndex = name.IndexOf(':');
                var format = formatIndex >= 0 ? name.Substring(formatIndex + 1) : null;
                name = formatIndex >= 0 ? name.Substring(0, formatIndex) : name;
                var value = i < args.Length ? args[i] : null;
                namedArgs[name] = value;

                var stringValue =
                    string.IsNullOrEmpty(format)
                        ? value?.ToString()
                        : value is IFormattable formattableValue
                            ? formattableValue.ToString(format, CultureInfo.CurrentCulture)
                            : value?.ToString();
                var indexToDelete = match.Index + offset;
                offset = offset + (stringValue?.Length ?? 0) - match.Value.Length;
                messageBuilder.Remove(indexToDelete, match.Value.Length);
                messageBuilder.Insert(indexToDelete, stringValue);

                match = match.NextMatch();
                i++;
            }

            return (messageBuilder.ToString(), null, namedArgs);
        }
    }
}
