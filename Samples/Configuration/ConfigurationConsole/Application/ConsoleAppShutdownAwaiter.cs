using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Operations;

namespace ConfigurationConsole.Application
{
    public class ConsoleAppShutdownAwaiter : IAppShutdownAwaiter
    {
        public ConsoleAppShutdownAwaiter(IConsole console)
        {
            Console = console;
        }

        public IConsole Console { get; }

        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
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
