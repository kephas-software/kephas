// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppRuntime.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Interface for abstracting away the runtime for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Application
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for abstracting away the runtime for the application.
    /// </summary>
    [ContractClass(typeof(AppRuntimeContractClass))]
    public interface IAppRuntime : IExpando
    {
        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">(Optional) A filter for the assemblies.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        Task<IEnumerable<Assembly>> GetAppAssembliesAsync(Func<AssemblyName, bool> assemblyFilter = null, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Code contracts for <see cref="IAppRuntime"/>.
    /// </summary>
    [ContractClassFor(typeof(IAppRuntime))]
    internal abstract class AppRuntimeContractClass : IAppRuntime
    {
        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Gets meta object.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The meta object.
        /// </returns>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">(Optional) A filter for the assemblies.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync(Func<AssemblyName, bool> assemblyFilter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Ensures(Contract.Result<Task<IEnumerable<Assembly>>>() != null);
            return Contract.Result<Task<IEnumerable<Assembly>>>();
        }
    }
}