// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data command factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.Commands.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Data.Resources;
    using Kephas.Services;

    /// <summary>
    /// A data command factory.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public class DataCommandFactory<TCommand> : IDataCommandFactory<TCommand>
        where TCommand : IDataCommand
    {
        /// <summary>
        /// The command factories.
        /// </summary>
        private readonly IFactoryEnumerable<TCommand, DataCommandMetadata> commandFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandFactory{TCommand}"/> class.
        /// </summary>
        /// <param name="commandFactories">The command factories.</param>
        public DataCommandFactory(ICollection<IExportFactory<TCommand, DataCommandMetadata>> commandFactories)
        {
            commandFactories = commandFactories ?? throw new System.ArgumentNullException(nameof(commandFactories));

            this.commandFactories = commandFactories.Order();
        }

        /// <summary>
        /// Gets the command factory for the given data context type.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <returns>
        ///  The command factory for the indicated command.
        /// </returns>
        public Func<IDataCommand> GetCommandFactory(Type dataContextType)
        {
            var commandFactoriesList = this.commandFactories
                                        .Where(f => f.Metadata.DataContextType == dataContextType)
                                        .ToList();
            if (commandFactoriesList.Count > 1)
            {
                if (commandFactoriesList[0].Metadata.OverridePriority == commandFactoriesList[1].Metadata.OverridePriority)
                {
                    throw new AmbiguousMatchDataException(string.Format(
                        Strings.DataCommandFactory_GetCommandFactory_AmbiguousMatch_Exception,
                        typeof(TCommand).FullName,
                        dataContextType.FullName,
                        commandFactoriesList[0].Metadata.ServiceType?.FullName,
                        commandFactoriesList[1].Metadata.ServiceType?.FullName));
                }
            }

            var commandFactory = commandFactoriesList.Count == 0 ? null : commandFactoriesList[0];
            if (commandFactory == null)
            {
                var dataContextTypeInfo = dataContextType.GetTypeInfo();
                commandFactory = this.commandFactories.SingleOrDefault(f => f.Metadata.DataContextType.GetTypeInfo().IsAssignableFrom(dataContextTypeInfo));
                if (commandFactory == null)
                {
                    return () => null;
                }
            }

            return () => commandFactory.CreateExportedValue();
        }
    }
}