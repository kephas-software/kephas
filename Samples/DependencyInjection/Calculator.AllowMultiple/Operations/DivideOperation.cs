namespace CalculatorConsole.Operations
{
    [Operation("/", Name = "Division")]
    public class DivideOperation : IOperation
    {
        public double Compute(double op1, double op2) => op1 / op2;
    }
}