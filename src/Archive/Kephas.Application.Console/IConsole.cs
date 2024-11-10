// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConsole.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConsole interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using Kephas.Services;

    /// <summary>
    /// Interface for console.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IConsole
    {
        /// <summary>
        /// Reads the line from the console.
        /// </summary>
        /// <returns>
        /// The line.
        /// </returns>
        string ReadLine();

        /// <summary>
        /// Writes the text to the console.
        /// </summary>
        /// <param name="value">The text to write.</param>
        void Write(string value);

        /// <summary>
        /// Writes the text to the console followed by a new line.
        /// </summary>
        /// <param name="value">Optional. The text to write.</param>
        void WriteLine(string value = null);
    }
}
