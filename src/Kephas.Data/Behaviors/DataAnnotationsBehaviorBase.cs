// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data annotations behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Validation;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Base class for data behaviors resulting from property annotations, like any type of validations.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public abstract class DataAnnotationsBehaviorBase<TEntity> : DataBehaviorBase<TEntity>
    {
        /// <summary>
        /// Gets the key of the validation function in the type info.
        /// </summary>
        /// <value>
        /// The " data annotations behavior base validation fn".
        /// </value>
        private const string ValidationFnKey = "__ValidationFn";

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IDataValidationResult"/>.
        /// </returns>
        public override IDataValidationResult Validate(TEntity entity, IEntityEntry entityEntry, IDataOperationContext operationContext)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            entityEntry = entityEntry ?? throw new System.ArgumentNullException(nameof(entityEntry));

            if (entityEntry.ChangeState == ChangeState.Deleted)
            {
                return DataValidationResult.Success;
            }

            var typeInfo = entity.GetTypeInfo();
            if (!(typeInfo[ValidationFnKey] is Func<object, IEntityEntry, IDataOperationContext, IDataValidationResult> validationFn))
            {
                validationFn = this.CreateValidationFn(typeInfo);
                typeInfo[ValidationFnKey] = validationFn;
            }

            return validationFn(entity, entityEntry, operationContext);
        }

        /// <summary>
        /// Creates the validation function.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// The new validation function.
        /// </returns>
        protected virtual Func<object, IEntityEntry, IDataOperationContext, IDataValidationResult> CreateValidationFn(ITypeInfo typeInfo)
        {
            var propValidations = new List<Func<object, IDataValidationResultItem?>>();

            foreach (var propInfo in typeInfo.Properties)
            {
                var propInfoValidations = propInfo.Annotations.OfType<ValidationAttribute>().ToList();
                propInfoValidations.AddRange(propInfo.Annotations
                    .OfType<IAttributeProvider>()
                    .Select(p => p.GetAttribute<ValidationAttribute>()!)
                    .Where(a => a != null));
                var propInfoLocalization = propInfo.GetLocalization();
                foreach (var propInfoValidation in propInfoValidations)
                {
                    string nameAccessor() => propInfoLocalization.Name ?? propInfo.Name;
                    string messageAccessor() => propInfoValidation.FormatErrorMessage(nameAccessor());
                    propValidations.Add(e => propInfoValidation.IsValid(propInfo.GetValue(e)) ? null : new DataValidationResultItem(messageAccessor(), nameAccessor()));
                }
            }

            if (propValidations.Count == 0)
            {
                return (e, entityEntry, context) => DataValidationResult.Success;
            }

            return (e, entityEntry, context) => this.GetDataValidationResult(e, propValidations);
        }

        /// <summary>
        /// Gets the data validation result.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="propValidations">The property validations.</param>
        /// <returns>
        /// The data validation result.
        /// </returns>
        private IDataValidationResult GetDataValidationResult(object entity, List<Func<object, IDataValidationResultItem?>> propValidations)
        {
            var propResults = propValidations
                .Select(v => v(entity)!)
                .Where(res => res != null)
                .ToArray();
            return propResults.Length == 0
                ? DataValidationResult.Success
                : new DataValidationResult(propResults);
        }
    }
}