// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionTargetResolverBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data conversion target resolver base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion.TargetResolvers
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for data conversion target resolver.
    /// </summary>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    public abstract class DataConversionTargetResolverBase<TSource, TTarget> : IDataConversionTargetResolver<TSource, TTarget>
    {
        /// <summary>
        /// Tries to resolve the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="sourceEntity">The source entity.</param>
        /// <param name="sourceEntityEntry">The source entity entry, if available.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        public async Task<object?> TryResolveTargetEntityAsync(
            IDataContext targetDataContext,
            TypeInfo targetType,
            object sourceEntity,
            IEntityEntry sourceEntityEntry,
            CancellationToken cancellationToken = default)
        {
            return await this.TryResolveTargetEntityAsync(
                       targetDataContext,
                       targetType,
                       (TSource)sourceEntity,
                       sourceEntityEntry,
                       cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Tries to resolve the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="sourceEntity">The source entity.</param>
        /// <param name="sourceEntityEntry">The source entity entry.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        protected abstract Task<TTarget> TryResolveTargetEntityAsync(
            IDataContext targetDataContext,
            TypeInfo targetType,
            TSource sourceEntity,
            IEntityEntry sourceEntityEntry,
            CancellationToken cancellationToken);
    }
}