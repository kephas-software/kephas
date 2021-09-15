// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message processing context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Messaging.Tests
{
    using System.Security.Principal;
    using Kephas.Composition;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class MessagingContextTest
    {
        [Test]
        public void MessagingContext_parent_context()
        {
            var parentContext =
                new MessagingContext(Substitute.For<IInjector>(), Substitute.For<IMessageProcessor>(), Substitute.For<IMessage>())
                    {
                        Identity = Substitute.For<IIdentity>()
                    };

            var context = new MessagingContext(parentContext, parentContext.MessageProcessor);

            Assert.AreSame(parentContext.MessageProcessor, context.MessageProcessor);
            Assert.AreSame(parentContext.Identity, context.Identity);
        }

        [Test]
        public void MessagingContext_parent_context_merge()
        {
            var parentContext =
                new MessagingContext(Substitute.For<IInjector>(), Substitute.For<IMessageProcessor>(), Substitute.For<IMessage>())
                    {
                        Identity = Substitute.For<IIdentity>(),
                        ["hi"] = "there",
                    };

            var context = new MessagingContext(parentContext, parentContext.MessageProcessor);

            Assert.AreSame(parentContext.MessageProcessor, context.MessageProcessor);
            Assert.AreSame(parentContext.Identity, context.Identity);
            Assert.AreSame("there", context["hi"]);
        }
    }
}