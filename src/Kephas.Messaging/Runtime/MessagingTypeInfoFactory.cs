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

    using Kephas.Messaging.Events;
    using Kephas.Runtime;

    /// <summary>
    /// A messaging type information factory.
    /// </summary>
    public class MessagingTypeInfoFactory : IRuntimeTypeInfoFactory
    {
        /// <summary>
        /// Tries to create the runtime type information type for the provided raw type.
        /// </summary>
        /// <param name="type">The raw type.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public IRuntimeTypeInfo TryCreateRuntimeTypeInfo(Type type)
        {
            if (typeof(IEvent).IsAssignableFrom(type))
            {
                return new RuntimeEventInfo(type);
            }

            if (typeof(IMessage).IsAssignableFrom(type))
            {
                return new RuntimeMessageInfo(type);
            }

            return null;
        }
    }
}