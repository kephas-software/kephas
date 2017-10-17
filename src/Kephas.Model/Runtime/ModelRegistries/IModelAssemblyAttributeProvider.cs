// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelAssemblyAttributeProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IModelAssemblyAttributeProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.ModelRegistries
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Interface for model assembly attribute provider.
    /// </summary>
    [SharedAppServiceContract]
    public interface IModelAssemblyAttributeProvider
    {
        /// <summary>
        /// Gets the model assembly attributes for the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The model assembly attributes.
        /// </returns>
        IEnumerable<ModelAssemblyAttribute> GetModelAssemblyAttributes(Assembly assembly);
    }
}