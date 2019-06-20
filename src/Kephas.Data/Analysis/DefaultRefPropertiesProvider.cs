// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRefPropertiesProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default reference properties provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Analysis
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas;
    using Kephas.Data;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A default reference properties provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRefPropertiesProvider : IRefPropertiesProvider
    {
        /// <summary>
        /// The properties cache.
        /// </summary>
        private readonly ConcurrentDictionary<ITypeInfo, IEnumerable<IPropertyInfo>> propertiesCache = new ConcurrentDictionary<ITypeInfo, IEnumerable<IPropertyInfo>>();

        /// <summary>
        /// The empty references.
        /// </summary>
        private readonly IRef[] emptyRefs = new IRef[0];

        /// <summary>
        /// Gets the reference properties of the provided entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// An enumeration of reference properties.
        /// </returns>
        public IEnumerable<IRef> GetRefProperties(object entity)
        {
            if (!(entity is IInstance instanceEntity))
            {
                return this.emptyRefs;
            }

            var typeInfo = instanceEntity.GetTypeInfo();
            if (typeInfo == null)
            {
                return this.emptyRefs;
            }

            var getters = this.propertiesCache.GetOrAdd(typeInfo, this.ComputeRefProperties);
            return getters.Select(g => (IRef)g.GetValue(entity));
        }

        /// <summary>
        /// Enumerates compute reference properties in this collection.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process compute reference properties in this
        /// collection.
        /// </returns>
        protected virtual IEnumerable<IPropertyInfo> ComputeRefProperties(ITypeInfo typeInfo)
        {
            // WARNING: for interfaces, the properties are from the interface only, excluding those from the base interfaces
            return typeInfo.Properties.Where(this.IsRefProperty).ToList();
        }

        /// <summary>
        /// Query if 'propertyInfo' is reference property.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <returns>
        /// True if reference property, false if not.
        /// </returns>
        protected virtual bool IsRefProperty(IPropertyInfo propertyInfo)
        {
            if (propertyInfo.ValueType is IRuntimeTypeInfo runtimePropertyType)
            {
                return typeof(IRef).IsAssignableFrom(runtimePropertyType.TypeInfo);
            }

            return propertyInfo.ValueType.Name == nameof(IRef);
        }
    }
}