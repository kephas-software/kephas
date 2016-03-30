namespace CalculatorConsole.Application
{
    using CalculatorConsole.Calculator;

    using Kephas.Application;

    public class CalculatorAppManifest : AppManifestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorAppManifest"/> class.
        /// </summary>
        /// <param name="calculator">The calculator.</param>
        public CalculatorAppManifest(ICalculator calculator)
            : base("demo-calculator")
        {
            this.Calculator = calculator;
        }

        public ICalculator Calculator { get; }
    }
}