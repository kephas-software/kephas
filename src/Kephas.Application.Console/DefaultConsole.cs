// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConsole.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default console class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    /// <summary>
    /// A default console.
    /// </summary>
    public class DefaultConsole : IConsole
    {
        /// <summary>
        /// Reads the line from the console.
        /// </summary>
        /// <returns>
        /// The line.
        /// </returns>
        public string ReadLine() => System.Console.ReadLine();

        /// <summary>
        /// Writes the text to the console.
        /// </summary>
        /// <param name="value">The text to write.</param>
        public void Write(string value) => System.Console.Write(value);

        /// <summary>
        /// Writes the text to the console followed by a new line.
        /// </summary>
        /// <param name="value">The text to write.</param>
        public void WriteLine(string value) => System.Console.WriteLine(value);
    }
}
