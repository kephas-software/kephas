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

    using Kephas.Commands;
    using Kephas.ComponentModel.DataAnnotations;
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
        public static readonly string EnvArgName = AppRuntimeBase.EnvKey;

        /// <summary>
        /// Gets the name of the AppId application argument.
        /// </summary>
        public static readonly string AppIdArgName = AppRuntimeBase.AppIdKey;

        /// <summary>
        /// Gets the name of the AppInstanceId application argument.
        /// </summary>
        public static readonly string AppInstanceIdArgName = AppRuntimeBase.AppInstanceIdKey;

        /// <summary>
        /// Gets the name of the Root application argument.
        /// </summary>
        public static readonly string RootArgName = "Root";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        public AppArgs()
            : base(ComputeArgs(System.Environment.GetCommandLineArgs()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        public AppArgs(IEnumerable<string> appArgs)
            : base(appArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public AppArgs(string commandLine)
            : base(commandLine)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="argValues">The argument values.</param>
        public AppArgs(IDictionary<string, object?> argValues)
            : base(argValues)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppArgs"/> class.
        /// </summary>
        /// <param name="args">The argument values.</param>
        public AppArgs(IExpando args)
            : base(args)
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
        [DisplayInfo(ShortName = "Env")]
        public string? Environment
        {
            get => this[EnvArgName] as string;
            set => this[EnvArgName] = value;
        }

        /// <summary>
        /// Gets or sets the ID of the root application instance, if set.
        /// </summary>
        [DisplayInfo(ShortName = "Root")]
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
    }
}