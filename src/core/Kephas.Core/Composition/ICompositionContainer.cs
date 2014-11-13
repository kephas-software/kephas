// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContainer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Public interface for the composition container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Public interface for the composition container.
    /// </summary>
    public interface ICompositionContainer
    {
        /// <summary>
        /// Composes the parts without registering them for recomposition.
        /// </summary>
        /// <param name="parts">The parts to be composed.</param>
        void SatisfyImports(params object[] parts);

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        object GetExport(Type contractType, string contractName = null);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        IEnumerable<object> GetExports(Type contractType, string contractName = null);

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <paramref name="{T}" />.
        /// </returns>
        T GetExport<T>(string contractName = null);

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="{T}" />.
        /// </returns>
        IEnumerable<T> GetExports<T>(string contractName = null);
    }
}