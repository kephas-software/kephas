// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListSubstituteTypeConstantHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the list substitute type generic handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// An <see cref="ISubstituteTypeConstantHandler"/> for lists.
    /// </summary>
    public class ListSubstituteTypeConstantHandler : ISubstituteTypeConstantHandler
    {
        /// <summary>
        /// Determines whether the provided type can be handled.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the provided type can be handled, otherwise <c>false</c>.
        /// </returns>
        public bool CanHandle(Type type)
        {
            return type == typeof(List<>);
        }

        /// <summary>
        /// Visits the provided value and returns a transformed value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="substituteType">The substitute type.</param>
        /// <returns>
        /// A transformed value.
        /// </returns>
        public object Visit(object value, Type substituteType)
        {
            var itemType = substituteType.GetTypeInfo().GenericTypeArguments[0];

            var convertedObject = EnumerableMethods.EnumerableOfType.MakeGenericMethod(itemType).Invoke(null, new[] { value });
            return convertedObject;
        }
    }
}