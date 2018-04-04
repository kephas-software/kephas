// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultClientQueryConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default client query converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.Client.Queries.Conversion.Composition;
    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;
    using Kephas.Data.Client.Resources;
    using Kephas.Data.Linq.Expressions;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Model.Services;
    using Kephas.Reflection;
    using Kephas.Services;

    using Expression = Kephas.Data.Client.Queries.Expression;
    using LinqExpression = System.Linq.Expressions.Expression;

    /// <summary>
    /// A default client query converter.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultClientQueryConverter : IClientQueryConverter
    {
        /// <summary>
        /// The data context query method.
        /// </summary>
        private static readonly MethodInfo DataContextQueryMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IDataContext)null).Query<string>(null));

        /// <summary>
        /// The type resolver.
        /// </summary>
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// The entity type resolver.
        /// </summary>
        private readonly IProjectedTypeResolver projectedTypeResolver;

        /// <summary>
        /// The expression converters.
        /// </summary>
        private readonly IDictionary<string, IExpressionConverter> expressionConverters = new Dictionary<string, IExpressionConverter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultClientQueryConverter"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="projectedTypeResolver">The entity type resolver.</param>
        /// <param name="converterFactories">The expression converters.</param>
        public DefaultClientQueryConverter(
            ITypeResolver typeResolver,
            IProjectedTypeResolver projectedTypeResolver,
            ICollection<IExportFactory<IExpressionConverter, ExpressionConverterMetadata>> converterFactories)
        {
            Requires.NotNull(typeResolver, nameof(typeResolver));
            Requires.NotNull(projectedTypeResolver, nameof(projectedTypeResolver));
            Requires.NotNull(converterFactories, nameof(converterFactories));

            this.typeResolver = typeResolver;
            this.projectedTypeResolver = projectedTypeResolver;

            foreach (
                var converterFactory in
                converterFactories.OrderBy(c => c.Metadata.Operator).ThenBy(c => c.Metadata.OverridePriority))
            {
                if (!this.expressionConverters.ContainsKey(converterFactory.Metadata.Operator))
                {
                    this.expressionConverters.Add(converterFactory.Metadata.Operator, converterFactory.CreateExportedValue());
                }
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultClientQueryConverter> Logger { get; set; }

        /// <summary>
        /// Converts the provided client query to a queryable which can be executed.
        /// </summary>
        /// <param name="clientQuery">The client query.</param>
        /// <param name="context">The query conversion context.</param>
        /// <returns>
        /// The converted query.
        /// </returns>
        public IQueryable ConvertQuery(ClientQuery clientQuery, IClientQueryConversionContext context)
        {
            var queryClientEntityType = this.GetQueryClientEntityType(clientQuery);
            var queryEntityType = this.GetQueryEntityType(clientQuery, queryClientEntityType);

            // create the query
            var queryMethod = DataContextQueryMethod.MakeGenericMethod(queryEntityType);
            var queryContext = new QueryOperationContext(context.DataContext);
            var queryable = (IQueryable)queryMethod.Call(context.DataContext, queryContext);

            // apply the filter expression
            var filterExpression = this.ConvertFilter(queryEntityType, queryClientEntityType, clientQuery.Filter);
            if (filterExpression != null)
            {
                var whereMethod = QueryableMethods.QueryableWhereGeneric.MakeGenericMethod(queryEntityType);
                queryable = (IQueryable)whereMethod.Call(null, queryable, filterExpression);
            }

            // apply the order by expressions
            var orderExpressions = this.ConvertOrder(queryEntityType, queryClientEntityType, clientQuery.Order)?.ToList();
            if (orderExpressions != null && orderExpressions.Count > 0)
            {
                Func<LinqExpression, Type> extractKeySelectorType = e => e.Type.GetTypeInfo().GenericTypeArguments[1];

                var firstOrder = orderExpressions[0];
                var orderByGenericMethod = firstOrder.op == AscExpressionConverter.Operator
                                        ? QueryableMethods.QueryableOrderByGeneric
                                        : QueryableMethods.QueryableOrderByDescendingGeneric;
                var orderByMethod = orderByGenericMethod.MakeGenericMethod(queryEntityType, extractKeySelectorType(firstOrder.expression));
                queryable = (IQueryable)orderByMethod.Call(null, queryable, firstOrder.expression);

                foreach (var nextOrder in orderExpressions.Skip(1))
                {
                    orderByGenericMethod = nextOrder.op == AscExpressionConverter.Operator
                                                   ? QueryableMethods.QueryableThenByGeneric
                                                   : QueryableMethods.QueryableThenByDescendingGeneric;
                    orderByMethod = orderByGenericMethod.MakeGenericMethod(queryEntityType, extractKeySelectorType(nextOrder.expression));
                    queryable = (IQueryable)orderByMethod.Call(null, queryable, nextOrder.expression);
                }
            }

            return queryable;
        }

        /// <summary>
        /// Converts the where clause.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="where">The where clause.</param>
        /// <returns>
        /// The converted where clause.
        /// </returns>
        protected virtual LinqExpression ConvertFilter(Type itemType, Type clientItemType, Expression where)
        {
            if (where == null)
            {
                return null;
            }

            var lambdaArg = LinqExpression.Parameter(itemType, "e");
            var body = this.ConvertExpression(where, clientItemType, lambdaArg);
            return body == null ? null : LinqExpression.Lambda(body, lambdaArg);
        }

        /// <summary>
        /// Converts the order clause.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="order">The order clause.</param>
        /// <returns>
        /// The converted order clause.
        /// </returns>
        protected virtual IEnumerable<(string op, LinqExpression expression)> ConvertOrder(Type itemType, Type clientItemType, Expression order)
        {
            if (order?.Args == null || order.Args.Count == 0)
            {
                yield break;
            }

            var lambdaArg = LinqExpression.Parameter(itemType, "e");

            (string op, LinqExpression expression)? GetOrder(object arg, string o, LinqExpression body)
            {
                if (body != null)
                {
                    return (o, LinqExpression.Lambda(body, lambdaArg));
                }

                this.Logger.Warn($"'{arg}' got converted to a <null> expression.");
                return null;
            }

            for (var i = 0; i < order.Args.Count; i++)
            {
                var orderArg = order.Args[i];
                var orderArgResult =
                orderArg is Expression orderArgExpression
                    // make expression
                    ? GetOrder(
                        orderArgExpression,
                        orderArgExpression.Op == AscExpressionConverter.Operator
                        || orderArgExpression.Op == DescExpressionConverter.Operator
                            ? orderArgExpression.Op
                            : AscExpressionConverter.Operator,
                        this.ConvertExpression(orderArgExpression, clientItemType, lambdaArg))

                    : this.IsMemberAccess(orderArg, i)
                        // make member access
                        ? GetOrder(
                            orderArg,
                            AscExpressionConverter.Operator,
                            this.MakeMemberAccessExpression(orderArg, clientItemType, lambdaArg))

                        // constants are not relevant for sorting, so just ignore them.
                        : null;

                if (orderArgResult != null)
                {
                    yield return orderArgResult.Value;
                }
            }
        }

        /// <summary>
        /// Gets the query client entity type.
        /// </summary>
        /// <param name="clientQuery">The client query.</param>
        /// <returns>
        /// The query client entity type.
        /// </returns>
        protected virtual Type GetQueryClientEntityType(ClientQuery clientQuery)
        {
            return this.typeResolver.ResolveType(clientQuery.EntityType);
        }

        /// <summary>
        /// Gets the query entity type based on the client entity type.
        /// </summary>
        /// <param name="clientQuery">The client query.</param>
        /// <param name="clientEntityType">The client entity type.</param>
        /// <returns>
        /// The query entity type.
        /// </returns>
        protected virtual Type GetQueryEntityType(ClientQuery clientQuery, Type clientEntityType)
        {
            return this.projectedTypeResolver.ResolveProjectedType(clientEntityType);
        }

        /// <summary>
        /// Converts the provided client expression to a LINQ expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <returns>
        /// The converted LINQ expression.
        /// </returns>
        protected virtual LinqExpression ConvertExpression(Expression expression, Type clientItemType, ParameterExpression lambdaArg)
        {
            var converter = this.expressionConverters.TryGetValue(expression.Op);
            if (converter == null)
            {
                throw new DataException(string.Format(Strings.DefaultClientQueryConverter_OperatorNotSupported_Exception, expression.Op));
            }

            var args = new List<LinqExpression>();
            var exprArgs = expression.Args;
            if (exprArgs != null && exprArgs.Count > 0)
            {
                for (var i = 0; i < exprArgs.Count; i++)
                {
                    var arg = exprArgs[i];
                    args.Add(
                        arg is Expression argExpression
                            ? this.ConvertExpression(argExpression, clientItemType, lambdaArg)
                            : this.IsMemberAccess(arg, i)
                                ? this.MakeMemberAccessExpression(arg, clientItemType, lambdaArg)
                                : this.MakeConstantExpression(arg));
                }
            }

            return converter.ConvertExpression(args);
        }

        /// <summary>
        /// Makes a constant expression out of the provided argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>
        /// A LINQ expression.
        /// </returns>
        protected virtual LinqExpression MakeConstantExpression(object arg)
        {
            return LinqExpression.Constant(arg);
        }

        /// <summary>
        /// Makes a member access expression.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <returns>
        /// A LINQ expression.
        /// </returns>
        protected virtual LinqExpression MakeMemberAccessExpression(object arg, Type clientItemType, ParameterExpression lambdaArg)
        {
            var memberName = (string)arg;
            memberName = memberName.Substring(1, memberName.Length - 1); // cut leading dot (.)
            memberName = memberName.ToPascalCase();

            var queryItemTypeInfo = lambdaArg.Type.AsRuntimeTypeInfo();
            if (!queryItemTypeInfo.Properties.TryGetValue(memberName, out var propertyInfo))
            {
                throw new MissingMemberException(string.Format(Strings.DefaultClientQueryConverter_MissingMember_Exception, memberName, queryItemTypeInfo));
            }

            return LinqExpression.MakeMemberAccess(lambdaArg, propertyInfo.PropertyInfo);
        }

        /// <summary>
        /// Query if 'arg' is member access.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="argIndex">Zero-based index of the argument.</param>
        /// <returns>
        /// True if member access, false if not.
        /// </returns>
        protected virtual bool IsMemberAccess(object arg, int argIndex)
        {
            var stringArg = arg as string;
            return !string.IsNullOrEmpty(stringArg) && stringArg[0] == '.';
        }
    }
}