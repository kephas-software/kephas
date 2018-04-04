// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndsWithExpressionConverterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ends with expression converter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;

    using NUnit.Framework;

    [TestFixture]
    public class EndsWithExpressionConverterTest
    {
        [Test]
        public void Convert()
        {
            var converter = new EndsWithExpressionConverter();

            var expr = (MethodCallExpression)converter.ConvertExpression(new List<Expression>
                                                                             {
                                                                                 Expression.Constant("123"),
                                                                                 Expression.Constant("12"),
                                                                             });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsFalse((bool)result);

            expr = (MethodCallExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.Constant("123"),
                                                                             Expression.Constant("23"),
                                                                         });
            result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }
    }
}