// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryableSubstituteTypeConstantHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context queryable substitute type constant handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// An <see cref="ISubstituteTypeConstantHandler"/> for <see cref="IDataContextQueryProvider"/>.
    /// </summary>
    public class DataContextQueryableSubstituteTypeConstantHandler : ISubstituteTypeConstantHandler
    {
        /// <summary>
        /// Determines whether the provided type can be handled.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if th provided type can be handled, otherwise <c>false</c>.
        /// </returns>
        public bool CanHandle(Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDataContextQueryable<>));
        }

        /// <summary>
        /// Visits the provided value and returns a transformed value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="value">The value.</param>
        /// <param name="substituteType">The substitute type.</param>
        /// <returns>
        /// A transformed value.
        /// </returns>
        public object Visit(object value, Type substituteType)
        {
            var itemType = substituteType.GetTypeInfo().GenericTypeParameters[0];
            var queryProvider = ((IQueryable)value).Provider as IDataContextQueryProvider;
            if (queryProvider == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Expected a {typeof(IDataContextQueryProvider)} in query {value}.");
            }

            var genericQueryMethod = typeof(IDataContext).AsRuntimeTypeInfo().Methods[nameof(IDataContext.Query)].FirstOrDefault();
            var queryMethod = genericQueryMethod.MethodInfo.MakeGenericMethod(itemType);
            var convertedObject = queryMethod.Invoke(queryProvider.DataContext, new object[] { queryProvider.QueryContext });

            return convertedObject;
        }
    }
}