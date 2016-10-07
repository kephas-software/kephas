namespace CalculatorConsole.Operations
{
    [Operation("+")]
    public class AddOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 + op2;
    }
}