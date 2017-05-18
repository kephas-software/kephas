// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndsWithExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the ends with expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    /// <summary>
    /// Expression converter for the string.EndsWith operator.
    /// </summary>
    [Operator("$endswith")]
    public class EndsWithExpressionConverter : MethodCallExpressionConverterBase<string, string, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndsWithExpressionConverter"/> class.
        /// </summary>
        public EndsWithExpressionConverter()
            : base((s1, s2) => s1.EndsWith(s2))
        {
        }
    }
}