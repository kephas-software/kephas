namespace CalculatorConsole.Operations
{
    [Operation("/", Name = "Division")]
    public class DivideOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 / op2;
    }
}