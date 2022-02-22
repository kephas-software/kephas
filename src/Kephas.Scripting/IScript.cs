// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScript interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for script.
    /// </summary>
    public interface IScript : IExpando
    {
        /// <summary>
        /// Gets the script name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the script language.
        /// </summary>
        /// <value>
        /// The script language.
        /// </value>
        string Language { get; }

        /// <summary>
        /// Gets the script source code.
        /// </summary>
        /// <returns>The source code.</returns>
        string GetSourceCode();

        /// <summary>
        /// Gets the script source code asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The source code.</returns>
        Task<string> GetSourceCodeAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(this.GetSourceCode());
    }
}