namespace CalculatorConsole.Operations
{
    [Operation("*")]
    public class MultiplyOperation : IOperation
    {
        public int Compute(int op1, int op2)
        {
            return op1 * op2;
        }
    }
}