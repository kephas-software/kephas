namespace CalculatorConsole.Calculator
{
    using Kephas.Services;

    [SharedAppServiceContract]
    public interface ICalculator
    {
        (double Value, string OperationName) Compute(string input);
    }
}