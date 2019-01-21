// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumDataSourceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enum data source provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A data source provider for enumeration based properties.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class EnumDataSourceProvider : IDataSourceProvider
    {
        /// <summary>
        /// Determines whether the provider can handle the list source request.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if we can handle, false if not.
        /// </returns>
        public bool CanHandle(IPropertyInfo propertyInfo, IDataSourceContext context)
        {
            var typeInfo = ((IRuntimeTypeInfo)propertyInfo.ValueType).TypeInfo;

            if (typeInfo.IsEnum)
            {
                return true;
            }

            var projectedPropertyInfo =
                context.ProjectedEntityType?.Properties.FirstOrDefault(p => p.Name == propertyInfo.Name);

            if (projectedPropertyInfo != null)
            {
                typeInfo = ((IRuntimeTypeInfo)projectedPropertyInfo.ValueType).TypeInfo.GetNonNullableType();
                if (typeInfo.IsEnum)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the data source asynchronously.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the data source.
        /// </returns>
        public Task<IEnumerable<object>> GetDataSourceAsync(
            IPropertyInfo propertyInfo,
            IDataSourceContext context,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<IDataSourceItem> listSource = null;
            var type = this.GetEnumType(propertyInfo, context);
            if (type == null)
            {
                return Task.FromResult<IEnumerable<object>>(null);
            }

            var values = Enum.GetValues(type.AsType()).OfType<object>();
            listSource = values.Select(v => new DataSourceItem { Id = v, DisplayText = Enum.GetName(type, v) })
                                   .ToList();
            return Task.FromResult<IEnumerable<object>>(listSource);
        }

        /// <summary>
        /// Gets the enum type.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The enum type.
        /// </returns>
        private TypeInfo GetEnumType(IPropertyInfo propertyInfo, IDataSourceContext context)
        {
            var typeInfo = ((IRuntimeTypeInfo)propertyInfo.ValueType).TypeInfo.GetNonNullableType();
            if (typeInfo.IsEnum)
            {
                return typeInfo;
            }

            var projectedPropertyInfo =
                context.ProjectedEntityType?.Properties.FirstOrDefault(p => p.Name == propertyInfo.Name);

            if (projectedPropertyInfo != null)
            {
                typeInfo = ((IRuntimeTypeInfo)projectedPropertyInfo.ValueType).TypeInfo;
                if (typeInfo.IsEnum)
                {
                    return typeInfo;
                }
            }

            return null;
        }
    }
}