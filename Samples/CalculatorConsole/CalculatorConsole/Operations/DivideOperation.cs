namespace CalculatorConsole.Operations
{
    [Operation("/")]
    public class DivideOperation : IOperation
    {
        public int Compute(int op1, int op2)
        {
            return op1 / op2;
        }
    }
}