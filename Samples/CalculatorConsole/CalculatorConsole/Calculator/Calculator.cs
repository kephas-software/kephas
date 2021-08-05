namespace CalculatorConsole.Calculator
{
    using System.Collections.Generic;
    using System.Linq;

    using CalculatorConsole.Operations;

    using Kephas.Composition;

    public class Calculator : ICalculator
    {
        private readonly IParser parser;

        public Calculator(ICollection<IExportFactory<IOperation, OperationMetadata>> operationFactories, IParser parser)
        {
            this.parser = parser;
            this.OperationsDictionary = operationFactories.ToDictionary(
                e => e.Metadata.Operation,
                e => e);
        }

        public IDictionary<string, IExportFactory<IOperation, OperationMetadata>> OperationsDictionary { get; set; }

        public (double Value, string OperationName) Compute(string input)
        {
            var (op1, opName, op2) = this.parser.Parse(input, this.OperationsDictionary.Select(op => op.Key));
            var operation = this.OperationsDictionary[opName];
            return (operation.CreateExportedValue().Compute(op1, op2), operation.Metadata.OperationName);
        }
    }
}