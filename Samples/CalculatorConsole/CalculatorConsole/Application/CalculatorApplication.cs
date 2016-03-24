namespace CalculatorConsole.Application
{
    using CalculatorConsole.Calculator;

    using Kephas.Application;

    public class CalculatorApplication : ApplicationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorApplication"/> class.
        /// </summary>
        /// <param name="appBootstrapper">The application bootstrapper.</param>
        /// <param name="calculator">The calculator.</param>
        public CalculatorApplication(IAppBootstrapper appBootstrapper, ICalculator calculator)
            : base("calculator", appBootstrapper)
        {
            this.Calculator = calculator;
        }

        public ICalculator Calculator { get; }
    }
}