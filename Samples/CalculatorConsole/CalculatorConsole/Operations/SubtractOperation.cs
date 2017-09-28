namespace CalculatorConsole.Operations
{
    [Operation("-", Name = "Subtraction")]
    public class SubtractOperation : IOperation
    {
        public double Compute(double op1, double op2) => op1 - op2;
    }
}