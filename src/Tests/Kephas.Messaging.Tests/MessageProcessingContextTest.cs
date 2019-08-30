// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message processing context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System.Security.Principal;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class MessageProcessingContextTest
    {
        [Test]
        public void MessageProcessingContext_parent_context()
        {
            var parentContext = Substitute.For<IMessageProcessingContext>();
            parentContext.MessageProcessor.Returns(Substitute.For<IMessageProcessor>());
            parentContext.Identity.Returns(Substitute.For<IIdentity>());

            var context = new MessageProcessingContext(parentContext, parentContext.MessageProcessor);

            Assert.AreSame(parentContext.MessageProcessor, context.MessageProcessor);
            Assert.AreSame(parentContext.Identity, context.Identity);
        }

        [Test]
        public void MessageProcessingContext_parent_context_merge()
        {
            var parentContext = Substitute.For<IMessageProcessingContext>();
            parentContext.MessageProcessor.Returns(Substitute.For<IMessageProcessor>());
            parentContext.Identity.Returns(Substitute.For<IIdentity>());

            var context = new MessageProcessingContext(parentContext, parentContext.MessageProcessor);

            Assert.AreSame(parentContext.MessageProcessor, context.MessageProcessor);
            Assert.AreSame(parentContext.Identity, context.Identity);
        }
    }
}