// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubstituteTypeConstantHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISubstituteTypeConstantHandler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;

    /// <summary>
    /// Handler for substituting a constant value with another one in the <see cref="SubstituteTypeExpressionVisitor"/>.
    /// </summary>
    public interface ISubstituteTypeConstantHandler
    {
        /// <summary>
        /// Determines whether the provided type can be handled.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the provided type can be handled, otherwise <c>false</c>.
        /// </returns>
        bool CanHandle(Type type);

        /// <summary>
        /// Visits the provided value and returns a transformed value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="substituteType">The substitute type.</param>
        /// <returns>
        /// A transformed value.
        /// </returns>
        object Visit(object value, Type substituteType);
    }
}