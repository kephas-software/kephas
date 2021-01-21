// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ConfigurationConsole
{
    using System.Runtime.Loader;
    using System.Threading.Tasks;
    using Kephas.Application;
    using StartupConsole.Application;

    class Program
    {
        public static async Task Main(string[] args)
        {
            await new ConsoleShell().BootstrapAsync(new AppArgs(args));
        }
    }
}
