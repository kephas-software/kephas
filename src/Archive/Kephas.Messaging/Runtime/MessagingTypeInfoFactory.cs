// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging type information factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Runtime
{
    using System;

    using Kephas.Logging;
    using Kephas.Messaging.Events;
    using Kephas.Runtime;
    using Kephas.Runtime.Factories;

    /// <summary>
    /// A messaging type information factory.
    /// </summary>
    public class MessagingTypeInfoFactory : RuntimeTypeInfoFactoryBase
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public override IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, int position = -1, ILogger? logger = null)
        {
            var type = reflectInfo;
            if (typeof(IEvent).IsAssignableFrom(type) && type.IsClass)
            {
                return new RuntimeEventInfo(registry, type, logger);
            }

            if (typeof(IMessageBase).IsAssignableFrom(type) && type.IsClass)
            {
                return new RuntimeMessageInfo(registry, type, logger);
            }

            return null;
        }
    }
}