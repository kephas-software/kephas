namespace CalculatorConsole.Operations
{
    using System;

    [Operation("^", Name = "Power")]
    public class PowerOperation : IOperation
    {
        public double Compute(double op1, double op2) => Math.Pow(op1, op2);
    }
}