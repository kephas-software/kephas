// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommandFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataCommandFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Factory
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Service contract for data command factory.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    [SharedAppServiceContract(AsOpenGeneric = true)]
    public interface IDataCommandFactory<out TCommand>
        where TCommand : IDataCommand
    {
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="dataRepositoryType">Type of the data repository.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        TCommand CreateCommand(Type dataRepositoryType);
    }
}