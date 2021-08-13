namespace CalculatorConsole.Operations
{
    [Operation("+", Name = "Addition")]
    public class AddOperation : IOperation
    {
        public double Compute(double op1, double op2) => op1 + op2;
    }
}
