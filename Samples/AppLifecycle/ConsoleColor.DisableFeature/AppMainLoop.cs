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

namespace ConsoleColor.DisableFeature
{
    public class AppMainLoop : IAppMainLoop
    {
        public AppMainLoop(IConsole console)
        {
            Console = console;
        }

        public IConsole Console { get; }

        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine($"Application started.");
            Console.WriteLine($"Modify GreenConsoleEnabledServiceBehaviorRule class for disabling the GreenConsole feature.");

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press any key to end the program.");
            Console.ReadLine();

            return (new OperationResult(), AppShutdownInstruction.Shutdown);
        }
    }
}
