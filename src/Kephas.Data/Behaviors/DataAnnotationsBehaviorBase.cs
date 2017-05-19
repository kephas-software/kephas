// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsBehaviorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

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
        private const string ValidationFnKey = "DataAnnotationsBehaviorBase_ValidationFn";

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IDataValidationResult"/>.
        /// </returns>
        public override IDataValidationResult Validate(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext)
        {
            Requires.NotNull(entity as object, nameof(entity));
            Requires.NotNull(entityInfo, nameof(entityInfo));

            if (entityInfo.ChangeState == ChangeState.Deleted)
            {
                return DataValidationResult.Success;
            }

            var typeInfo = entity.GetTypeInfo();
            var validationFn = typeInfo[ValidationFnKey] as Func<object, IEntityInfo, IDataOperationContext, IDataValidationResult>;
            if (validationFn == null)
            {
                validationFn = this.CreateValidationFn(typeInfo);
                typeInfo[ValidationFnKey] = validationFn;
            }

            return validationFn(entity, entityInfo, operationContext);
        }

        /// <summary>
        /// Creates the validation function.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// The new validation function.
        /// </returns>
        protected virtual Func<object, IEntityInfo, IDataOperationContext, IDataValidationResult> CreateValidationFn(ITypeInfo typeInfo)
        {
            var propValidations = new List<Func<object, IDataValidationResultItem>>();

            foreach (var propInfo in typeInfo.Properties)
            {
                var propInfoValidations = propInfo.Annotations.OfType<ValidationAttribute>().ToList();
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
                return (e, entityInfo, context) => DataValidationResult.Success;
            }

            return (e, entityInfo, context) => this.GetDataValidationResult(e, propValidations);
        }

        /// <summary>
        /// Gets the data validation result.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="propValidations">The property validations.</param>
        /// <returns>
        /// The data validation result.
        /// </returns>
        private IDataValidationResult GetDataValidationResult(object entity, List<Func<object, IDataValidationResultItem>> propValidations)
        {
            var propResults = propValidations.Select(v => v(entity)).Where(res => res != null).ToArray();
            if (propResults.Length == 0)
            {
                return DataValidationResult.Success;
            }

            return new DataValidationResult(propResults);
        }
    }
}