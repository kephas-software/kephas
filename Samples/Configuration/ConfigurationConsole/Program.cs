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

    using StartupConsole.Application;

    class Program
    {
        public static async Task Main(string[] args)
        {
            //AssemblyLoadContext.Default.Resolving += (context, name) =>
            //    {
            //        if (name.Name.EndsWith(".resources"))
            //        {
            //            return null;
            //        }

            //        return null;
            //    };

            await new ConsoleShell().BootstrapAsync(args);
        }
    }
}
