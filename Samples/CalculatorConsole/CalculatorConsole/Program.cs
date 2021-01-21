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

    using CalculatorConsole.Application;
    using Kephas.Application;

    /// <summary>
    /// The entry point class.
    /// </summary>
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return new CalculatorShell().BootstrapAsync(new AppArgs(args));
        }
    }
}
