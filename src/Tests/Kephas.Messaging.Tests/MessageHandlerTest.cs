// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message handler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Messaging.Composition;
    using Kephas.Testing.Composition.Mef;

    using NUnit.Framework;

    [TestFixture]
    public class MessageHandlerTest : CompositionTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer(parts: new[]
                                                            {
                                                                typeof(IMessageHandler<>),
                                                                typeof(HiTestHandler)
                                                            });

            var handlers = container.GetExports<IMessageHandler<IMessage>>().ToList();

            Assert.AreEqual(1, handlers.Count);
            Assert.IsInstanceOf<HiTestHandler>(handlers[0]);

            var handlerFactories = container.GetExportFactories<IMessageHandler<IMessage>, MessageHandlerMetadata>().ToList();
            Assert.AreEqual(1, handlerFactories.Count);
            Assert.AreEqual("Hi", handlerFactories[0].Metadata.MessageName);
        }

        [MessageName("Hi")]
        public class HiTestHandler : MessageHandlerBase<IMessage, IMessage>
        {
            public override async Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context, CancellationToken token)
            {
                throw new NotImplementedException();
            }
        }
    }
}