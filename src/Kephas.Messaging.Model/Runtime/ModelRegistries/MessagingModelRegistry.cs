// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingModelRegistry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the messaging model registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    using IMessage = Kephas.Messaging.IMessage;

    /// <summary>
    /// A messaging model registry.
    /// </summary>
    public class MessagingModelRegistry : IRuntimeModelRegistry
    {
        /// <summary>
        /// The application runtime.
        /// </summary>
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// The type loader.
        /// </summary>
        private readonly ITypeLoader typeLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingModelRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public MessagingModelRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
        }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public async Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var assemblies = await this.appRuntime.GetAppAssembliesAsync(cancellationToken: cancellationToken).PreserveThreadContext();

            var types = new HashSet<Type>();
            var markerInterface = typeof(IMessage).GetTypeInfo();
            foreach (var assembly in assemblies)
            {
                types.AddRange(this.typeLoader.GetLoadableExportedTypes(assembly).Where(
                    t =>
                        {
                            var ti = t.GetTypeInfo();
                            return this.IsMessage(ti, markerInterface) || this.IsMessagePart(ti);
                        }));
            }

            return types;
        }

        /// <summary>
        /// Query if 'type' is message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="markerInterface">The marker interface.</param>
        /// <returns>
        /// True if the type is a message, false if not.
        /// </returns>
        private bool IsMessage(TypeInfo type, TypeInfo markerInterface)
        {
            return type.IsClass && !type.IsAbstract && markerInterface.IsAssignableFrom(type);
        }

        /// <summary>
        /// Query if 'type' is a message part.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// True if the type is a message part, false if not.
        /// </returns>
        private bool IsMessagePart(TypeInfo type)
        {
            return type.IsClass && !type.IsAbstract && type.GetCustomAttribute<MessagePartAttribute>() != null;
        }
    }
}