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
    public interface IDataConversionService
    {
        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
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
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        public Task<IDataConversionResult> ConvertAsync<TSource, TTarget>(TSource source, TTarget target,
            IDataConversionContext conversionContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(conversionContext != null);

            return Contract.Result<Task<IDataConversionResult>>();
        }
    }
}