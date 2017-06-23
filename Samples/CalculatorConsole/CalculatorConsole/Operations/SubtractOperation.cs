namespace CalculatorConsole.Operations
{
    [Operation("-", Name = "Subtraction")]
    public class SubtractOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 - op2;
    }
}