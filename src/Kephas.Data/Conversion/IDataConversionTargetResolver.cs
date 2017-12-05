// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionTargetResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// Shared application service contract for resolving targets during data conversion.
    /// </summary>
    public interface IDataConversionTargetResolver
    {
        /// <summary>
        /// Tries to resolve the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="sourceEntity">The source entity.</param>
        /// <param name="sourceEntityInfo">The source entity information, if available.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        Task<object> TryResolveTargetEntityAsync(
            IDataContext targetDataContext,
            TypeInfo targetType,
            object sourceEntity,
            IEntityInfo sourceEntityInfo,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Shared application service contract for resolving targets during data conversion.
    /// </summary>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    [SharedAppServiceContract(ContractType = typeof(IDataConversionTargetResolver))]
    public interface IDataConversionTargetResolver<TSource, TTarget> : IDataConversionTargetResolver
    {
    }
}