using System;

namespace CalculatorConsole.Operations
{
    [Operation("min", Name = "Minimum")]
    public class MinOperation : IOperation
    {
        public double Compute(double op1, double op2) => Math.Min(op1, op2);
    }
}
