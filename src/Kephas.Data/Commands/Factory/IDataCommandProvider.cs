// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommandProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataCommandProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Factory
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract creating commands for specific data contexts.
    /// </summary>
    [SharedAppServiceContract]
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
}