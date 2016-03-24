namespace CalculatorConsole.Calculator
{
    using Kephas.Services;

    [SharedAppServiceContract]
    public interface ICalculator
    {
        int Compute(string input);
    }
}