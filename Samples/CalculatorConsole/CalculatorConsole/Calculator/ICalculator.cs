namespace CalculatorConsole.Calculator
{
    using Kephas.Services;

    [SharedAppServiceContract]
    public interface ICalculator
    {
        (int Value, string OperationName) Compute(string input);
    }
}