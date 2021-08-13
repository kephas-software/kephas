// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The entry point class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CalculatorConsole
{
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;

    /// <summary>
    /// The entry point class.
    /// </summary>
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return new App(ambientServices => ambientServices
                .WithNLogManager()
                .WithStaticAppRuntime()
                .BuildWithAutofac()).BootstrapAsync(new AppArgs(args));
        }
    }
}
