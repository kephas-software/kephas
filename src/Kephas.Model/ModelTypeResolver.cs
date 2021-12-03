// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelTypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A model type resolver.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class ModelTypeResolver : IModelTypeResolver
    {
        /// <summary>
        /// The model space provider.
        /// </summary>
        private readonly IModelSpaceProvider modelSpaceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelTypeResolver"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public ModelTypeResolver(IModelSpaceProvider modelSpaceProvider)
        {
            modelSpaceProvider = modelSpaceProvider ?? throw new System.ArgumentNullException(nameof(modelSpaceProvider));

            this.modelSpaceProvider = modelSpaceProvider;
        }

        /// <summary>
        /// Resolves the model type.
        /// </summary>
        /// <param name="rawType">The raw type.</param>
        /// <param name="context">Optional. A context for the resolution.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw or not when the indicated
        ///                               type is not found.</param>
        /// <returns>
        /// The resolved model type or <c>null</c>, if <paramref name="throwOnNotFound" /> is set to
        /// false and a model type could not be found.
        /// </returns>
        public ITypeInfo ResolveModelType(ITypeInfo rawType, IContext? context = null, bool throwOnNotFound = true)
        {
            var modelSpace = this.modelSpaceProvider.GetModelSpace();
            var classifier = modelSpace.TryGetClassifier(rawType, context);
            return classifier ?? rawType;
        }
    }
}