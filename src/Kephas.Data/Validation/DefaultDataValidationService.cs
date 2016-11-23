// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataValidationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data validation service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Validation.Composition;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default validation service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataValidationService : IDataValidationService
    {
        /// <summary>
        /// The data validator factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataValidator, DataValidatorMetadata>> dataValidatorFactories;

        /// <summary>
        /// The validators cache.
        /// </summary>
        private readonly ConcurrentDictionary<Type, IEnumerable<IDataValidator>> validatorsCache =
            new ConcurrentDictionary<Type, IEnumerable<IDataValidator>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataValidationService"/> class.
        /// </summary>
        /// <param name="dataValidatorFactories">The data validator factories.</param>
        public DefaultDataValidationService(ICollection<IExportFactory<IDataValidator, DataValidatorMetadata>> dataValidatorFactories)
        {
            Contract.Requires(dataValidatorFactories != null);

            this.dataValidatorFactories = dataValidatorFactories;
        }

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="operationContext">Context for the operation.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        public async Task<IDataValidationResult> ValidateAsync(object obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                return DataValidationResult.Success;
            }

            var results = new List<IDataValidationResult>();
            var validators = this.validatorsCache.GetOrAdd(obj.GetType(), this.ComputeDataValidators);
            foreach (var validator in validators)
            {
                results.Add(await validator.ValidateAsync(obj, operationContext, cancellationToken).PreserveThreadContext());
            }

            return DataValidationResult.Success;
        }

        /// <summary>
        /// Calculates the data validators matching the type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>
        /// An enumeration of <see cref="IDataValidator"/> objects.
        /// </returns>
        private IEnumerable<IDataValidator> ComputeDataValidators(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var validators = (from f in this.dataValidatorFactories
                                where typeInfo.IsAssignableFrom(f.Metadata.EntityType.GetTypeInfo())
                                orderby f.Metadata.ProcessingPriority
                                select f.CreateExport().Value)
                             .ToList();
            return validators;
        }
    }
}