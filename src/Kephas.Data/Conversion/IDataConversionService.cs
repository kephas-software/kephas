// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataConversionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for data conversions.
    /// </summary>
    [SingletonAppServiceContract]
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
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        Task<IDataConversionResult> ConvertAsync<TSource, TTarget>(
            TSource source,
            TTarget target,
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken = default);
    }
}