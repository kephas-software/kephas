// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelTypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IModelTypeResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Interface for model type resolver.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IModelTypeResolver
    {
        /// <summary>
        /// Resolves the model type.
        /// </summary>
        /// <param name="rawType">The raw type.</param>
        /// <param name="context">Optional. A context for the resolution.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// The resolved model type or <c>null</c>, if <paramref name="throwOnNotFound"/> is set to false and a model type could not be found.
        /// </returns>
        ITypeInfo ResolveModelType(ITypeInfo rawType, IContext? context = null, bool throwOnNotFound = true);
    }
}