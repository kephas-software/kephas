// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommandProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataCommandProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Factory
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract creating commands for specific data contexts.
    /// </summary>
    [SharedAppServiceContract]
    [ContractClass(typeof(DataCommandProviderContractClass))]
    public interface IDataCommandProvider
    {
        /// <summary>
        /// Creates a command for the provided data context type.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        IDataCommand CreateCommand(Type dataContextType, Type commandType);
    }

    /// <summary>
    /// Contract class for <see cref="IDataCommandProvider"/>.
    /// </summary>
    [ContractClassFor(typeof(IDataCommandProvider))]
    internal abstract class DataCommandProviderContractClass : IDataCommandProvider
    {
        /// <summary>
        /// Creates a command for the provided data context type.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        public IDataCommand CreateCommand(Type dataContextType, Type commandType)
        {
            Contract.Requires(dataContextType != null);
            Contract.Requires(commandType != null);
            Contract.Ensures(Contract.Result<IDataCommand>() != null);

            return Contract.Result<IDataCommand>();
        }
    }
}