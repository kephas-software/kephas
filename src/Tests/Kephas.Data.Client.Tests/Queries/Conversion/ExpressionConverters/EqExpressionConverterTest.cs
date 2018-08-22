// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqExpressionConverterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the eq expression converter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;

    using NUnit.Framework;

    [TestFixture]
    public class EqExpressionConverterTest
    {
        [Test]
        public void Convert_int_eq_int()
        {
            var converter = new EqExpressionConverter();

            var expr = converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.MakeMemberAccess(Expression.Constant(new Person { Age = 3 }), typeof(Person).GetProperty("Age")),
                                                                             Expression.Constant(3),
                                                                         });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_int_eq_nullable_int()
        {
            var converter = new EqExpressionConverter();

            var age = (int?)3;
            var expr = converter.ConvertExpression(new List<Expression>
                                                       {
                                                           Expression.MakeMemberAccess(Expression.Constant(new Person { Age = 3 }), typeof(Person).GetProperty("Age")),
                                                           Expression.Constant(age),
                                                       });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_string_eq_string_success()
        {
            var converter = new EqExpressionConverter();

            var expr = converter.ConvertExpression(new List<Expression>
                                                       {
                                                           Expression.MakeMemberAccess(Expression.Constant(new Person { Name = "gigi" }), typeof(Person).GetProperty("Name")),
                                                           Expression.Constant("gigi"),
                                                       });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_string_eq_string_fail()
        {
            var converter = new EqExpressionConverter();

            var expr = converter.ConvertExpression(new List<Expression>
                                                       {
                                                           Expression.MakeMemberAccess(Expression.Constant(new Person { Name = "gigi" }), typeof(Person).GetProperty("Name")),
                                                           Expression.Constant("non-gigi"),
                                                       });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsFalse((bool)result);
        }

        [Test]
        public void Convert_int_eq_string_success()
        {
            var converter = new EqExpressionConverter();

            var expr = converter.ConvertExpression(new List<Expression>
                                                       {
                                                           Expression.MakeMemberAccess(Expression.Constant(new Person { Age = 3 }), typeof(Person).GetProperty("Age")),
                                                           Expression.Constant("3"),
                                                       });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? Age { get; set; }
        }
    }
}