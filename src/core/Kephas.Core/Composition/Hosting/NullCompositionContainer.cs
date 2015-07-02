// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCompositionContainer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A composition container doing nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// A composition container doing nothing.
    /// </summary>
    public class NullCompositionContainer : ICompositionContainer
    {
        /// <summary>
        /// Composes the parts without registering them for recomposition.
        /// </summary>
        /// <param name="parts">The parts to be composed.</param>
        public void SatisfyImports(params object[] parts)
        {
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        public object GetExport(Type contractType, string contractName = null)
        {
            throw new NullServiceException(this);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        public IEnumerable<object> GetExports(Type contractType, string contractName = null)
        {
            return new object[0];
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <paramref name="{T}" />.
        /// </returns>
        public T GetExport<T>(string contractName = null)
        {
            throw new NullServiceException(this);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="{T}" />.
        /// </returns>
        public IEnumerable<T> GetExports<T>(string contractName = null)
        {
            return new T[0];
        }
    }
}