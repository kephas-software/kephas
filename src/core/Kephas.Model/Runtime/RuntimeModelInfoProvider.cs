// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Model provider based on the .NET runtime and the type system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.Contracts;

    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Factory;

    /// <summary>
    /// Model provider based on the .NET runtime and the type system.
    /// </summary>
    public class RuntimeModelInfoProvider : IModelInfoProvider
    {
        /// <summary>
        /// The model registrars.
        /// </summary>
        private readonly ICollection<IRuntimeModelRegistrar> modelRegistrars;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelInfoProvider"/> class.
        /// </summary>
        /// <param name="modelRegistrars">The model registrars.</param>
        public RuntimeModelInfoProvider([ImportMany] ICollection<IRuntimeModelRegistrar> modelRegistrars)
        {
            Contract.Requires(modelRegistrars != null);

            this.modelRegistrars = modelRegistrars;
        }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <returns>
        /// An enumeration of element information.
        /// </returns>
        public IEnumerable<INamedElementInfo> GetElementInfos()
        {
            var runtimeObjects = new List<object>();
            foreach (var registrar in this.modelRegistrars)
            {
                runtimeObjects.AddRange(registrar.GetRuntimeElements());
            }
        }
    }
}