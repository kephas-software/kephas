namespace CalculatorConsole.Calculator
{
    using System;
    using System.Collections.Generic;

    public class Parser : IParser
    {
        /// <summary>
        /// Parses the input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>
        /// A Tuple&lt;int,string,int&gt;
        /// </returns>
        public Tuple<double, string, double> Parse(string input, IEnumerable<string> operations)
        {
            foreach (var op in operations)
            {
                var opPos = input.IndexOf(op);
                if (opPos >= 0)
                {
                    var term1 = input.Substring(0, opPos);
                    var term2 = input.Substring(opPos + op.Length);
                    return Tuple.Create(double.Parse(term1), op, double.Parse(term2));
                }
            }

            throw new Exception($"Cannot parse the input string '{input}', no supported operation found!");
        }
    }
}