// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The entry point class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StartupConsole
{
    using System;
    using System.Threading.Tasks;
using Kephas;
    using Kephas.Application;
    using StartupConsole.Application;

    /// <summary>
    /// The entry point class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry-point for this application.
        /// </summary>
        /// <param name="args">Array of command-line argument strings.</param>
        public static Task Main(string[] args)
        {
            return new App(ambientServices =>
                ambientServices
                    .WithNLogManager()
                    .WithStaticAppRuntime()
                    .BuildWithLite()).BootstrapAsync(new AppArgs(args));
        }
    }
}
