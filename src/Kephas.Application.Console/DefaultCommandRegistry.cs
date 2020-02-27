// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Application.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas;
    using Kephas.Application.Console.Resources;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A default command registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultCommandRegistry : ICommandRegistry
    {
        private readonly IAppRuntime appRuntime;
        private readonly IAssemblyLoader assemblyLoader;
        private IList<ITypeInfo>? commandTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultCommandRegistry(IAppRuntime appRuntime, IAssemblyLoader assemblyLoader)
        {
            this.appRuntime = appRuntime;
            this.assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Gets the command types.
        /// </summary>
        /// <param name="commandPattern">Optional. A pattern specifying the command types to retrieve.</param>
        /// <returns>
        /// The command types.
        /// </returns>
        public IEnumerable<ITypeInfo> GetCommandTypes(string? commandPattern = null)
        {
            this.commandTypes ??= this.appRuntime.GetAppAssemblies()
                                            .SelectMany(a => this.assemblyLoader.GetExportedTypes(a).Where(this.IsMessageType))
                                            .Select(t => (ITypeInfo)t.AsRuntimeTypeInfo())
                                            .ToList()
                                            .AsReadOnly();

            if (string.IsNullOrEmpty(commandPattern))
            {
                return this.commandTypes;
            }

            return this.commandTypes.Where(c => c.Name.StartsWith(commandPattern, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Resolves the command type.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        public ITypeInfo ResolveCommandType(string command)
        {
            var matchingCommandTypes = this.GetCommandTypes(command).ToList();
            var commandType = (matchingCommandTypes.SingleOrDefault(m => m.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase))
                               ?? matchingCommandTypes.SingleOrDefault(m => m.Name.Equals(command + "Message", StringComparison.InvariantCultureIgnoreCase)))
                              ?? matchingCommandTypes.SingleOrDefault(m => m.Name.Equals(command + "Event", StringComparison.InvariantCultureIgnoreCase));

            if (commandType == null)
            {
                if (matchingCommandTypes.Count == 0)
                {
                    throw new KeyNotFoundException(Strings.DefaultCommandRegistry_CommandNotFound.FormatWith(command));
                }

                if (matchingCommandTypes.Count > 1)
                {
                    throw new AmbiguousMatchException(Strings.DefaultCommandRegistry_AmbiguousCommandName.FormatWith(command, string.Join("', '", matchingCommandTypes.Select(m => m.Name))));
                }

                commandType = matchingCommandTypes[0];
            }

            return commandType;
        }

        private bool IsMessageType(Type type) => !type.IsAbstract
                                                    && typeof(IMessage).IsAssignableFrom(type)
                                                    && !typeof(IResponse).IsAssignableFrom(type)
                                                    && !type.Name.EndsWith("ResponseMessage")
                                                    && !typeof(IMessageEnvelope).IsAssignableFrom(type)
                                                    && type.GetCustomAttribute<ExcludeFromModelAttribute>() == null;
    }
}
