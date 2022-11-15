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
    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Services;

    using IMessage = Kephas.Messaging.IMessage;

    /// <summary>
    /// A messaging model registry.
    /// </summary>
    public class MessagingModelRegistry : ConventionsRuntimeModelRegistryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingModelRegistry"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="typeLoader">Optional. The type loader.</param>
        /// <param name="logManager">optional. The log manager.</param>
        public MessagingModelRegistry(
            IInjectableFactory injectableFactory,
            IAmbientServices ambientServices,
            ITypeLoader? typeLoader = null,
            ILogManager? logManager = null)
            : base(
                injectableFactory,
                ambientServices,
                typeLoader,
                conventions =>
                {
                    conventions.MarkerBaseTypes = new[] { typeof(IMessage) };
                    conventions.MarkerAttributeTypes = new[] { typeof(MessagePartAttribute) };
                    conventions.IncludeClasses = true;
                    conventions.ExcludeMarkers = true;
                },
                logManager: logManager)
        {
        }
    }
}