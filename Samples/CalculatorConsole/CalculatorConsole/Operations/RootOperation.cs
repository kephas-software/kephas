namespace CalculatorConsole.Operations
{
    using System;

    [Operation("r", Name = "Root")]
    public class RootOperation : IOperation
    {
        public double Compute(double op1, double op2) => Math.Pow(op1, 1 / op2);
    }
}