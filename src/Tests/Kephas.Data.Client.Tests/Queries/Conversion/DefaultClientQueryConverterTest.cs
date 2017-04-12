// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultClientQueryConverterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default client query converter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests.Queries.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Client.Queries.Conversion.Composition;
    using Kephas.Reflection;
    using Kephas.Testing.Core.Composition;

    using NSubstitute;

    using NUnit.Framework;

    using Expression = Kephas.Data.Client.Queries.Expression;
    using LinqExpression = System.Linq.Expressions.Expression;

    [TestFixture]
    public class DefaultClientQueryConverterTest
    {
        [Test]
        [TestCase(arg: new string[] { "I", "saw", "the", "film", "Heidi" })]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConverQuery_int_constants(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, new[] { this.EqConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                ItemType = "item-type",
                Where = new Expression { Op = "=", Args = new List<object> { 1, 2 } }
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();
            Assert.AreEqual(0, result.Count);
        }


        [Test]
        [TestCase(arg: new string[] { "I", "saw", "the", "film", "Heidi" })]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConverQuery_member_access(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, new[] { this.GtConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                ItemType = "item-type",
                Where = new Expression { Op = ">", Args = new List<object> { ".Length", 3 } }
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();
            Assert.AreEqual(data.Count(s => s.Length > 3), result.Count);
        }

        [Test]
        [TestCase(arg: new string[] { "doesn't matter" })]
        public void ConverQuery_operator_not_supported(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, new[] { this.GtConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                ItemType = "item-type",
                Where = new Expression { Op = "=", Args = new List<object> { ".Length", 3 } }
            };

            // supported operator >, not =.
            Assert.Throws<DataException>(() => converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext)));
        }

        private ITypeResolver GetTypeResolverMock<TValue>(TValue[] data) where TValue : class
        {
            var typeResolver = Substitute.For<ITypeResolver>();
            typeResolver.ResolveType(Arg.Any<string>(), Arg.Any<bool>())
                .Returns(typeof(TValue));

            return typeResolver;
        }

        private IDataContext GetDataContextMock<TValue>(TValue[] data) where TValue : class
        {
            var dataContext = Substitute.For<IDataContext>();
            dataContext.Query<TValue>(Arg.Any<IQueryOperationContext>())
                .Returns(data.AsQueryable());
            return dataContext;
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> EqConverter()
        {
            return this.GetExpressionConverter("=", args => LinqExpression.Equal(args[0], args[1]));
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> GtConverter()
        {
            return this.GetExpressionConverter(">", args => LinqExpression.GreaterThan(args[0], args[1]));
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> GteConverter()
        {
            return this.GetExpressionConverter(">=", args => LinqExpression.GreaterThanOrEqual(args[0], args[1]));
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> GetExpressionConverter(
            string op,
            Func<IList<LinqExpression>, LinqExpression> conversionFunc)
        {
            var converter = Substitute.For<IExpressionConverter>();
            converter.ConvertExpression(Arg.Any<IList<LinqExpression>>())
                .Returns((ci) => conversionFunc(ci.Arg<IList<LinqExpression>>()));
            return new TestExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => converter, new ExpressionConverterMetadata(op));
        }
    }
}