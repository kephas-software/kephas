// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityValidationBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity validation behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Behaviors
{
    using System;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Validation;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// An entity validation behavior using the model space classifier, if applicable, for validating the entity using annotations.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class EntityValidationBehavior : DataAnnotationsBehaviorBase<object>
    {
        /// <summary>
        /// The model space provider.
        /// </summary>
        private readonly IModelSpaceProvider modelSpaceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidationBehavior"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public EntityValidationBehavior(IModelSpaceProvider modelSpaceProvider)
        {
            Requires.NotNull(modelSpaceProvider, nameof(modelSpaceProvider));

            this.modelSpaceProvider = modelSpaceProvider;
        }

        /// <summary>Creates the validation function.</summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>The new validation function.</returns>
        protected override Func<object, IEntityEntry, IDataOperationContext, IDataValidationResult> CreateValidationFn(ITypeInfo typeInfo)
        {
            var modelSpace = this.modelSpaceProvider.GetModelSpace();
            var entityClassifier = modelSpace.TryGetClassifier(typeInfo);

            return base.CreateValidationFn(entityClassifier ?? typeInfo);
        }
    }
}