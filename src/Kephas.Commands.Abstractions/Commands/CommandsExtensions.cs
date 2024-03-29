﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// Converts the provided expando arguments to <see cref="IArgs"/>.
        /// </summary>
        /// <typeparam name="TArgs">The arguments type.</typeparam>
        /// <param name="args">The expando arguments.</param>
        /// <returns>The same instance, if it is convertible to <see cref="IArgs"/>, otherwise app args constructed on the provided <paramref name="args"/>.</returns>
        public static TArgs AsArgs<TArgs>(this IDynamic args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return args is TArgs appArgs ? appArgs : (TArgs)Activator.CreateInstance(typeof(TArgs), args)!;
        }

        /// <summary>
        /// Converts the provided expando arguments to <see cref="IArgs"/>.
        /// </summary>
        /// <param name="args">The expando arguments.</param>
        /// <returns>The same instance, if it is convertible to <see cref="IArgs"/>, otherwise app args constructed on the provided <paramref name="args"/>.</returns>
        public static IArgs AsArgs(this IDynamic args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return args is IArgs appArgs ? appArgs : new Args(args);
        }

        /// <summary>
        /// Converts the provided expando arguments to <see cref="IArgs"/>.
        /// </summary>
        /// <typeparam name="TArgs">The arguments type.</typeparam>
        /// <param name="args">The expando arguments.</param>
        /// <returns>The same instance, if it is convertible to <see cref="IArgs"/>, otherwise app args constructed on the provided <paramref name="args"/>.</returns>
        public static TArgs AsArgs<TArgs>(this IDictionary<string, object?> args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return (TArgs)Activator.CreateInstance(typeof(TArgs), args)!;
        }

        /// <summary>
        /// Converts the provided expando arguments to <see cref="IArgs"/>.
        /// </summary>
        /// <param name="args">The expando arguments.</param>
        /// <returns>The same instance, if it is convertible to <see cref="IArgs"/>, otherwise app args constructed on the provided <paramref name="args"/>.</returns>
        public static IArgs AsArgs(this IDictionary<string, object?> args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return new Args(args);
        }

        /// <summary>
        /// Converts the app arguments list to a list of string arguments for use in command lines.
        /// </summary>
        /// <param name="arguments">The arguments as a dictionary.</param>
        /// <returns>A list of string arguments.</returns>
        public static IEnumerable<string> ToCommandArgs(this IDynamic arguments)
        {
            if (arguments is IArgs args)
            {
                return args.ToCommandArgs();
            }

            return arguments.ToDictionary().ToCommandArgs();
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