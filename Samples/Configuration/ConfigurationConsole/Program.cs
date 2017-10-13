// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ConfigurationConsole
{
    using System;
    using System.Threading.Tasks;

    using StartupConsole.Application;

    class Program
    {
        public static async Task Main(string[] args)
        {
            await new ConsoleShell().StartAppAsync();

            Console.ReadLine();
        }
    }
}
