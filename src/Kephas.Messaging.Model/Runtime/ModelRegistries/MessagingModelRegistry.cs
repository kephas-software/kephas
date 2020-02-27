// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Model.Reflection;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;

    using IMessage = Kephas.Messaging.IMessage;

    /// <summary>
    /// A messaging model registry.
    /// </summary>
    public class MessagingModelRegistry : IRuntimeModelRegistry
    {
        private readonly IAppRuntime appRuntime;
        private readonly IAssemblyLoader assemblyLoader;
        private readonly TypeInfo markerInterface = typeof(IMessage).GetTypeInfo();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingModelRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="assemblyLoader">The type loader.</param>
        public MessagingModelRegistry(IAppRuntime appRuntime, IAssemblyLoader assemblyLoader)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.appRuntime = appRuntime;
            this.assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            var assemblies = this.appRuntime.GetAppAssemblies();

            var types = new HashSet<Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(this.assemblyLoader.GetExportedTypes(assembly).Where(
                    t =>
                        {
                            var ti = t.GetTypeInfo();
                            return (this.IsMessage(ti) || this.IsMessagePart(ti)) && !ti.IsExcludedFromModel();
                        }));
            }

            return Task.FromResult<IEnumerable<object>>(types);
        }

        /// <summary>
        /// Query if 'type' is message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// True if the type is a message, false if not.
        /// </returns>
        private bool IsMessage(TypeInfo type)
        {
            return type.IsClass && !type.IsAbstract && this.markerInterface.IsAssignableFrom(type);
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