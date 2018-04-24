// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Script.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the script class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.CompilerServices.Scripting
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// A script.
    /// </summary>
    public class Script : Expando, IScript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        /// <param name="language">The script language.</param>
        /// <param name="sourceCode">The source code.</param>
        public Script(string language, string sourceCode)
        {
            Requires.NotNullOrEmpty(language, nameof(language));
            Requires.NotNull(sourceCode, nameof(sourceCode));

            this.Language = language;
            this.SourceCode = sourceCode;
        }

        /// <summary>
        /// Gets the script language.
        /// </summary>
        /// <value>
        /// The script language.
        /// </value>
        public string Language { get; }

        /// <summary>
        /// Gets the source code as a stream.
        /// </summary>
        /// <value>
        /// The source code.
        /// </value>
        public string SourceCode { get; }
    }
}