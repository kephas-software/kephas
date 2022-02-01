// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Kephas.Collections;
    using Kephas.Commands;
    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// Application arguments.
    /// </summary>
    public class AppArgs : Args, IAppArgs
    {
        /// <summary>
        /// Gets the name of the LogLevel application argument.
        /// </summary>
        public static readonly string LogLevelArgName = "LogLevel";

        /// <summary>
        /// Gets the name of the Service application argument.
        /// </summary>
        public static readonly string ServiceArgName = "Service";

        /// <summary>
        /// Gets the name of the Env application argument.
        /// </summary>
        public static readonly string EnvArgName = IAppRuntime.EnvKey;

        /// <summary>
        /// Gets the name of the AppId application argument.
        /// </summary>
        public static readonly string AppIdArgName = IAppRuntime.AppIdKey;

        /// <summary>
        /// Gets the name of the AppInstanceId application argument.
        /// </summary>
        public static readonly string AppInstanceIdArgName = IAppRuntime.AppInstanceIdKey;

        /// <summary>
        /// Gets the name of the Root application argument.
        /// </summary>
        public static readonly string RootArgName = "Root";

        /// <summary>
        /// Gets or sets the name prefix of environment variables which will be read
        /// to populate the arguments collection. This value should be set before doing anything
        /// related to application arguments.
        /// </summary>
        /// <remarks>
        /// Set this value with caution, as changing this prefix during application execution time
        /// can lead to unexpected results.
        /// </remarks>
        private static string? environmentAppArgsPrefix = "KEPHASAPP_";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class
        /// from the process's command line arguments. To initialize
        /// empty args use <c>new AppArgs(new string[0])</c>.
        /// </summary>
        public AppArgs()
            : base(ComputeAppArgs(ComputeArgs(System.Environment.GetCommandLineArgs())))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        public AppArgs(IEnumerable<string> appArgs)
            : base(ComputeAppArgs(ComputeArgs(appArgs)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public AppArgs(string commandLine)
            : base(ComputeAppArgs(ComputeArgs(commandLine)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="argValues">The argument values.</param>
        public AppArgs(IDictionary<string, object?> argValues)
            : base(ComputeAppArgs(argValues))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="args">The argument values.</param>
        public AppArgs(IDynamic args)
            : base(ComputeAppArgs(ComputeArgs(args.ToDictionary())))
        {
        }

        /// <summary>
        /// Gets or sets the application ID.
        /// </summary>
        public string? AppId
        {
            get => this[AppIdArgName] as string;
            set => this[AppIdArgName] = value;
        }

        /// <summary>
        /// Gets or sets the application instance ID.
        /// </summary>
        public string? AppInstanceId
        {
            get => this[AppInstanceIdArgName] as string;
            set => this[AppInstanceIdArgName] = value;
        }

        /// <summary>
        /// Gets or sets the environment name.
        /// </summary>
        [Display(ShortName = "Env")]
        public string? Environment
        {
            get => this[EnvArgName] as string;
            set => this[EnvArgName] = value;
        }

        /// <summary>
        /// Gets or sets the ID of the root application instance, if set.
        /// </summary>
        [Display(ShortName = "Root")]
        public string? RootAppInstanceId
        {
            get => this[RootArgName] as string;
            set => this[RootArgName] = value;
        }

        /// <summary>
        /// Gets the log level, if set.
        /// </summary>
        public virtual LogLevel? LogLevel
        {
            get
            {
                var logLevelString = this[LogLevelArgName] as string;
                if (!string.IsNullOrEmpty(logLevelString)
                    && Enum.TryParse<LogLevel>(logLevelString, ignoreCase: true, out var logLevel))
                {
                    return logLevel;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the application is executed as a Windows service (or Linux daemon).
        /// </summary>
        public virtual bool RunAsService => this.HasDynamicMember(ServiceArgName);

        /// <summary>
        /// Gets a value indicating whether this application instance is the root.
        /// </summary>
        public virtual bool RunAsRoot => string.IsNullOrEmpty(this.RootAppInstanceId);

        /// <summary>
        /// Gets a value indicating whether the application is in development mode.
        /// </summary>
        public virtual bool IsDevelopment =>
            string.Equals(EnvironmentName.Development, this[EnvArgName] as string, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Sets the name prefix of environment variables which will be read
        /// to populate the arguments collection. This value should be set before doing anything
        /// related to application arguments. A value of <c>null</c> indicates that no environment variables should be
        /// used.
        /// </summary>
        /// <remarks>
        /// Set this value with caution, as changing this prefix during application execution time
        /// can lead to unexpected results.
        /// </remarks>
        /// <param name="prefix">The prefix to use.</param>
        public static void SetEnvironmentAppArgsPrefix(string? prefix)
        {
            environmentAppArgsPrefix = prefix;
        }

        /// <summary>
        /// Computes the application arguments by merging into the provided arguments
        /// those read from environment variables, if the argument does not already exist.
        /// </summary>
        /// <param name="args">The raw arguments.</param>
        /// <returns>The arguments augumented with environment variables provided ones.</returns>
        protected static IDictionary<string, object?> ComputeAppArgs(IDictionary<string, object?> args)
        {
            if (environmentAppArgsPrefix == null)
            {
                return args;
            }

            var env = System.Environment.GetEnvironmentVariables();
            env.Keys
                .OfType<string>()
                .Where(k => k.StartsWith(environmentAppArgsPrefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(k => k, k => k[environmentAppArgsPrefix.Length..])
                .Where(kv => !string.IsNullOrEmpty(kv.Value) && !args.ContainsKey(kv.Value))
                .ForEach(kv => args[kv.Value] = env[kv.Key]);

            return args;
        }
    }
}