namespace CalculatorConsole.Operations
{
    [Operation("*", Name = "Multiplication")]
    public class MultiplyOperation : IOperation
    {
        public double Compute(double op1, double op2) => op1 * op2;
    }
}