// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionTargetResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataConversionTargetResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for resolving targets during data conversion.
    /// </summary>
    public interface IDataConversionTargetResolver
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
        Task<object> TryResolveTargetEntityAsync(
            IDataContext targetDataContext,
            TypeInfo targetType,
            object sourceEntity,
            IEntityEntry sourceEntityEntry,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Singleton application service contract for resolving targets during data conversion.
    /// </summary>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    [SingletonAppServiceContract(ContractType = typeof(IDataConversionTargetResolver))]
    public interface IDataConversionTargetResolver<TSource, TTarget> : IDataConversionTargetResolver
    {
    }
}