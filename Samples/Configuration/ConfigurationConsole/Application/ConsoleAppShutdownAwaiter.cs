// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleAppShutdownAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Operations;

namespace ConfigurationConsole.Application
{
    public class ConsoleAppShutdownAwaiter : IAppMainLoop
    {
        public ConsoleAppShutdownAwaiter(IConsole console)
        {
            Console = console;
        }

        public IConsole Console { get; }

        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine($"Application started.");

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press any key to end the program.");
            Console.ReadLine();

            return (new OperationResult(), AppShutdownInstruction.Shutdown);
        }
    }
}
