// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the eq expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Resources;

    /// <summary>
    /// Expression converter for the equals operator.
    /// </summary>
    [Operator("$eq")]
    public class EqExpressionConverter : IExpressionConverter
    {
        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public Expression ConvertExpression(IList<Expression> args)
        {
            if (args.Count != 2)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 2));
            }

            return Expression.Equal(args[0], args[1]);
        }
    }
}