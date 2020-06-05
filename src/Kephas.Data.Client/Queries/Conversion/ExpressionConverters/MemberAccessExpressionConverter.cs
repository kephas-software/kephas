// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberAccessExpressionConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the member expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// A member expression converter.
    /// </summary>
    [Operator(Operator)]
    public class MemberAccessExpressionConverter : IExpressionConverter
    {
        /// <summary>
        /// The operator for descending sort.
        /// </summary>
        public const string Operator = "$m";

        private const char MemberOperator = '.';
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAccessExpressionConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public MemberAccessExpressionConverter(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public Expression ConvertExpression(IList<Expression> args, Type clientItemType, ParameterExpression lambdaArg)
        {
            if (args.Count != 1)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 1));
            }

            var arg = args[0];
            if (arg is ConstantExpression constExprArg)
            {
                return MakeMemberAccessExpression(constExprArg.Value, clientItemType, lambdaArg, this.typeRegistry);
            }

            // TODO localization
            throw new DataException($"Only constant expressions supported, instead {arg} provided.");
        }

        /// <summary>
        /// Makes a member access expression.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="clientItemType">The client item type.</param>
        /// <param name="lambdaArg">The lambda argument.</param>
        /// <param name="typeRegistry">The type registry.</param>
        /// <returns>
        /// A LINQ expression.
        /// </returns>
        internal static Expression MakeMemberAccessExpression(object arg, Type clientItemType, ParameterExpression lambdaArg, IRuntimeTypeRegistry typeRegistry)
        {
            var memberName = (string)arg;
            Requires.NotNullOrEmpty(memberName, nameof(memberName));

            if (memberName[0] == MemberOperator)
            {
                memberName = memberName.Substring(1, memberName.Length - 1); // cut leading dot (.)
            }

            memberName = memberName.ToPascalCase();

            var queryItemTypeInfo = lambdaArg.Type.AsRuntimeTypeInfo(typeRegistry);
            if (!queryItemTypeInfo.Properties.TryGetValue(memberName, out var propertyInfo))
            {
                throw new MissingMemberException(string.Format(Strings.DefaultClientQueryConverter_MissingMember_Exception, memberName, queryItemTypeInfo));
            }

            return Expression.MakeMemberAccess(lambdaArg, propertyInfo.PropertyInfo);
        }

        /// <summary>
        /// Query if 'arg' is member access.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>
        /// True if member access, false if not.
        /// </returns>
        internal static bool IsMemberAccess(object arg)
        {
            var stringArg = arg as string;
            return !string.IsNullOrEmpty(stringArg) && stringArg?[0] == MemberOperator;
        }
    }
}