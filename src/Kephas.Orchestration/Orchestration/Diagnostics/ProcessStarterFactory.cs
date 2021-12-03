// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessStarterFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Factory service for process starters.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class ProcessStarterFactory : IProcessStarterFactory
    {
        private readonly ProcessStartInfo processStartInfo = new ProcessStartInfo();
        private readonly IList<string> arguments = new List<string>();
        private readonly ILogManager? logManager;
        private string? executableFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStarterFactory"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public ProcessStarterFactory(IAppRuntime appRuntime, ILogManager? logManager = null)
        {
            appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

            if (!RuntimeEnvironment.IsWindows())
            {
                // include all app paths in the library paths.
                var appBinDirectories = appRuntime.GetAppBinLocations();
                var libraryPath = string.Join(":", appBinDirectories);

                this.WithEnvironmentVariable(RuntimeEnvironment.LibraryPathEnvVariable, libraryPath);
            }

            this.WithShell(false);
            this.logManager = logManager;
        }

        /// <summary>
        /// Sets an environment variable for the process.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The variable value.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithEnvironmentVariable(string name, string value)
        {
            this.processStartInfo.EnvironmentVariables[name] = value;

            return this;
        }

        /// <summary>
        /// Sets the executable file as an operating system native process.
        /// </summary>
        /// <param name="executableFile">Full pathname of the executable file.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithNativeExecutable(string executableFile)
        {
            if (!string.IsNullOrEmpty(this.executableFile))
            {
                throw new InvalidOperationException($"The executable is already set to {this.executableFile}.");
            }

            this.executableFile = executableFile;
            this.processStartInfo.FileName = executableFile;

            return this;
        }

        /// <summary>
        /// Sets the executable file as a managed process.
        /// </summary>
        /// <param name="executableFile">Full pathname of the executable file.</param>
        /// <param name="runtime">Optional. The runtime executing the entry assembly.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithManagedExecutable(string executableFile, string? runtime = null)
        {
            if (!string.IsNullOrEmpty(this.executableFile))
            {
                throw new InvalidOperationException($"The executable is already set to {this.executableFile}.");
            }

            this.executableFile = executableFile;

            if (string.IsNullOrEmpty(runtime))
            {
                this.processStartInfo.FileName = executableFile;
            }
            else
            {
                this.processStartInfo.FileName = runtime;
                this.arguments.Insert(0, executableFile);
            }

            return this;
        }

        /// <summary>
        /// Sets the process arguments.
        /// </summary>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithArguments(params string[] args)
        {
            this.arguments.AddRange(args.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()));

            return this;
        }

        /// <summary>
        /// With process information configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithProcessStartInfoConfig(Action<ProcessStartInfo> config)
        {
            config?.Invoke(this.processStartInfo);

            return this;
        }

        /// <summary>
        /// Sets the process working directory.
        /// </summary>
        /// <param name="workingDirectory">Pathname of the working directory.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithWorkingDirectory(string workingDirectory)
        {
            this.processStartInfo.WorkingDirectory = workingDirectory;

            return this;
        }

        /// <summary>
        /// Indicates whether the process should use the shell execution (for console display).
        /// </summary>
        /// <param name="useShell">Optional. True to use shell.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        public IProcessStarterFactory WithShell(bool useShell = false)
        {
            this.processStartInfo.UseShellExecute = useShell;
            this.processStartInfo.RedirectStandardOutput = !useShell;
            this.processStartInfo.RedirectStandardError = !useShell;

            return this;
        }

        /// <summary>
        /// Gets the process start information.
        /// </summary>
        /// <returns>
        /// The process start information.
        /// </returns>
        public ProcessStartInfo GetProcessStartInfo()
        {
            if (string.IsNullOrEmpty(this.executableFile))
            {
                throw new InvalidOperationException("The executable file name is not set.");
            }

            this.processStartInfo.Arguments = string.Join(" ", this.arguments);
            return this.processStartInfo;
        }

        /// <summary>
        /// Creates the process launcher.
        /// </summary>
        /// <returns>
        /// The new <see cref="ProcessStarter"/>.
        /// </returns>
        public IProcessStarter CreateProcessStarter()
        {
            return new ProcessStarter(this.GetProcessStartInfo(), this.logManager);
        }
    }
}