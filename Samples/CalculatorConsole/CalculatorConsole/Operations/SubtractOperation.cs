namespace CalculatorConsole.Operations
{
    [Operation("-")]
    public class SubtractOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 - op2;
    }
}