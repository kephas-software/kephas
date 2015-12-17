// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides platform specific functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting
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
    /// Provides platform specific functionality.
    /// </summary>
    [ContractClass(typeof(HostingEnvironmentContractClass))]
    public interface IHostingEnvironment : IExpando
    {
        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        Task<IEnumerable<Assembly>> GetAppAssembliesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Code contracts for <see cref="IHostingEnvironment"/>.
    /// </summary>
    [ContractClassFor(typeof(IHostingEnvironment))]
    internal abstract class HostingEnvironmentContractClass : IHostingEnvironment
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Ensures(Contract.Result<Task<IEnumerable<Assembly>>>() != null);
            return Contract.Result<Task<IEnumerable<Assembly>>>();
        }
    }
}