// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableQuerySubstituteTypeConstantHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the enumerable query substitute type constant handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// An <see cref="ISubstituteTypeConstantHandler"/> for <see cref="EnumerableQuery"/>.
    /// </summary>
    public class EnumerableQuerySubstituteTypeConstantHandler : ISubstituteTypeConstantHandler
    {
        /// <summary>
        /// Determines whether the provided type can be handled.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the provided type can be handled, otherwise <c>false</c>.
        /// </returns>
        public bool CanHandle(Type type)
        {
            return type == typeof(EnumerableQuery<>);
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

            // avoid adding an OfType() expression if the enumerable item type is the same type as the substituted type.
            var valueEnumerableItemType = value?.GetType().TryGetEnumerableItemType();
            if (itemType == valueEnumerableItemType)
            {
                return value;
            }

            var convertedObject = QueryableMethods.QueryableOfType.MakeGenericMethod(itemType).Invoke(null, new[] { value });
            return convertedObject;
        }
    }
}