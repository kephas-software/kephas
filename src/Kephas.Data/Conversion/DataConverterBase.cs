// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConverterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A data converter base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A data converter base.
    /// </summary>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    public abstract class DataConverterBase<TSource, TTarget> : IDataConverter<TSource, TTarget>
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
        public abstract Task<IDataConversionResult> ConvertAsync(
            TSource source,
            TTarget target,
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken = default);

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
        public Task<IDataConversionResult> ConvertAsync(
            object source,
            object target,
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken = default)
        {
            return this.ConvertAsync((TSource)source, (TTarget)target, conversionContext, cancellationToken);
        }
    }
}
