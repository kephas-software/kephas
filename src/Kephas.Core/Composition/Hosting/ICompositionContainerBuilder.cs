// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContainerBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICompositionContainerBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for composition container builders.
    /// </summary>
    [ContractClass(typeof(CompositionContainerBuilderContractClass))]
    public interface ICompositionContainerBuilder
    {
        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        ICompositionContext CreateContainer();

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        Task<ICompositionContext> CreateContainerAsync();
    }

    /// <summary>
    /// Contract class for <see cref="ICompositionContainerBuilder"/>
    /// </summary>
    [ContractClassFor(typeof(ICompositionContainerBuilder))]
    internal abstract class CompositionContainerBuilderContractClass : ICompositionContainerBuilder
    {
        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public ICompositionContext CreateContainer()
        {
            Contract.Ensures(Contract.Result<ICompositionContext>() != null);

            return Contract.Result<ICompositionContext>();
        }

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public Task<ICompositionContext> CreateContainerAsync()
        {
            Contract.Ensures(Contract.Result<Task<ICompositionContext>>() != null);

            return Contract.Result<Task<ICompositionContext>>();
        }
    }
}