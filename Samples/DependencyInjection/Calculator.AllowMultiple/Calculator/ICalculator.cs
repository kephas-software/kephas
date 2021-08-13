namespace CalculatorConsole.Calculator
{
    using Kephas.Services;

    [SingletonAppServiceContract]
    public interface ICalculator
    {
        (double Value, string OperationName) Compute(string input);
    }
}