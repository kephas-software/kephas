// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConverterBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A data converter base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// A data converter base.
    /// </summary>
    /// <typeparam name="TSource">Type of the source.</typeparam>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    public abstract class DataConverterBase<TSource, TTarget> : Loggable, IDataConverter<TSource, TTarget>
    {
        private bool isInitialized;

        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
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
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        Task<IDataConversionResult> IDataConverter.ConvertAsync(
            object source,
            object target,
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken)
        {
            this.EnsureInitialized(conversionContext);
            return this.ConvertAsync((TSource)source, (TTarget)target, conversionContext, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureInitialized(IContext context)
        {
            if (!this.isInitialized)
            {
                this.Logger = this.GetLogger(context);
                this.isInitialized = true;
            }
        }
    }
}
