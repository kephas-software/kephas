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
                e => e.CreateExport().Value);
        }

        public IDictionary<string, IOperation> OperationsDictionary { get; set; }

        public int Compute(string input)
        {
            var parsedOperation = this.parser.Parse(input, this.OperationsDictionary.Select(op => op.Key));
            var operation = this.OperationsDictionary[parsedOperation.Item2];
            return operation.Compute(parsedOperation.Item1, parsedOperation.Item3);
        }
    }
}