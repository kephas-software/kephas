namespace Kephas.Data.Linq.Expressions
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// An <see cref="ISubstituteTypeConstantHandler"/> for <see cref="IDataRepositoryQueryProvider"/>.
    /// </summary>
    public class DataRepositoryQueryableSubstituteTypeConstantHandler : ISubstituteTypeConstantHandler
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
            return type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDataRepositoryQueryable<>));
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
            var queryProvider = ((IQueryable)value).Provider as IDataRepositoryQueryProvider;
            if (queryProvider == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Expected a {typeof(IDataRepositoryQueryProvider)} in query {value}.");
            }

            var genericQueryMethod = typeof(IDataRepository).AsDynamicTypeInfo().Methods[nameof(IDataRepository.Query)].FirstOrDefault();
            var queryMethod = genericQueryMethod.MethodInfo.MakeGenericMethod(itemType);
            var convertedObject = queryMethod.Invoke(queryProvider.Repository, new object[] { queryProvider.QueryContext });

            return convertedObject;
        }
    }
}