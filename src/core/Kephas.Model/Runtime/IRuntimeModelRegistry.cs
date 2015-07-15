// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelRegistry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Registrar application service for providing runtime elements used in building the model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Application service for providing runtime elements used in building the model space.
    /// </summary>
    [SharedAppServiceContract]
    public interface IRuntimeModelRegistry
    {
        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <returns>An enumeration of runtime elements.</returns>
        IEnumerable<object> GetRuntimeElements();
    }
}