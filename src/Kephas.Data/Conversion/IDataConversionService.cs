// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataConversionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for data conversions.
    /// </summary>
    [ContractClass(typeof(DataConversionServiceContractClass))]
    [SharedAppServiceContract]
    public interface IDataConversionService : IAmbientServicesAware
    {
        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        Task<IDataConversionResult> ConvertAsync<TSource, TTarget>(TSource source, TTarget target, IDataConversionContext conversionContext, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// A data conversion service contract class.
    /// </summary>
    [ContractClassFor(typeof(IDataConversionService))]
    internal abstract class DataConversionServiceContractClass : IDataConversionService
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public abstract IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        public Task<IDataConversionResult> ConvertAsync<TSource, TTarget>(
            TSource source,
            TTarget target,
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(conversionContext != null);

            return Contract.Result<Task<IDataConversionResult>>();
        }
    }
}