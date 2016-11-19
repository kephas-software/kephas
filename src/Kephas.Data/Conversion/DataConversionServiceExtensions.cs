// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionServiceExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data conversion service extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Data.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IDataConversionService"/>.
    /// </summary>
    public static class DataConversionServiceExtensions
    {
        /// <summary>
        /// The convert asynchronous method.
        /// </summary>
        private static readonly MethodInfo ConvertAsyncMethod;

        /// <summary>
        /// Initializes static members of the <see cref="DataConversionServiceExtensions"/> class.
        /// </summary>
        static DataConversionServiceExtensions()
        {
            ConvertAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IDataConversionService)null).ConvertAsync<int, int>(0, 0, null, default(CancellationToken)));
        }

        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <param name="conversionService">The conversion service to act on.</param>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        public static Task<IDataConversionResult> ConvertAsync(
            this IDataConversionService conversionService,
            object source, 
            object target, 
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (target == null)
            {
                var exception = new DataConversionException(Strings.DefaultDataConversionService_NonTypedTargetIsNull_Exception);
                if (conversionContext.ThrowOnError)
                {
                    throw exception;
                }

                return Task.FromResult((IDataConversionResult)DataConversionResult.FromException(exception));
            }

            var convertAsync = ConvertAsyncMethod.MakeGenericMethod(source.GetType(), target.GetType());
            var result = convertAsync.Call(conversionService, source, target, conversionContext, cancellationToken);
            return (Task<IDataConversionResult>)result;
        }
    }
}