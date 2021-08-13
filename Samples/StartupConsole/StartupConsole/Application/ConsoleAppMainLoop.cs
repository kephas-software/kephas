// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleAppMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The entry point class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Operations;

namespace StartupConsole.Application
{
    public class ConsoleAppMainLoop : IAppMainLoop
    {
        public ConsoleAppMainLoop(IConsole console)
        {
            this.Console = console;
        }

        public IConsole Console { get; }

        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
        {
            this.Console.WriteLine(string.Empty);
            this.Console.WriteLine($"Application started.");

            this.Console.WriteLine(string.Empty);
            this.Console.WriteLine("Press any key to end the program.");
            this.Console.ReadLine();

            return (new OperationResult(), AppShutdownInstruction.Shutdown);
        }
    }
}
