﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A messaging model registry.
    /// </summary>
    public class MessagingModelRegistry : ConventionsRuntimeModelRegistryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingModelRegistry"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">Optional. The type loader.</param>
        /// <param name="logManager">optional. The log manager.</param>
        public MessagingModelRegistry(
            IInjectableFactory injectableFactory,
            IAppRuntime appRuntime,
            ITypeLoader? typeLoader = null,
            ILogManager? logManager = null)
            : base(
                injectableFactory,
                appRuntime,
                typeLoader,
                conventions =>
                {
                    conventions.MarkerBaseTypes = new[] { typeof(IMessageBase) };
                    conventions.MarkerAttributeTypes = new[] { typeof(MessagePartAttribute) };
                    conventions.IncludeClasses = true;
                    conventions.ExcludeMarkers = true;
                },
                logManager: logManager)
        {
        }
    }
}