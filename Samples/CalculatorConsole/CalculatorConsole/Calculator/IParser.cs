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
        /// A Tuple&lt;int,string,int&gt;
        /// </returns>
        Tuple<int, string, int> Parse(string input, IEnumerable<string> operations);
    }
}