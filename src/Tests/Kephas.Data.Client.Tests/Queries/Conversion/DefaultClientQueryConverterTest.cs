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
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Client.Queries.Conversion.Composition;
    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;
    using Kephas.Model.Services;
    using Kephas.Reflection;
    using Kephas.Services;

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
        public void ConvertQuery_int_constants(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.EqConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Filter = new Expression { Op = "=", Args = new List<object> { 1, 2 } }
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(arg: new string[] { "I", "saw", "the", "film", "Heidi" })]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConvertQuery_member_access(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.GtConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Filter = new Expression { Op = ">", Args = new List<object> { ".Length", 3 } }
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();
            Assert.AreEqual(data.Count(s => s.Length > 3), result.Count);
        }

        [Test]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConvertQuery_member_access_missing(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.GtConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
                            {
                                EntityType = "item-type",
                                Filter = new Expression { Op = ">", Args = new List<object> { ".count", 3 } }
                            };

            Assert.Throws<MissingMemberException>(() => converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext)));
        }

        [Test]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators", "!" })]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators" })]
        [TestCase(arg: new string[] { "hi", "all" })]
        [TestCase(arg: new string[] { "hi" })]
        public void ConvertQuery_nary_operator_without_arguments(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.AndConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
                            {
                                EntityType = "item-type",
                                Filter = new Expression { Op = "&&", Args = null }
                            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();
            Assert.AreEqual(data.Length, result.Count);
        }

        [Test]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators", "!" })]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators" })]
        [TestCase(arg: new string[] { "hi", "all" })]
        [TestCase(arg: new string[] { "hi" })]
        public void ConvertQuery_orderby_asc(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.AscConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
                            {
                                EntityType = "item-type",
                                Order = new Expression { Op = "$orderby", Args = new List<object> { new Expression { Op = "$asc", Args = new List<object> { ".Length" } } } }
                            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();

            var orderedData = data.OrderBy(s => s.Length).ToArray();
            Assert.AreEqual(orderedData.Length, result.Count);
            for (var i = 0; i < orderedData.Length; i++)
            {
                Assert.AreEqual(orderedData[i], result[i]);
            }
        }


        [Test]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators", "!" })]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators" })]
        [TestCase(arg: new string[] { "hi", "all" })]
        [TestCase(arg: new string[] { "hi" })]
        public void ConvertQuery_orderby_desc(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.DescConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
                            {
                                EntityType = "item-type",
                                Order = new Expression { Op = "$orderby", Args = new List<object> { new Expression { Op = "$desc", Args = new List<object> { ".Length" } } } }
                            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();

            var orderedData = data.OrderByDescending(s => s.Length).ToArray();
            Assert.AreEqual(orderedData.Length, result.Count);
            for (var i = 0; i < orderedData.Length; i++)
            {
                Assert.AreEqual(orderedData[i], result[i]);
            }
        }

        [Test]
        [TestCase(arg: new string[] { "doesn't matter" })]
        public void ConvertQuery_operator_not_supported(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(typeResolver, this.GetIdempotentProjectedTypeResolver(), new[] { this.GtConverter() });

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Filter = new Expression { Op = "=", Args = new List<object> { ".Length", 3 } }
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

        private IProjectedTypeResolver GetIdempotentProjectedTypeResolver()
        {
            var resolver = Substitute.For<IProjectedTypeResolver>();
            resolver.ResolveProjectedType(Arg.Any<Type>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(ci => ci.Arg<Type>());
            return resolver;
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> DescConverter()
        {
            return new ExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => new DescExpressionConverter(), new ExpressionConverterMetadata(DescExpressionConverter.Operator));
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> AscConverter()
        {
            return new ExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => new AscExpressionConverter(), new ExpressionConverterMetadata(AscExpressionConverter.Operator));
        }

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> AndConverter()
        {
            return new ExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => new AndExpressionConverter(), new ExpressionConverterMetadata("&&"));
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
            return new ExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => converter, new ExpressionConverterMetadata(op));
        }
    }
}