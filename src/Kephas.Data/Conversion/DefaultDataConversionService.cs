// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataConversionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data conversion service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Data.Conversion
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Resources;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data conversion service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataConversionService : Loggable, IDataConversionService
    {
        /// <summary>
        /// The converter export factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataConverter, DataConverterMetadata>> converterExportFactories;

        /// <summary>
        /// Target resolver factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>> targetResolverFactories;

        /// <summary>
        /// The converters cache.
        /// </summary>
        private readonly ConcurrentDictionary<TypeInfo, ConcurrentDictionary<TypeInfo, IList<IDataConverter>>>
          convertersCache = new ConcurrentDictionary<TypeInfo, ConcurrentDictionary<TypeInfo, IList<IDataConverter>>>();

        /// <summary>
        /// The converters cache.
        /// </summary>
        private readonly ConcurrentDictionary<TypeInfo, ConcurrentDictionary<TypeInfo, IList<IDataConversionTargetResolver>>>
            targetResolversCache = new ConcurrentDictionary<TypeInfo, ConcurrentDictionary<TypeInfo, IList<IDataConversionTargetResolver>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataConversionService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="converterExportFactories">The converter export factories.</param>
        /// <param name="targetResolverFactories">The target resolver factories.</param>
        public DefaultDataConversionService(
            IServiceProvider serviceProvider,
            ICollection<IExportFactory<IDataConverter, DataConverterMetadata>> converterExportFactories,
            ICollection<IExportFactory<IDataConversionTargetResolver, DataConversionTargetResolverMetadata>> targetResolverFactories)
            : base(serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            converterExportFactories = converterExportFactories ?? throw new System.ArgumentNullException(nameof(converterExportFactories));
            targetResolverFactories = targetResolverFactories ?? throw new System.ArgumentNullException(nameof(targetResolverFactories));

            this.ServiceProvider = serviceProvider;
            this.converterExportFactories = converterExportFactories;
            this.targetResolverFactories = targetResolverFactories;
        }

        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The injector.
        /// </value>
        public IServiceProvider ServiceProvider { get; }

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
            conversionContext = conversionContext ?? throw new System.ArgumentNullException(nameof(conversionContext));

            cancellationToken.ThrowIfCancellationRequested();

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

            var sourceType = this.GetInstanceTypeInfo(source, typeof(TSource), conversionContext.RootSourceType, conversionContext);
            var targetType = this.GetInstanceTypeInfo(target, typeof(TTarget), conversionContext.RootTargetType, conversionContext);

            if (target == null && (targetType == null || targetType.AsType() == typeof(object)))
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
        /// <param name="conversionContext">The conversion context.</param>
        /// <returns>
        /// The instance type information.
        /// </returns>
        protected virtual TypeInfo GetInstanceTypeInfo(object instance, Type declaredType, Type providedType, IDataConversionContext conversionContext)
        {
            // get the widest type:
            // 1. if the instance is provided, get the type from the instance itself
            // 2. otherwise get the widest type from the declared one and the provided one.
            var instanceTypeInfo = instance?.GetType().GetTypeInfo();
            if (instanceTypeInfo == null)
            {
                var declaredTypeInfo = declaredType.GetTypeInfo();
                if (providedType == null)
                {
                    instanceTypeInfo = declaredTypeInfo;
                }
                else
                {
                    instanceTypeInfo = providedType.GetTypeInfo();
                    if (instanceTypeInfo.IsAssignableFrom(declaredTypeInfo))
                    {
                        instanceTypeInfo = declaredTypeInfo;
                    }
                }
            }

            return instanceTypeInfo;
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
            result.Complete();

            if (matchingConverters.Count == 0)
            {
                return result;
            }

            IList<IDataConversionResult>? convertersResult = null;
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
                    result.Fail(ex);
                }
            }

            this.AddConverterResultsToOverallResult(result, convertersResult!);
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
            if (target != null)
            {
                return target;
            }

            var sourceDataContext = conversionContext.GetDataContext(source);
            var sourceEntityEntry = sourceDataContext?.GetEntityEntry(source);
            var sourceId = sourceEntityEntry?.EntityId;
            var sourceChangeState = sourceEntityEntry?.ChangeState;

            targetType = conversionContext.RootTargetType?.GetTypeInfo() ?? targetType;
            var targetDataContext = conversionContext.GetDataContext(targetType);
            target = await this.TryResolveTargetEntityAsync(
                         targetDataContext,
                         targetType,
                         source,
                         sourceEntityEntry,
                         cancellationToken: cancellationToken).PreserveThreadContext();
            if (target != null)
            {
                return target;
            }

            if (sourceChangeState == ChangeState.Added || sourceChangeState == ChangeState.AddedOrChanged || sourceChangeState == null)
            {
                target = await this.CreateTargetEntityAsync(targetDataContext, targetType, conversionContext, cancellationToken).PreserveThreadContext();
            }
            else
            {
                var exception = new NotFoundDataException(string.Format(Strings.DataContext_FindAsync_NotFound_Exception, $"Id == {sourceId}"));
                throw exception;
            }

            return target;
        }

        /// <summary>
        /// Tries to resolve the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="sourceEntity">The source entity.</param>
        /// <param name="sourceEntityEntry">The source entity entry.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the target entity.
        /// </returns>
        protected virtual async Task<object> TryResolveTargetEntityAsync(IDataContext targetDataContext, TypeInfo targetType, object sourceEntity, IEntityEntry sourceEntityEntry, CancellationToken cancellationToken)
        {
            var sourceType = sourceEntity.GetType().GetTypeInfo();
            var matchingResolversDictionary = this.targetResolversCache.GetOrAdd(sourceType, _ => new ConcurrentDictionary<TypeInfo, IList<IDataConversionTargetResolver>>());
            var matchingResolvers = matchingResolversDictionary.GetOrAdd(targetType, _ => this.ComputeMatchingTargetResolvers(sourceType, targetType));

            foreach (var resolver in matchingResolvers)
            {
                var target = await resolver.TryResolveTargetEntityAsync(
                                 targetDataContext,
                                 targetType,
                                 sourceEntity,
                                 sourceEntityEntry,
                                 cancellationToken).PreserveThreadContext();
                if (target != null)
                {
                    return target;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the target entity asynchronously.
        /// </summary>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <param name="targetType">The type of the target object.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the new target entity.
        /// </returns>
        protected virtual async Task<object> CreateTargetEntityAsync(IDataContext targetDataContext, TypeInfo targetType, IDataConversionContext conversionContext, CancellationToken cancellationToken)
        {
            var target = await targetDataContext.CreateAsync(
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
                .Order()
                .Select(lc => lc.CreateExportedValue())
                .ToList();

            if (matchingConverters.Count == 0)
            {
                this.Logger.Warn(Strings.DataConverterNotFound_Exception, sourceType, targetType);
            }

            return matchingConverters;
        }

        /// <summary>
        /// Computes the matching target resolvers.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>
        /// The matching target resolvers.
        /// </returns>
        private IList<IDataConversionTargetResolver> ComputeMatchingTargetResolvers(TypeInfo sourceType, TypeInfo targetType)
        {
            var matchingConverters = this.targetResolverFactories
                .Where(lc => IsConverterTypeMatch(lc.Metadata.SourceType.GetTypeInfo(), sourceType) && IsConverterTypeMatch(lc.Metadata.TargetType.GetTypeInfo(), targetType))
                .Order()
                .Select(lc => lc.CreateExportedValue())
                .ToList();

            if (matchingConverters.Count == 0)
            {
                this.Logger.Warn(Strings.DataConverterNotFound_Exception, sourceType, targetType);
            }

            return matchingConverters;
        }

        /// <summary>
        /// Adds the converter results to the overall result.
        /// </summary>
        /// <param name="result">The overall result.</param>
        /// <param name="convertersResult">The converters result.</param>
        private void AddConverterResultsToOverallResult(IDataConversionResult result, IList<IDataConversionResult>? convertersResult)
        {
            if (convertersResult == null)
            {
                return;
            }

            foreach (var converterResult in convertersResult)
            {
                result.MergeAll(converterResult);
            }
        }
    }
}