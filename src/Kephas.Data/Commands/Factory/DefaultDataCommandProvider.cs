// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataCommandProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data command provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Factory
{
    using System;
    using System.Collections.Concurrent;

    using Kephas.Composition;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A default data command provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataCommandProvider : IDataCommandProvider
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// The command factories.
        /// </summary>
        private readonly ConcurrentDictionary<DataCommandFactoryKey, Func<IDataCommand>> commandFactories = new ConcurrentDictionary<DataCommandFactoryKey, Func<IDataCommand>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataCommandProvider"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public DefaultDataCommandProvider(ICompositionContext compositionContext)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));

            this.compositionContext = compositionContext;
        }

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
            Requires.NotNull(dataContextType, nameof(dataContextType));
            Requires.NotNull(commandType, nameof(commandType));

            var key = new DataCommandFactoryKey(dataContextType, commandType);
            var factory = this.commandFactories.GetOrAdd(key, _ => this.CreateCommandFactory(dataContextType, commandType));
            var dataCommand = factory();

            if (dataCommand == null)
            {
                throw new MissingDataCommandException(string.Format(Strings.DataCommandFactory_CreateCommand_NotSupported_Exception, commandType));
            }

            return dataCommand;
        }

        /// <summary>
        /// Creates a command factory.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// The new command factory.
        /// </returns>
        private Func<IDataCommand> CreateCommandFactory(Type dataContextType, Type commandType)
        {
            var dataCommandFactoryType = typeof(IDataCommandFactory<>).MakeGenericType(commandType);
            var dataCommandFactory = (IDataCommandFactory)this.compositionContext.TryGetExport(dataCommandFactoryType);
            if (dataCommandFactory == null)
            {
                return () => null;
            }

            return dataCommandFactory.GetCommandFactory(dataContextType);
        }

        /// <summary>
        /// A data command factory key.
        /// </summary>
        private class DataCommandFactoryKey
        {
            /// <summary>
            /// Type of the data context.
            /// </summary>
            private readonly Type dataContextType;

            /// <summary>
            /// Type of the command.
            /// </summary>
            private readonly Type commandType;

            /// <summary>
            /// Initializes a new instance of the <see cref="DataCommandFactoryKey"/> class.
            /// </summary>
            /// <param name="dataContextType">Type of the data context.</param>
            /// <param name="commandType">Type of the command.</param>
            public DataCommandFactoryKey(Type dataContextType, Type commandType)
            {
                this.dataContextType = dataContextType;
                this.commandType = commandType;
            }

            /// <summary>Serves as the default hash function. </summary>
            /// <returns>A hash code for the current object.</returns>
            public override int GetHashCode()
            {
                return this.dataContextType.GetHashCode() + (this.commandType.GetHashCode() << 3);
            }

            /// <summary>Determines whether the specified object is equal to the current object.</summary>
            /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
            /// <param name="obj">The object to compare with the current object. </param>
            public override bool Equals(object obj)
            {
                var key = obj as DataCommandFactoryKey;
                if (key == null)
                {
                    return false;
                }

                return key.commandType == this.commandType && key.dataContextType == this.dataContextType;
            }
        }
    }
}