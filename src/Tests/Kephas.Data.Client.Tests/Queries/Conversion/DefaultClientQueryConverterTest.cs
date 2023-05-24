// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultClientQueryConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default client query converter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Data.Client.Tests.Queries.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;
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
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.EqConverter() },
                new RuntimeTypeRegistry());

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
        public void ConvertQuery_operators_with_override_priority(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var eqConverter = this.EqConverter();
            var altEqConverter = this.GetExpressionConverter(eqConverter.Metadata.Operator, args => LinqExpression.NotEqual(args[0], args[1]), overridePriority: (Priority)(-100));
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { eqConverter, altEqConverter },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
                            {
                                EntityType = "item-type",
                                Filter = new Expression { Op = "=", Args = new List<object> { 1, 2 } }
                            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext));
            var result = queryable.ToList();
            Assert.AreEqual(data.Length, result.Count);
        }

        [Test]
        [TestCase(arg: new string[] { "I", "saw", "the", "film", "Heidi" })]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConvertQuery_member_access(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.GtConverter() },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Filter = new Expression { Op = ">", Args = new List<object> { ".Length", 3 } }
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext) { UseMemberAccessConvention = true });
            var result = queryable.ToList();
            Assert.AreEqual(data.Count(s => s.Length > 3), result.Count);
        }

        [Test]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConvertQuery_member_access_missing(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.GtConverter() },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Filter = new Expression { Op = ">", Args = new List<object> { ".count", 3 } }
            };

            Assert.Throws<MissingMemberException>(() => converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext) { UseMemberAccessConvention = true }));
        }

        [Test]
        [TestCase(arg: new string[] { "Pippi", "Langstrumpf" })]
        public void ConvertQuery_member_access_missing_no_member_access_convention(string[] data)
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.GtConverter(), this.MemberAccessConverter(typeRegistry) },
                typeRegistry);

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
                            {
                                EntityType = "item-type",
                                Filter = new Expression { Op = ">", Args = new List<object> { new Expression { Op = "$m", Args = new List<object> { ".count" } }, 3 } },
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
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.AndConverter() },
                new RuntimeTypeRegistry());

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
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.AscConverter() },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Order = new Expression { Op = "$orderby", Args = new List<object> { new Expression { Op = "$asc", Args = new List<object> { ".Length" } } } }
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext) { UseMemberAccessConvention = true });
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
        public void ConvertQuery_orderby_desc_no_member_access_convention(string[] data)
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.DescConverter(), this.MemberAccessConverter(typeRegistry) },
                typeRegistry);

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Order = new Expression { Op = "$orderby", Args = new List<object> { new Expression { Op = "$desc", Args = new List<object> { new Expression { Op = "$m", Args = new List<object> { ".Length" } } } } } },
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
        [TestCase(arg: new string[] { "hi", "all", "nary-operators", "!" })]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators" })]
        [TestCase(arg: new string[] { "hi", "all" })]
        [TestCase(arg: new string[] { "hi" })]
        public void ConvertQuery_orderby_desc(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.DescConverter() },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Order = new Expression { Op = "$orderby", Args = new List<object> { new Expression { Op = "$desc", Args = new List<object> { ".Length" } } } },
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext) { UseMemberAccessConvention = true });
            var result = queryable.ToList();

            var orderedData = data.OrderByDescending(s => s.Length).ToArray();
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
        public void ConvertQuery_skip(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.DescConverter() },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Skip = 2,
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext) { UseMemberAccessConvention = true });
            var result = queryable.ToList();

            var skippedData = data.Skip(2).ToArray();
            Assert.AreEqual(skippedData.Length, result.Count);
            for (var i = 0; i < skippedData.Length; i++)
            {
                Assert.AreEqual(skippedData[i], result[i]);
            }
        }

        [Test]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators", "!" })]
        [TestCase(arg: new string[] { "hi", "all", "nary-operators" })]
        [TestCase(arg: new string[] { "hi", "all" })]
        [TestCase(arg: new string[] { "hi" })]
        public void ConvertQuery_take(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.DescConverter() },
                new RuntimeTypeRegistry());

            var dataContext = this.GetDataContextMock(data);
            var query = new ClientQuery
            {
                EntityType = "item-type",
                Take = 2,
            };

            var queryable = (IQueryable<string>)converter.ConvertQuery(query, new ClientQueryConversionContext(dataContext) { UseMemberAccessConvention = true });
            var result = queryable.ToList();

            var takenData = data.Take(2).ToArray();
            Assert.AreEqual(takenData.Length, result.Count);
            for (var i = 0; i < takenData.Length; i++)
            {
                Assert.AreEqual(takenData[i], result[i]);
            }
        }

        [Test]
        [TestCase(arg: new string[] { "doesn't matter" })]
        public void ConvertQuery_operator_not_supported(string[] data)
        {
            var typeResolver = this.GetTypeResolverMock(data);
            var converter = new DefaultClientQueryConverter(
                typeResolver,
                this.GetIdempotentProjectedTypeResolver(),
                new[] { this.GtConverter() },
                new RuntimeTypeRegistry());

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
            dataContext.Query<TValue>(Arg.Any<Action<IQueryOperationContext>>())
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

        private IExportFactory<IExpressionConverter, ExpressionConverterMetadata> MemberAccessConverter(IRuntimeTypeRegistry typeRegistry)
        {
            return new ExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => new MemberAccessExpressionConverter(typeRegistry), new ExpressionConverterMetadata(MemberAccessExpressionConverter.Operator));
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
            Func<IList<LinqExpression>, LinqExpression> conversionFunc,
            Priority overridePriority = 0)
        {
            var converter = Substitute.For<IExpressionConverter>();
            converter.ConvertExpression(Arg.Any<IList<LinqExpression>>(), Arg.Any<Type>(), Arg.Any<ParameterExpression>())
                .Returns((ci) => conversionFunc(ci.Arg<IList<LinqExpression>>()));
            return new ExportFactory<IExpressionConverter, ExpressionConverterMetadata>(() => converter, new ExpressionConverterMetadata(op, overridePriority: overridePriority));
        }
    }
}