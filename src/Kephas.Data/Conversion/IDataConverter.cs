// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataConverter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Services;

    /// <summary>
    /// Service for converting a source object to a target object.
    /// </summary>
    public interface IDataConverter
    {
        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        Task<IDataConversionResult> ConvertAsync(object source, object target, IDataConversionContext conversionContext, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Service for converting a typed source object to a typed target object.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    [AppServiceContract(ContractType = typeof(IDataConverter))]
    public interface IDataConverter<in TSource, in TTarget> : IDataConverter
    {
        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        Task<IDataConversionResult> ConvertAsync(TSource source, TTarget target, IDataConversionContext conversionContext, CancellationToken cancellationToken = default);
    }
}