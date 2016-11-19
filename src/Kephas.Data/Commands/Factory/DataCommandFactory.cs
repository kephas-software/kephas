// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data command factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Data.Commands.Composition;
    using Kephas.Data.Resources;

    /// <summary>
    /// A data command factory.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public class DataCommandFactory<TCommand> : IDataCommandFactory<TCommand>
        where TCommand : IDataCommand
    {
        private readonly ICollection<IExportFactory<TCommand, DataCommandMetadata>> commandFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandFactory{TCommand}"/> class.
        /// </summary>
        /// <param name="commandFactories">The command factories.</param>
        public DataCommandFactory(ICollection<IExportFactory<TCommand, DataCommandMetadata>> commandFactories)
        {
            Contract.Requires(commandFactories != null);

            this.commandFactories = commandFactories.OrderBy(f => f.Metadata.OverridePriority).ToList();
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        public TCommand CreateCommand(Type dataContextType)
        {
            var commandFactory = this.commandFactories.FirstOrDefault(f => f.Metadata.DataContextType == dataContextType);
            if (commandFactory == null)
            {
                throw new NotSupportedException(string.Format(Strings.DataCommandFactory_CreateCommand_NotSupported_Exception, typeof(TCommand)));
            }

            return commandFactory.CreateExport().Value;
        }
    }
}