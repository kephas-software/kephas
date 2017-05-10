// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFormatter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the code formatter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Generation
{
    /// <summary>
    /// A code formatter.
    /// </summary>
    public class CodeFormatter : ICodeFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFormatter"/> class.
        /// </summary>
        public CodeFormatter()
        {
            this.IndentUnitSize = 2;
        }

        /// <summary>
        /// Gets or sets the size of the indent for one unit.
        /// </summary>
        /// <value>
        /// The size of the indent unit.
        /// </value>
        public int IndentUnitSize { get; set; }

        /// <summary>
        /// Gets or sets the indent units.
        /// </summary>
        /// <value>
        /// The indent units.
        /// </value>
        public int IndentUnits { get; set; }
    }
}