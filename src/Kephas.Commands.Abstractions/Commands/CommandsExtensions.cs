// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Extension methods for commands.
    /// </summary>
    public static class CommandsExtensions
    {
        /// <summary>
        /// Converts this arguments list to a list of string arguments for use in command lines.
        /// </summary>
        /// <returns>A list of string arguments.</returns>
        public static IEnumerable<string> ToCommandArgs(this IDynamic args)
        {
            _ = args ?? throw new ArgumentNullException(nameof(args));

            return args.ToDictionary().ToCommandArgs();
        }

        /// <summary>
        /// Converts the app arguments list to a list of string arguments for use in command lines.
        /// </summary>
        /// <param name="arguments">The arguments as a dictionary.</param>
        /// <returns>A list of string arguments.</returns>
        public static IEnumerable<string> ToCommandArgs(this IDictionary<string, object?> arguments)
        {
            static string ToParamValue(object? o) =>
                o switch
                {
                    null => string.Empty,
                    bool boolean => $"{boolean.ToString().ToLower()}",
                    int integer => $"{integer}",
                    string str => string.IsNullOrEmpty(str) ? str : $"\"{str.Replace("\"", "\"\"")}\"",
                    DateTime dateTime => $"\"{dateTime:s}\"",
                    DateTimeOffset dateTimeOffset => $"\"{dateTimeOffset:s}\"",
                    _ => $"\"{o?.ToString()?.Replace("\"", "\"\"")}\""
                };

            foreach (var a in arguments)
            {
                yield return $"-{a.Key}";
                var paramValue = ToParamValue(a.Value);
                if (!string.IsNullOrEmpty(paramValue))
                {
                    yield return paramValue;
                }
            }
        }
    }
}
