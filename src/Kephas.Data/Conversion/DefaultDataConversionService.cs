// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataConversionService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data conversion service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion.Composition;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data conversion service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataConversionService : IDataConversionService
    {
        /// <summary>
        /// The converter export factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataConverter, DataConverterMetadata>> converterExportFactories;

        /// <summary>
        /// The converters cache.
        /// </summary>
        private readonly ConcurrentDictionary<TypeInfo, ConcurrentDictionary<TypeInfo, IList<IDataConverter>>>
          convertersCache = new ConcurrentDictionary<TypeInfo, ConcurrentDictionary<TypeInfo, IList<IDataConverter>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataConversionService"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="converterExportFactories">The converter export factories.</param>
        public DefaultDataConversionService(
            IAmbientServices ambientServices,
            ICollection<IExportFactory<IDataConverter, DataConverterMetadata>> converterExportFactories)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(converterExportFactories, nameof(converterExportFactories));

            this.AmbientServices = ambientServices;
            this.converterExportFactories = converterExportFactories;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultDataConversionService> Logger { get; set; }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

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
        public Task<IDataConversionResult> ConvertAsync<TSource, TTarget>(TSource source, TTarget target, IDataConversionContext conversionContext, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(conversionContext, nameof(conversionContext));

            if (source == null)
            {
                var exception = new DataConversionException(Strings.DefaultDataConversionService_NonTypedSourceIsNull_Exception);
                if (conversionContext.ThrowOnError)
                {
                    throw exception;
                }

                var noSourceResult = DataConversionResult.FromException(exception);
                noSourceResult.Target = target;
                return Task.FromResult((IDataConversionResult)noSourceResult);
            }

            var sourceType = this.GetInstanceTypeInfo(source, typeof(TSource), conversionContext.RootSourceType);
            var targetType = this.GetInstanceTypeInfo(target, typeof(TTarget), conversionContext.RootTargetType);

            if (target == null && targetType == null)
            {
                var exception = new DataConversionException(Strings.DefaultDataConversionService_NonTypedTargetIsNull_Exception);
                if (conversionContext.ThrowOnError)
                {
                    throw exception;
                }

                return Task.FromResult((IDataConversionResult)DataConversionResult.FromException(exception));
            }

            var result = this.ConvertCoreAsync(source, sourceType, target, targetType, conversionContext, cancellationToken);
            return result;
        }

        /// <summary>
        /// Gets the type information for the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="declaredType">Type of the declared.</param>
        /// <param name="providedType">Type of the provided.</param>
        /// <returns>
        /// The instance type information.
        /// </returns>
        protected virtual TypeInfo GetInstanceTypeInfo(object instance, Type declaredType, Type providedType)
        {
            var instanceType = declaredType == typeof(object) ? providedType : declaredType;
            if (instanceType == null || instanceType == typeof(object))
            {
                return instance?.GetType().GetTypeInfo();
            }

            return instanceType.GetTypeInfo();
        }

        /// <summary>
        /// Converts the source object to the target object asynchronously.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="sourceType">The type of the source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data conversion result.
        /// </returns>
        protected virtual async Task<IDataConversionResult> ConvertCoreAsync(object source, TypeInfo sourceType, object target, TypeInfo targetType, IDataConversionContext conversionContext, CancellationToken cancellationToken = default)
        {
            var matchingConvertersDictionary = this.convertersCache.GetOrAdd(sourceType, _ => new ConcurrentDictionary<TypeInfo, IList<IDataConverter>>());
            var matchingConverters = matchingConvertersDictionary.GetOrAdd(targetType, _ => this.ComputeMatchingConverters(sourceType, targetType));

            var result = this.CreateDataConversionResult();

            target = await this.EnsureTargetEntity(source, target, targetType, conversionContext, cancellationToken).PreserveThreadContext();

            result.Target = target;

            if (matchingConverters.Count == 0)
            {
                return result;
            }

            IList<IDataConversionResult> convertersResult = null;
            if (conversionContext.ThrowOnError)
            {
                convertersResult = await this.InvokeConvertersAsync(matchingConverters, source, target, conversionContext, cancellationToken).PreserveThreadContext();
            }
            else
            {
                try
                {
                    convertersResult = await this.InvokeConvertersAsync(matchingConverters, source, target, conversionContext, cancellationToken).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                }
            }

            this.AddConverterResultsToOverallResult(result, convertersResult);
            return result;
        }

        /// <summary>
        /// Ensures that a target entity is retrieved or created.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        protected virtual async Task<object> EnsureTargetEntity(
            object source,
            object target,
            TypeInfo targetType,
            IDataConversionContext conversionContext,
            CancellationToken cancellationToken)
        {
            if (target != null || conversionContext.TargetDataContext == null)
            {
                return target;
            }

            var targetDataContext = conversionContext.TargetDataContext;
            var sourceEntityInfo = conversionContext.SourceDataContext.GetEntityInfo(source);
            var sourceId = sourceEntityInfo?.EntityId;

            var sourceChangeState = sourceEntityInfo?.ChangeState;

            if (!Id.IsEmpty(sourceId))
            {
                target = await this.FindTargetEntityAsync(
                             targetDataContext,
                             targetType,
                             sourceId,
                             throwIfNotFound: false,
                             cancellationToken: cancellationToken).PreserveThreadContext();
                if (target != null)
                {
                    return target;
                }
            }

            if (sourceChangeState == ChangeState.Added || sourceChangeState == ChangeState.AddedOrChanged || sourceChangeState == null)
            {
                target = await this.CreateTargetEntityAsync(targetDataContext, targetType, cancellationToken).PreserveThreadContext();
            }
            else
            {
                var exception  = new NotFoundDataException(string.Format(Strings.DataContext_FindAsync_NotFound_Exception, $"Id == {sourceId}"));
                throw exception;
            }

            return target;
        }

        /// <summary>
        /// Searches for the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="throwIfNotFound">True to throw if not found.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        protected virtual async Task<object> FindTargetEntityAsync(IDataContext targetDataContext, TypeInfo targetType, object id, bool throwIfNotFound, CancellationToken cancellationToken)
        {
            var target = await targetDataContext.FindAsync(
                             new FindContext(targetDataContext, targetType.AsType(), id, throwIfNotFound),
                             cancellationToken).PreserveThreadContext();
            return target;
        }

        /// <summary>
        /// Creates the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the new target entity.
        /// </returns>
        protected virtual async Task<object> CreateTargetEntityAsync(IDataContext targetDataContext, TypeInfo targetType, CancellationToken cancellationToken)
        {
            var target = await targetDataContext.CreateEntityAsync(
                             new CreateEntityContext<object>(targetDataContext)
                             {
                                 EntityType = targetType.AsType()
                             },
                             cancellationToken).PreserveThreadContext();
            return target;
        }

        /// <summary>
        /// Invokes the converters asynchronously for the convert operation.
        /// </summary>
        /// <param name="matchingConverters">The matching converters.</param>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The list of results from converters.
        /// </returns>
        protected virtual async Task<IList<IDataConversionResult>> InvokeConvertersAsync(IEnumerable<IDataConverter> matchingConverters, object source, object target, IDataConversionContext conversionContext, CancellationToken cancellationToken)
        {
            var converterResults = new List<IDataConversionResult>();
            foreach (var converter in matchingConverters)
            {
                var converterResult = await converter.ConvertAsync(source, target, conversionContext, cancellationToken).PreserveThreadContext();
                converterResults.Add(converterResult);
            }

            return converterResults;
        }

        /// <summary>
        /// Creates a data conversion result.
        /// </summary>
        /// <returns>
        /// The new data conversion result.
        /// </returns>
        protected virtual IDataConversionResult CreateDataConversionResult()
        {
            return new DataConversionResult();
        }

        /// <summary>
        /// Determines whether the converter metadata type matches the tested type.
        /// </summary>
        /// <param name="metadataType">The type from metadata.</param>
        /// <param name="checkType">The type to be checked.</param>
        /// <returns><c>true</c> if the converter metadata type matches the tested type; otherwise <c>false</c>.</returns>
        private static bool IsConverterTypeMatch(TypeInfo metadataType, TypeInfo checkType)
        {
            return metadataType.IsAssignableFrom(checkType);
        }

        /// <summary>
        /// Computes the matching converters.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>
        /// The matching converters.
        /// </returns>
        private IList<IDataConverter> ComputeMatchingConverters(TypeInfo sourceType, TypeInfo targetType)
        {
            var matchingConverters = this.converterExportFactories
                .Where(lc => IsConverterTypeMatch(lc.Metadata.SourceType.GetTypeInfo(), sourceType) && IsConverterTypeMatch(lc.Metadata.TargetType.GetTypeInfo(), targetType))
                .OrderBy(lc => lc.Metadata.ProcessingPriority)
                .Select(lc => lc.CreateExport().Value)
                .ToList();

            if (matchingConverters.Count == 0)
            {
                this.Logger.Warn(string.Format(Strings.DataConverterNotFound_Exception, sourceType, targetType));
            }

            return matchingConverters;
        }

        /// <summary>
        /// Adds the converter results to the overall result.
        /// </summary>
        /// <param name="result">The overall result.</param>
        /// <param name="convertersResult">The converters result.</param>
        private void AddConverterResultsToOverallResult(IDataConversionResult result, IList<IDataConversionResult> convertersResult)
        {
            if (convertersResult == null)
            {
                return;
            }

            var overallExceptions = new List<Exception>();
            foreach (var converterResult in convertersResult)
            {
                result.Target = converterResult.Target;
                if (converterResult.Exception != null)
                {
                    overallExceptions.Add(converterResult.Exception);
                }
            }

            if (overallExceptions.Count > 0)
            {
                result.Exception = new AggregateException(overallExceptions);
            }
        }
    }
}