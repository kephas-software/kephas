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
    using System;

    using CalculatorConsole.Application;

    /// <summary>
    /// The entry point class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry-point for this application.
        /// </summary>
        /// <param name="args">Array of command-line argument strings.</param>
        public static void Main(string[] args)
        {
            var shell = new CalculatorShell();
            shell.StartAppAsync();
        }
    }
}
