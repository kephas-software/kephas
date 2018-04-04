// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstituteTypeExpressionVisitor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Services;

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
        /// The implementation type resolver.
        /// </summary>
        private readonly Func<Type, IContext, Type> implementationTypeResolver;

        /// <summary>
        /// The generic handlers.
        /// </summary>
        private readonly IEnumerable<ISubstituteTypeConstantHandler> constantHandlers;

        /// <summary>
        /// The context.
        /// </summary>
        private readonly IContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstituteTypeExpressionVisitor"/> class.
        /// </summary>
        /// <param name="implementationTypeResolver">The implementation type resolver (optional).</param>
        /// <param name="activator">The activator (optional).</param>
        /// <param name="constantHandlers">The constant handlers (optional).</param>
        /// <param name="context">The context (optional).</param>
        public SubstituteTypeExpressionVisitor(Func<Type, IContext, Type> implementationTypeResolver = null, IActivator activator = null, IEnumerable<ISubstituteTypeConstantHandler> constantHandlers = null, IContext context = null)
        {
            this.implementationTypeResolver = (t, ctx) =>
                {
                    var implementationType = implementationTypeResolver?.Invoke(t, ctx)
                                             ?? (activator?.GetImplementationType(t.AsRuntimeTypeInfo(), throwOnNotFound: false, activationContext: ctx) as IRuntimeTypeInfo)?.Type;
                    return implementationType;
                };
            this.constantHandlers = constantHandlers ?? GetDefaultSubstituteTypeConstantHandlers();
            this.context = context;
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
            if (node is LambdaExpression lambdaExpression)
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
                var mappedGenericArgs = mappedMethod.GetGenericArguments().Select(t => this.TryResolveDeepImplementationType(t) ?? t).ToList();
                mappedMethod = genericMethodDefinition.MakeGenericMethod(mappedGenericArgs.ToArray());

                if (mappedMethod.Name == nameof(Queryable.Cast) 
                    || mappedMethod.Name == nameof(Queryable.OfType))
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

        /// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression" />.</summary>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitNew(NewExpression node)
        {
            var newType = node.Type;
            if (!newType.IsConstructedGenericType)
            {
                return base.VisitNew(node);
            }

            var newImplementationType = this.TryResolveDeepImplementationType(newType);
            if (newImplementationType != null)
            {
                var constructorArgTypes = node.Constructor.GetParameters()
                    .Select(p => this.TryResolveDeepImplementationType(p.ParameterType) ?? p.ParameterType).ToArray();
                var newImplementationTypeInfo = newImplementationType.GetTypeInfo();

#if NETSTANDARD1_3
                var constructor = newImplementationTypeInfo.DeclaredConstructors.First(
                    c => c.GetParameters()
                          .Select((p, i) => p.ParameterType == constructorArgTypes[i])
                          .All(t => t));
                var arguments = node.Arguments.Select(this.Visit);
                var members = node.Members.Select(m => (MemberInfo)newImplementationTypeInfo.GetDeclaredProperty(m.Name));
#else
                var constructor = newImplementationTypeInfo.GetConstructor(constructorArgTypes);
                var arguments = node.Arguments.Select(this.Visit);
                var members = node.Members.Select(m => newImplementationType.GetMember(m.Name, m.MemberType, BindingFlags.Instance | BindingFlags.Public)[0]);
#endif

                return Expression.New(constructor, arguments, members);
            }

            return base.VisitNew(node);
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

            var concreteType = this.TryResolveDeepImplementationType(node.Expression?.Type);
            if (concreteType != null)
            {
                var memberName = node.Member.Name;
                IRuntimeElementInfo otherMember;
                switch (node.Member)
                {
                    case FieldInfo _:
                        otherMember = concreteType.AsRuntimeTypeInfo().Fields[memberName];
                        break;
                    case PropertyInfo _:
                        otherMember = concreteType.AsRuntimeTypeInfo().Properties[memberName];
                        break;
                    default:
                        return base.VisitMember(node);
                }

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
            var concreteType = this.TryResolveImplementationType(node.Type);
            if (concreteType != null)
            {
                if (concreteType != node.Type)
                {
                    return Expression.Constant(Convert.ChangeType(node.Value, concreteType), concreteType);
                }

                return base.VisitConstant(node);
            }

            if (nodeTypeInfo.IsGenericType)
            {
                var genericTypeDefinition = nodeTypeInfo.GetGenericTypeDefinition();
                var mappedGenericArgs = nodeTypeInfo.GenericTypeArguments.Select(t => this.TryResolveImplementationType(t) ?? t);
                concreteType = genericTypeDefinition.MakeGenericType(mappedGenericArgs.ToArray());
                if (node.Value == null)
                {
                    return Expression.Constant(node.Value, concreteType);
                }

                var handler = this.constantHandlers.FirstOrDefault(h => h.CanHandle(genericTypeDefinition));
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

            var concreteType = this.TryResolveDeepImplementationType(node.Type);
            if (concreteType != null)
            {
                mappedParam = Expression.Parameter(concreteType, node.Name);
                this.parametersMap.Add(node, mappedParam);
                return mappedParam;
            }

            return base.VisitParameter(node);
        }

        /// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression" />.</summary>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                var operandTypeInfo = node.Operand.Type.GetTypeInfo();
                var nodeTypeInfo = node.Type.GetTypeInfo();

                // simplify expression if the conversion is superfluous
                // and the type is not a value type.
                if (nodeTypeInfo.IsAssignableFrom(operandTypeInfo) && !nodeTypeInfo.IsValueType)
                {
                    return this.Visit(node.Operand);
                }

                var operand = this.Visit(node.Operand);
                return Expression.Convert(operand, node.Type);
            }

            if (node.NodeType == ExpressionType.Quote)
            {
                return Expression.Quote(this.Visit(node.Operand));
            }

            return base.VisitUnary(node);
        }


        /// <summary>
        /// Tries to get the implementation type of the provided abstract type.
        /// </summary>
        /// <remarks>
        /// The provided abstract type may be a collection, in which case
        /// the item type is replaced with an implementation type.
        /// </remarks>
        /// <param name="abstractType">The abstract type.</param>
        /// <returns>The implementation type.</returns>
        protected virtual Type TryResolveImplementationType(Type abstractType)
        {
            if (abstractType == null)
            {
                return null;
            }

            var collectionItemType = abstractType.TryGetCollectionItemType();
            if (collectionItemType == null)
            {
                return this.implementationTypeResolver(abstractType, this.context);
            }

            if (collectionItemType.GetTypeInfo().IsInterface)
            {
                var collectionItemImplementationType = this.implementationTypeResolver(abstractType, this.context);
                if (collectionItemImplementationType != null)
                {
                    var collectionGenericDefinitionType = abstractType.GetGenericTypeDefinition();
                    var collectionConcreteType = collectionGenericDefinitionType.MakeGenericType(collectionItemImplementationType);
                    return collectionConcreteType;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to get the generic implementation type where the generic type arguments
        /// are replaced with implementation types.
        /// </summary>
        /// <param name="constructedType">Type of the constructed.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        protected virtual Type TryResolveDeepImplementationType(Type constructedType)
        {
            if (constructedType == null)
            {
                return null;
            }

            if (!constructedType.IsConstructedGenericType)
            {
                return this.TryResolveImplementationType(constructedType);
            }

            var newGenericTypeDefiniton = constructedType.GetGenericTypeDefinition();
            var constructedTypeInfo = constructedType.GetTypeInfo();
            var newGenericArgs = constructedTypeInfo.GenericTypeArguments.Select(t => this.TryResolveDeepImplementationType(t) ?? t).ToArray();
            var newGenericType = newGenericTypeDefiniton.MakeGenericType(newGenericArgs);
            return newGenericType;
        }

        /// <summary>
        /// Gets the default substitute type constant handlers in this collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default substitute type constant
        /// handlers in this collection.
        /// </returns>
        private static IEnumerable<ISubstituteTypeConstantHandler> GetDefaultSubstituteTypeConstantHandlers()
        {
            yield return new ListSubstituteTypeConstantHandler();
            yield return new EnumerableQuerySubstituteTypeConstantHandler();
            yield return new DataContextQueryableSubstituteTypeConstantHandler();
        }
    }
}