﻿namespace CalculatorConsole.Calculator
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for parser.
    /// </summary>
    [SingletonAppServiceContract]
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
        (double op1, string opName, double op2) Parse(string input, IEnumerable<string> operations);
    }
}