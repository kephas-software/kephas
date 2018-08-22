// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the eq expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression converter for the equals operator.
    /// </summary>
    /// <remarks>
    /// The equality expression converter uses the object.Equals to support the reference types (like string).
    /// </remarks>
    [Operator("$eq")]
    public class EqExpressionConverter : MethodCallExpressionConverterBase<object, object, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqExpressionConverter"/> class.
        /// </summary>
        public EqExpressionConverter()
            : base((v1, v2) => object.Equals(v1, v2), convertArgs: true)
        {
        }

        /// <summary>
        /// Gets the call arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The call arguments.
        /// </returns>
        protected override IEnumerable<Expression> PreProcessArguments(IEnumerable<Expression> args)
        {
            args = ExpressionHelper.NormalizeBinaryExpressionArgs(args.ToList());
            return base.PreProcessArguments(args);
        }
    }
}