// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the description expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Resources;

    /// <summary>
    /// A descending expression converter.
    /// </summary>
    [Operator(Operator)]
    public class DescExpressionConverter : IExpressionConverter
    {
        /// <summary>
        /// The operator for descending sort.
        /// </summary>
        public const string Operator = "$desc";

        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public Expression ConvertExpression(IList<Expression> args, Type clientItemType, ParameterExpression lambdaArg)
        {
            if (args.Count != 1)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 1));
            }

            return args[0];
        }
    }
}