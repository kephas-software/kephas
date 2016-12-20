// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IInitializable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides the <see cref="Initialize"/> method for service initialization.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void Initialize(IContext context = null);
    }
}