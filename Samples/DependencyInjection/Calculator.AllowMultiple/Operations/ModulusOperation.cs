namespace CalculatorConsole.Operations
{
    [Operation("%", Name = "Modulus")]
    public class ModulusOperation : IOperation
    {
        public double Compute(double op1, double op2) => op1 % op2;
    }
}