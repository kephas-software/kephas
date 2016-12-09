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

    using Kephas.Composition;
    using Kephas.Data.Commands.Composition;
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
        /// Gets the command factory for the given data context type.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <returns>
        ///  The command factory for the indicated command.
        /// </returns>
        Func<IDataCommand> GetCommandFactory(Type dataContextType);
    }
}