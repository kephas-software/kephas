// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Commands.Messaging.Reflection;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A default command registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MessagingCommandRegistry : ICommandRegistry
    {
        private readonly IAppRuntime appRuntime;
        private readonly ITypeLoader typeLoader;
        private readonly Lazy<IMessageProcessor> lazyMessageProcessor;
        private IList<IOperationInfo>? commandTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommandRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        /// <param name="lazyMessageProcessor">The lazy message processor.</param>
        public MessagingCommandRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader, Lazy<IMessageProcessor> lazyMessageProcessor)
        {
            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
            this.lazyMessageProcessor = lazyMessageProcessor;
        }

        /// <summary>
        /// Gets the command types.
        /// </summary>
        /// <param name="commandPattern">Optional. A pattern specifying the command types to retrieve.</param>
        /// <returns>
        /// The command types.
        /// </returns>
        public IEnumerable<IOperationInfo> GetCommandTypes(string? commandPattern = null)
        {
            this.commandTypes ??= this.appRuntime.GetAppAssemblies()
                                            .SelectMany(a => this.typeLoader.GetExportedTypes(a).Where(this.IsMessageType))
                                            .Select(t => (IOperationInfo)new MessageOperationInfo(t.AsRuntimeTypeInfo(), this.lazyMessageProcessor))
                                            .ToList()
                                            .AsReadOnly();

            return string.IsNullOrEmpty(commandPattern)
                ? this.commandTypes
                : this.commandTypes.Where(c => c.Name.StartsWith(commandPattern, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool IsMessageType(Type type) => !type.IsAbstract
                                                    && typeof(IMessage).IsAssignableFrom(type)
                                                    && !typeof(IResponse).IsAssignableFrom(type)
                                                    && !type.Name.EndsWith("ResponseMessage")
                                                    && !typeof(IMessageEnvelope).IsAssignableFrom(type)
                                                    && type.GetCustomAttribute<ExcludeFromModelAttribute>() == null;
    }
}
