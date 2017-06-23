namespace CalculatorConsole.Operations
{
    [Operation("+", Name = "Addition")]
    public class AddOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 + op2;
    }
}