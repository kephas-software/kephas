// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeLoader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ITypeLoader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Application service contract for loading types.
    /// </summary>
    [ContractClass(typeof(TypeLoaderContractClass))]
    public interface ITypeLoader
    {
        /// <summary>
        /// Gets the loadable exported types from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the types.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        IEnumerable<Type> GetLoadableExportedTypes(Assembly assembly);
    }

    /// <summary>
    /// Contract class fpr <see cref="ITypeLoader"/>.
    /// </summary>
    [ContractClassFor(typeof(ITypeLoader))]
    internal abstract class TypeLoaderContractClass : ITypeLoader
    {
        /// <summary>
        /// Gets the loadable exported types from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the types.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        public IEnumerable<Type> GetLoadableExportedTypes(Assembly assembly)
        {
            Contract.Requires(assembly != null);
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);

            return Contract.Result<IEnumerable<Type>>();
        }
    }
}