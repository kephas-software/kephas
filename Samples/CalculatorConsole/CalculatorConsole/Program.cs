// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The entry point class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CalculatorConsole
{
    using System.Threading.Tasks;

    using CalculatorConsole.Application;

    /// <summary>
    /// The entry point class.
    /// </summary>
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return new CalculatorShell().BootstrapAsync(args);
        }
    }
}
