// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics
{
    using System;
    using System.Diagnostics;

    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Interface for process starter factory.
    /// </summary>
    [AppServiceContract]
    public interface IProcessStarterFactory
    {
        /// <summary>
        /// Sets the process arguments.
        /// </summary>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithArguments(params string[] args);

        /// <summary>
        /// Sets an environment variable for the process.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The variable value.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithEnvironmentVariable(string name, string value);

        /// <summary>
        /// Sets the executable file as a managed process.
        /// </summary>
        /// <param name="executableFile">Full pathname of the executable file or the entry assembly.</param>
        /// <param name="runtime">Optional. The runtime executing the entry assembly.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithManagedExecutable(string executableFile, string? runtime = null);

        /// <summary>
        /// Sets the executable file as an operating system native process.
        /// </summary>
        /// <param name="executableFile">Full pathname of the executable file.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithNativeExecutable(string executableFile);

        /// <summary>
        /// Adds further configuration to the process information.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithProcessStartInfoConfig(Action<ProcessStartInfo> config);

        /// <summary>
        /// Indicates whether the process should use the shell execution (for console display).
        /// </summary>
        /// <param name="useShell">Optional. True to use shell.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithShell(bool useShell = false);

        /// <summary>
        /// Sets the process working directory.
        /// </summary>
        /// <param name="workingDirectory">Pathname of the working directory.</param>
        /// <returns>
        /// This <see cref="IProcessStarterFactory"/>.
        /// </returns>
        IProcessStarterFactory WithWorkingDirectory(string workingDirectory);

        /// <summary>
        /// Creates the process starter.
        /// </summary>
        /// <returns>
        /// The new <see cref="ProcessStarter"/>.
        /// </returns>
        IProcessStarter CreateProcessStarter();

        /// <summary>
        /// Gets the process start information.
        /// </summary>
        /// <returns>
        /// The process start information.
        /// </returns>
        ProcessStartInfo GetProcessStartInfo();
    }
}