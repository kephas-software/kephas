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

    using Kephas.Data.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// An <see cref="ISubstituteTypeConstantHandler"/> for <see cref="IDataContextQueryProvider"/>.
    /// </summary>
    public class DataContextQueryableSubstituteTypeConstantHandler : ISubstituteTypeConstantHandler
    {
        /// <summary>
        /// The data context query method.
        /// </summary>
        private static readonly MethodInfo DataContextQueryMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((IDataContext)null).Query<string>(null));

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
            var itemType = substituteType.GetTypeInfo().GenericTypeArguments[0];
            if (!(((IQueryable)value).Provider is IDataContextQueryProvider queryProvider))
            {
                throw new InvalidOperationException(string.Format(Strings.DataContextQueryableSubstituteTypeConstantHandler_BadQueryProvider_Exception, typeof(IDataContextQueryProvider), value));
            }

            var queryMethod = DataContextQueryMethod.MakeGenericMethod(itemType);

            // TODO The element type provided in the parent query operation context may not be appropriate to pass to the Query
            // method, as it may contain another entity to query
            var convertedObject = queryMethod.Invoke(queryProvider.DataContext, new object[] { queryProvider.QueryOperationContext });

            return convertedObject;
        }
    }
}