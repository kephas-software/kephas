// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeFormatter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICodeFormatter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.CodeAnalysis.Generation
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Interface for code formatter.
    /// </summary>
    public interface ICodeFormatter
    {
        /// <summary>
        /// Gets or sets the size of the indent unit.
        /// </summary>
        /// <value>
        /// The size of the indent unit.
        /// </value>
        int IndentUnitSize { get; set; }

        /// <summary>
        /// Gets or sets the indent units.
        /// </summary>
        /// <value>
        /// The indent units.
        /// </value>
        int IndentUnits { get; set; }
    }

    /// <summary>
    /// A code formatter extensions.
    /// </summary>
    public static class CodeFormatterExtensions
    {
        /// <summary>
        /// An ICodeFormatter extension method that increase indent.
        /// </summary>
        /// <param name="formatter">The formatter to act on.</param>
        public static void IncreaseIndent(this ICodeFormatter formatter)
        {
            Requires.NotNull(formatter, nameof(formatter));

            formatter.IndentUnits++;
        }

        /// <summary>
        /// An ICodeFormatter extension method that decrease indent.
        /// </summary>
        /// <param name="formatter">The formatter to act on.</param>
        public static void DecreaseIndent(this ICodeFormatter formatter)
        {
            Requires.NotNull(formatter, nameof(formatter));

            if (formatter.IndentUnits <= 0)
            {
                return;
            }

            formatter.IndentUnits--;
        }

        /// <summary>
        /// An ICodeFormatter extension method that gets the current indent.
        /// </summary>
        /// <param name="formatter">The formatter to act on.</param>
        /// <returns>
        /// The indent.
        /// </returns>
        public static string CurrentIndent(this ICodeFormatter formatter)
        {
            Requires.NotNull(formatter, nameof(formatter));

            return formatter.IndentUnits <= 0 || formatter.IndentUnits <= 0
                       ? string.Empty
                       : string.Empty.PadRight(formatter.IndentUnits * formatter.IndentUnitSize);
        }
    }
}