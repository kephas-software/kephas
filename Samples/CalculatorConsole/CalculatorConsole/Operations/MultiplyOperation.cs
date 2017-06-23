namespace CalculatorConsole.Operations
{
    [Operation("*", Name = "Multiplication")]
    public class MultiplyOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 * op2;
    }
}