// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommandFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    /// Interface for data command factory.
    /// </summary>
    public interface IDataCommandFactory
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

    /// <summary>
    /// Singleton application service contract for data command factory.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    [SingletonAppServiceContract(AsOpenGeneric = true)]
    public interface IDataCommandFactory<out TCommand> : IDataCommandFactory
        where TCommand : IDataCommand
    {
    }
}