// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IExpressionConverter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for expression converters.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(OperatorAttribute) })]
    public interface IExpressionConverter
    {
        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        Expression ConvertExpression(IList<Expression> args, Type clientItemType, ParameterExpression lambdaArg);
    }
}