// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeGenerationUnit.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the code generation unit class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.CodeAnalysis.Generation
{
    using System.Text;

    /// <summary>
    /// A code generation unit.
    /// </summary>
    public class CodeGenerationUnit : ICodeGenerationUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerationUnit"/> class.
        /// </summary>
        /// <param name="text">The text builder (optional).</param>
        public CodeGenerationUnit(StringBuilder? text = null)
        {
            this.Text = text ?? new StringBuilder();
        }

        /// <summary>
        /// Gets or sets the name of the output.
        /// </summary>
        /// <value>
        /// The name of the output.
        /// </value>
        public string? OutputName { get; set; }

        /// <summary>
        /// Gets or sets the full pathname of the output file.
        /// </summary>
        /// <value>
        /// The full pathname of the output file.
        /// </value>
        public string? OutputPath { get; set; }

        /// <summary>
        /// Gets the text builder.
        /// </summary>
        /// <value>
        /// The text builder.
        /// </value>
        public StringBuilder Text { get; }
    }
}