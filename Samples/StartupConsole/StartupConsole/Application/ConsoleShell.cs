// --------------------------------------------------------------------------------------------------------------------
// <copyright file="consoleshell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the consoleshell class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StartupConsole.Application
{
    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.NLog;

    /// <summary>
    /// A console shell.
    /// </summary>
    public class ConsoleShell : AppBase
    {
        protected override void BuildServicesContainer(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithStaticAppRuntime()
                .BuildWithLite();
        }
    }
}