// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Script.cs" company="Kephas Software SRL">
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
    public class Script : Expando, IScript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        /// <param name="language">The script language.</param>
        /// <param name="sourceCode">The source code.</param>
        public Script(string language, object sourceCode)
        {
            this.Language = language ?? throw new ArgumentNullException(nameof(language));
            this.SourceCode = sourceCode ?? throw new ArgumentNullException(nameof(sourceCode));
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
        public object SourceCode { get; }
    }
}