// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message handler registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Concurrent;
using Kephas.Services;

namespace Kephas.Messaging
{
    /// <summary>
    /// A default message handler registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageHandlerRegistry : IMessageHandlerRegistry
    {
        private readonly ConcurrentBag<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerRegistry = new();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IExportFactory<IMessageHandler, MessageHandlerMetadata>> GetEnumerator() => handlerRegistry.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => handlerRegistry.GetEnumerator();

        /// <summary>
        /// Registers the message handler factory.
        /// </summary>
        /// <param name="handlerFactory">The handler factory.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        public IMessageHandlerRegistry RegisterHandler(IExportFactory<IMessageHandler, MessageHandlerMetadata> handlerFactory)
        {
            handlerRegistry.Add(handlerFactory);
            return this;
        }
    }
}
