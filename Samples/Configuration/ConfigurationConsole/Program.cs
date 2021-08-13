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
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Application;

    class Program
    {
        public static async Task Main(string[] args)
        {
            await new App(ambientServices =>
                ambientServices
                    .WithNLogManager()
                    .WithDynamicAppRuntime()
                    .BuildWithAutofac()).BootstrapAsync(new AppArgs(args));
        }
    }
}
