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

            var cmdArgs = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            var args = appArgs;
            foreach (var arg in args)
            {
                var argSplit = arg.Split(new[] { '=' }, new[] { '"' }).ToArray();
                if (argSplit.Length == 0)
                {
                    continue;
                }

                var key = argSplit[0].TrimStart('-', '/');
                var value = argSplit.Length == 1 ? string.Empty : argSplit[1].Replace("\"", string.Empty);
                cmdArgs[key] = value;
            }

            return cmdArgs;
        }
    }
}
