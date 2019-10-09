using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Operations;

namespace StartupConsole.Application
{
    public class ConsoleAppShutdownAwaiter : IAppShutdownAwaiter
    {
        public ConsoleAppShutdownAwaiter(IConsole console)
        {
            this.Console = console;
        }

        public IConsole Console { get; }

        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
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
