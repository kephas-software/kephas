namespace CalculatorConsole.Calculator
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for parser.
    /// </summary>
    [SharedAppServiceContract]
    public interface IParser
    {
        /// <summary>
        /// Parses the input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>
        /// A tuple of (term1, operation, term2).
        /// </returns>
        Tuple<double, string, double> Parse(string input, IEnumerable<string> operations);
    }
}