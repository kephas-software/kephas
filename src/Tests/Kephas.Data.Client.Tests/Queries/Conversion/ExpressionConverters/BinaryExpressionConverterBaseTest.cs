// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExpressionConverterBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the binary expression converter base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;

    using NUnit.Framework;

    [TestFixture]
    public class BinaryExpressionConverterBaseTest
    {
        [Test]
        public void Convert_const_eq_constant()
        {
            var converter = new TestBinaryExpressionConverter(Expression.Equal);

            var expr = (BinaryExpression)converter.ConvertExpression(new List<Expression>
                                                                             {
                                                                                 Expression.Constant("123"),
                                                                                 Expression.Constant("12"),
                                                                             });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsFalse((bool)result);
        }

        [Test]
        public void Convert_member_eq_constant()
        {
            var converter = new TestBinaryExpressionConverter(Expression.Equal);

            var expr = (BinaryExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.MakeMemberAccess(Expression.Constant("123"), typeof(string).GetProperty("Length")),
                                                                             Expression.Constant(3),
                                                                         });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_member_eq_constant_nullable()
        {
            var converter = new TestBinaryExpressionConverter(Expression.Equal);

            var expr = (BinaryExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.MakeMemberAccess(Expression.Constant("123"), typeof(string).GetProperty("Length")),
                                                                             Expression.Constant((int?)3),
                                                                         });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_member_eq_constant_auto_convert()
        {
            var converter = new TestBinaryExpressionConverter(Expression.Equal);

            var expr = (BinaryExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.MakeMemberAccess(Expression.Constant("123"), typeof(string).GetProperty("Length")),
                                                                             Expression.Constant("3"),
                                                                         });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_member_eq_constant_nullable_member_auto_convert()
        {
            var converter = new TestBinaryExpressionConverter(Expression.Equal);

            var expr = (BinaryExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.MakeMemberAccess(Expression.Constant(new NullableAge { Age = 3 }), typeof(NullableAge).GetProperty("Age")),
                                                                             Expression.Constant("3"),
                                                                         });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_member_eq_constant_nullable_member_explicit_convert()
        {
            var converter = new TestBinaryExpressionConverter(Expression.Equal);

            var expr = (BinaryExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.MakeMemberAccess(Expression.Constant(new NullableAge { Age = 3 }), typeof(NullableAge).GetProperty("Age")),
                                                                             Expression.Constant(new Age(3)),
                                                                         });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }

        public class NullableAge
        {
            public int? Age { get; set; }
        }

        public class TestBinaryExpressionConverter : BinaryExpressionConverterBase
        {
            public TestBinaryExpressionConverter(Func<Expression, Expression, BinaryExpression> binaryExpressionFactory)
                : base(binaryExpressionFactory)
            {
            }
        }

        public class Age
        {
            private readonly int age;

            public Age(int age)
            {
                this.age = age;
            }

            public static explicit operator int(Age age)
            {
                return age.age;
            }
        }
    }
}