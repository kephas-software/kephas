// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartsWithExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the starts with expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    /// <summary>
    /// Expression converter for the string.StartsWith operator.
    /// </summary>
    [Operator("$startswith")]
    public class StartsWithExpressionConverter : MethodCallExpressionConverterBase<string, string, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartsWithExpressionConverter"/> class.
        /// </summary>
        public StartsWithExpressionConverter()
            : base((s1, s2) => s1.StartsWith(s2))
        {
        }
    }
}