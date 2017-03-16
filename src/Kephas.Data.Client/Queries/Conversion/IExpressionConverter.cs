// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IExpressionConverter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Services;

    /// <summary>
    /// Service contract for expression converters.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(OperatorAttribute) })]
    public interface IExpressionConverter
    {
        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        Expression ConvertExpression(IList<Expression> args);
    }
}