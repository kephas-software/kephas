// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstituteTypeExpressionVisitor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the replace type expression visitor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Collections;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// The <see cref="SubstituteTypeExpressionVisitor"/>
    /// adjust types in the query substituting types with their substitute values
    /// and removes redundant Cast and OfType methods.
    /// </summary>
    public class SubstituteTypeExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The parameters map.
        /// </summary>
        private readonly IDictionary<ParameterExpression, ParameterExpression> parametersMap = new Dictionary<ParameterExpression, ParameterExpression>();

        /// <summary>
        /// The activator used for.
        /// </summary>
        private readonly IActivator activator;

        /// <summary>
        /// The generic handlers.
        /// </summary>
        private readonly IEnumerable<ISubstituteTypeConstantHandler> genericHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstituteTypeExpressionVisitor"/> class.
        /// </summary>
        /// <param name="activator">The activator.</param>
        public SubstituteTypeExpressionVisitor(IActivator activator)
            : this(
                  activator, 
                  new ISubstituteTypeConstantHandler[]
                                  {
                                      new ListSubstituteTypeConstantHandler(), 
                                      new EnumerableQuerySubstituteTypeConstantHandler(), 
                                      new DataContextQueryableSubstituteTypeConstantHandler(), 
                                  })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstituteTypeExpressionVisitor"/> class.
        /// </summary>
        /// <param name="activator">The activator.</param>
        /// <param name="genericHandlers">The generic handlers.</param>
        public SubstituteTypeExpressionVisitor(IActivator activator, IEnumerable<ISubstituteTypeConstantHandler> genericHandlers)
        {
            this.activator = activator;
            this.genericHandlers = genericHandlers;
        }

        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        public override Expression Visit(Expression node)
        {
            var lambdaExpression = node as LambdaExpression;
            if (lambdaExpression != null)
            {
                var parameters = lambdaExpression.Parameters.Select(p => (ParameterExpression)this.VisitParameter(p));
                var body = this.Visit(lambdaExpression.Body);
                var mappedLambda = Expression.Lambda(body, parameters);
                return mappedLambda;
            }

            return base.Visit(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var mappedArgs = node.Arguments.Select(this.Visit).ToList();
            var mappedObject = this.Visit(node.Object);
            var mappedMethod = node.Method;
            if (mappedMethod.IsGenericMethod)
            {
                var genericMethodDefinition = mappedMethod.GetGenericMethodDefinition();
                var mappedGenericArgs = mappedMethod.GetGenericArguments().Select(t => this.TryGetConcreteType(t) ?? t).ToList();
                mappedMethod = genericMethodDefinition.MakeGenericMethod(mappedGenericArgs.ToArray());

                if (mappedMethod.Name == nameof(Queryable.Cast) || mappedMethod.Name == nameof(Queryable.OfType))
                {
                    var mappedItemType = mappedArgs[0].Type.TryGetEnumerableItemType();
                    var mappedConvertedItemType = mappedGenericArgs[0];
                    if (mappedItemType == mappedConvertedItemType)
                    {
                        return mappedArgs[0];
                    }
                }
            }

            return Expression.Call(mappedObject, mappedMethod, mappedArgs);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ConstantExpression)
            {
                return node;
            }

            var concreteType = this.TryGetConcreteType(node.Expression.Type);
            if (concreteType != null)
            {
                var memberName = node.Member.Name;
                var otherMember = concreteType.AsRuntimeTypeInfo().Properties[memberName];
                var expr = this.Visit(node.Expression);
                if (expr.Type.GetTypeInfo().IsInterface)
                {
                    return node;
                }

                var memberExpression = Expression.MakeMemberAccess(expr, otherMember.GetUnderlyingMemberInfo());
                return memberExpression;
            }

            return base.VisitMember(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            var nodeTypeInfo = node.Type.GetTypeInfo();
            var concreteType = this.TryGetConcreteType(node.Type);
            if (concreteType != null)
            {
                return Expression.Constant(Convert.ChangeType(node.Value, concreteType), concreteType);
            }

            if (nodeTypeInfo.IsGenericType)
            {
                var genericTypeDefinition = nodeTypeInfo.GetGenericTypeDefinition();
                var mappedGenericArgs = nodeTypeInfo.GenericTypeArguments.Select(t => this.TryGetConcreteType(t) ?? t);
                concreteType = genericTypeDefinition.MakeGenericType(mappedGenericArgs.ToArray());
                if (node.Value == null)
                {
                    return Expression.Constant(node.Value, concreteType);
                }

                var handler = this.genericHandlers.FirstOrDefault(h => h.CanHandle(genericTypeDefinition));
                if (handler != null)
                {
                    return Expression.Constant(handler.Visit(node.Value, concreteType), concreteType);
                }
            }

            return base.VisitConstant(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            var mappedParam = this.parametersMap.TryGetValue(node);
            if (mappedParam != null)
            {
                return mappedParam;
            }

            var concreteType = this.TryGetConcreteType(node.Type);
            if (concreteType != null)
            {
                mappedParam = Expression.Parameter(concreteType, node.Name);
                this.parametersMap.Add(node, mappedParam);
                return mappedParam;
            }

            return base.VisitParameter(node);
        }

        /// <summary>
        /// Tries to get the concrete type of the interface type.
        /// </summary>
        /// <param name="abstractType">Type of the interface.</param>
        /// <returns>The concerete type for an interface.</returns>
        private Type TryGetConcreteType(Type abstractType)
        {
            var collectionItemType = abstractType.TryGetCollectionItemType();
            if (collectionItemType == null)
            {
                return this.TryGetImplementationType(abstractType);
            }

            if (collectionItemType.GetTypeInfo().IsInterface)
            {
                var concreteCollectionItemType = this.TryGetImplementationType(collectionItemType);
                if (concreteCollectionItemType != null)
                {
                    var collectionGenericDefinitionType = abstractType.GetGenericTypeDefinition();
                    var collectionConcreteType = collectionGenericDefinitionType.MakeGenericType(concreteCollectionItemType);
                    return collectionConcreteType;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the implementation type.
        /// </summary>
        /// <param name="abstractType">The abstract type which needs to be replaced.</param>
        /// <returns>
        /// The implementation type.
        /// </returns>
        private Type TryGetImplementationType(Type abstractType)
        {
            var concreteType = this.activator.GetImplementationType(
                abstractType.AsRuntimeTypeInfo(),
                throwOnNotFound: false);
            return (concreteType as IRuntimeTypeInfo)?.Type;
        }
    }
}