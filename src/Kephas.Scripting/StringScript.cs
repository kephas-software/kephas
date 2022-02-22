// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringScript.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the script class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;

    using Kephas.Dynamic;

    /// <summary>
    /// A script.
    /// </summary>
    public class StringScript : Expando, IScript
    {
        private readonly string sourceCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringScript"/> class.
        /// </summary>
        /// <param name="language">The script language.</param>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="name">Optional. The script name.</param>
        public StringScript(string language, string sourceCode, string? name = null)
        {
            this.Language = language ?? throw new ArgumentNullException(nameof(language));
            this.Name = name ?? $"{nameof(StringScript)}_{Guid.NewGuid():N}";
            this.sourceCode = sourceCode ?? throw new ArgumentNullException(nameof(sourceCode));
        }

        /// <summary>
        /// Gets the script name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the script language.
        /// </summary>
        /// <value>
        /// The script language.
        /// </value>
        public string Language { get; }

        /// <summary>
        /// Gets the script source code.
        /// </summary>
        /// <returns>The source code.</returns>
        public string GetSourceCode() => this.sourceCode;
    }
}