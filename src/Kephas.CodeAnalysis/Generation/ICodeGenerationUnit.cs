// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeGenerationUnit.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICodeGenerationUnit interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Generation
{
    using System.Text;

    /// <summary>
    /// Interface for code generation unit.
    /// </summary>
    public interface ICodeGenerationUnit
    {
        /// <summary>
        /// Gets or sets the name of the output.
        /// </summary>
        /// <value>
        /// The name of the output.
        /// </value>
        string OutputName { get; set; }

        /// <summary>
        /// Gets or sets the full pathname of the output file.
        /// </summary>
        /// <value>
        /// The full pathname of the output file.
        /// </value>
        string OutputPath { get; set; }

        /// <summary>
        /// Gets the text builder.
        /// </summary>
        /// <value>
        /// The text builder.
        /// </value>
        StringBuilder Text { get; }
    }
}