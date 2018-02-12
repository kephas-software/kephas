// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailSenderServiceExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email sender service extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Tests.Services
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Mail.Services;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class EmailSenderServiceExtensionsTest
    {
        [Test]
        public async Task SendAsync_single_address()
        {
            IEmailMessage message = null;
            var sender = Substitute.For<IEmailSenderService>();
            sender.SendAsync(Arg.Any<IEmailMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        message = ci.Arg<IEmailMessage>();
                        return Task.FromResult(0);
                    });

            await sender.SendAsync("me@me.com", "subject", "body").PreserveThreadContext();

            Assert.AreEqual("me@me.com", message.ToRecipients.Single().Address);
            Assert.AreEqual("subject", message.Subject);
            Assert.AreEqual("body", message.Body.Content);
        }

        [Test]
        public async Task SendAsync_multiple_addresses()
        {
            IEmailMessage message = null;
            var sender = Substitute.For<IEmailSenderService>();
            sender.SendAsync(Arg.Any<IEmailMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        message = ci.Arg<IEmailMessage>();
                        return Task.FromResult(0);
                    });

            await sender.SendAsync(new [] { "me@me.com", "you@you.com" }, "subject", "body").PreserveThreadContext();

            Assert.AreEqual("me@me.com", message.ToRecipients.First().Address);
            Assert.AreEqual("you@you.com", message.ToRecipients.Skip(1).Single().Address);
            Assert.AreEqual("subject", message.Subject);
            Assert.AreEqual("body", message.Body.Content);
        }
    }
}