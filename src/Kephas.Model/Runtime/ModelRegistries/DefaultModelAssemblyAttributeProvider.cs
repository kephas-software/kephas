// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelAssemblyAttributeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default model assembly attribute provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.ModelRegistries
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// A default model assembly attribute provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultModelAssemblyAttributeProvider : IModelAssemblyAttributeProvider
    {
        /// <summary>
        /// Gets the model assembly attributes for the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The model assembly attributes.
        /// </returns>
        public IEnumerable<ModelAssemblyAttribute> GetModelAssemblyAttributes(Assembly assembly)
        {
            return assembly.GetCustomAttributes<ModelAssemblyAttribute>();
        }
    }
}