// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application arguments class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings retrieved from the application command line arguments.
    /// </summary>
    public class AppArgs : Expando, IAppArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        public AppArgs()
            : this(ComputeArgs(Environment.GetCommandLineArgs()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        public AppArgs(IEnumerable<string> appArgs)
            : this(ComputeArgs(appArgs))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public AppArgs(string commandLine)
            : this(ComputeArgs(commandLine))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="argValues">The argument values.</param>
        public AppArgs(IDictionary<string, object> argValues)
            : base(argValues)
        {
        }

        /// <summary>
        /// Calculates the arguments as a dictionary of values.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <returns>
        /// The calculated arguments.
        /// </returns>
        protected static IDictionary<string, object> ComputeArgs(string commandLine)
        {
            Requires.NotNull(commandLine, nameof(commandLine));

            var args = commandLine.Split(new[] { ' ', '\r', '\n', '\t' }, new[] { '"' });
            return ComputeArgs(args);
        }

        /// <summary>
        /// Calculates the arguments as a dictionary of values.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// The calculated arguments.
        /// </returns>
        protected static IDictionary<string, object> ComputeArgs(IEnumerable<string> appArgs)
        {
            Requires.NotNull(appArgs, nameof(appArgs));

            var cmdArgs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            var key = string.Empty;
            object value = null;
            var expectedValue = false;

            using var enumerator = appArgs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentArg = enumerator.Current;
                var keyStartIndex = 0;

                if (currentArg.StartsWith("--"))
                {
                    keyStartIndex = 2;
                }
                else if (currentArg.StartsWith("-"))
                {
                    keyStartIndex = 1;
                }
                else if (currentArg.StartsWith("/"))
                {
                    // "/SomeSwitch" is equivalent to "--SomeSwitch"
                    keyStartIndex = 1;
                }

                // if we received a new argument, but we expected a value, add the previous key with the value "true"
                if (expectedValue)
                {
                    expectedValue = false;

                    if (keyStartIndex > 0)
                    {
                        // set the previous key to true and continue with processing the current arg
                        cmdArgs[key] = true;
                    }
                    else
                    {
                        cmdArgs[key] = Unescape(currentArg);
                        continue;
                    }
                }

                // currentArg starts a new argument
                var separator = currentArg.IndexOf('=');

                if (separator >= 0)
                {
                    // currentArg specifies a key with value
                    key = Unescape(currentArg.Substring(keyStartIndex, separator - keyStartIndex));
                    value = Unescape(currentArg.Substring(separator + 1));
                }
                else
                {
                    // currentArg specifies only a key
                    // If there is no prefix in current argument, consider it as a key with value "true"
                    if (keyStartIndex == 0)
                    {
                        key = Unescape(currentArg);
                        value = true;
                    }
                    else
                    {
                        key = Unescape(currentArg.Substring(keyStartIndex));
                        expectedValue = true;
                    }
                }

                // Override value when key is duplicated. So we always have the last argument win.
                if (!expectedValue)
                {
                    cmdArgs[key] = value;
                }
            }

            if(expectedValue)
            {
                cmdArgs[key] = true;
            }

            return cmdArgs;
        }

        private static string Unescape(string value)
        {
#if NETSTANDARD2_1
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                value = value[1..^1];
                return value.Replace("\\\"", "\"");
            }
#else
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
                return value.Replace("\\\"", "\"");
            }
#endif

            return value;
        }
    }
}
