using System;
using System.Threading;
using System.Threading.Tasks;
using CalculatorConsole.Calculator;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Operations;

namespace Calculator.AllowMultiple
{
    public class AppMainLoop : IAppMainLoop
    {
        private readonly ICalculator calculator;

        public AppMainLoop(IConsole console, ICalculator calculator)
        {
            Console = console;
            this.calculator = calculator;
        }

        public IConsole Console { get; }

        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine($"Application started.");

            Console.WriteLine(string.Empty);
            Console.WriteLine("Provide an operation in form of: term1 op term2. End the program with q instead of an operation.");

            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToLower().StartsWith("q"))
                {
                    break;
                }

                try
                {
                    var (value, operationName) = calculator.Compute(input);
                    Console.WriteLine($"Result is: {value} (using {operationName}).");
                    Console.WriteLine(string.Empty);
                }
                catch (Exception ex)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(ex.Message);

                    System.Console.ForegroundColor = ConsoleColor.White;
                }
            }

            return (new OperationResult(), AppShutdownInstruction.Shutdown);
        }
    }
}
