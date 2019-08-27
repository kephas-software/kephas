// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Services.Composition;

    /// <summary>
    /// An ordered service collection extensions.
    /// </summary>
    public static class OrderedServiceCollectionExtensions
    {
        /// <summary>
        /// Orders the given factories.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
        /// <param name="factories">The factories to act on.</param>
        /// <returns>
        /// A list of ordered services.
        /// </returns>
        public static IOrderedServiceCollection<T, TMetadata> Order<T, TMetadata>(
            this IEnumerable<IExportFactory<T, TMetadata>> factories)
            where TMetadata : AppServiceMetadata
        {
            return new OrderedServiceCollection<T, TMetadata>(factories);
        }
    }
}