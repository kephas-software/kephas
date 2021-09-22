// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerInjectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message handler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Messaging.AttributedModel;
    using Kephas.Messaging.Composition;
    using Kephas.Testing.Composition;

    using NUnit.Framework;

    [TestFixture]
    public class MessageHandlerInjectionTest : SystemCompositionInjectionTestBase
    {
        [Test]
        public void Injection_single_handler()
        {
            var container = this.CreateContainer(parts: new[]
                                                            {
                                                                typeof(IMessageHandler),
                                                                typeof(IMessageHandler<>),
                                                                typeof(HiTestHandler)
                                                            });

            var handlers = container.ResolveMany<IMessageHandler>().ToList();

            Assert.AreEqual(1, handlers.Count);
            Assert.IsInstanceOf<HiTestHandler>(handlers[0]);

            var handlerFactories = container.GetExportFactories<IMessageHandler, MessageHandlerMetadata>()
                .OrderBy(f => f.Metadata.MessageId)
                .ToList();
            Assert.AreEqual(1, handlerFactories.Count);
            Assert.AreEqual("Hi", handlerFactories[0].Metadata.MessageId);
        }

        [Test]
        public void Injection_two_handlers()
        {
            var container = this.CreateContainer(parts: new[]
                                                            {
                                                                typeof(IMessageHandler),
                                                                typeof(IMessageHandler<>),
                                                                typeof(HiTestHandler),
                                                                typeof(ThereTestHandler)
                                                            });

            var handlers = container.ResolveMany<IMessageHandler>().ToList();

            Assert.AreEqual(2, handlers.Count);
            Assert.IsInstanceOf<HiTestHandler>(handlers[0]);

            var handlerFactories = container.GetExportFactories<IMessageHandler, MessageHandlerMetadata>()
                .OrderBy(f => f.Metadata.MessageId)
                .ToList();
            Assert.AreEqual(2, handlerFactories.Count);
            Assert.AreEqual("Hi", handlerFactories[0].Metadata.MessageId);
            Assert.AreEqual("There", handlerFactories[1].Metadata.MessageId);
        }

        [MessageHandler("Hi")]
        public class HiTestHandler : MessageHandlerBase<IMessage, IMessage>
        {
            public override async Task<IMessage> ProcessAsync(IMessage message, IMessagingContext context, CancellationToken token)
            {
                throw new NotImplementedException();
            }
        }

        [MessageHandler("There")]
        public class ThereTestHandler : MessageHandlerBase<IMessage, IMessage>
        {
            public override async Task<IMessage> ProcessAsync(IMessage message, IMessagingContext context, CancellationToken token)
            {
                throw new NotImplementedException();
            }
        }
    }
}