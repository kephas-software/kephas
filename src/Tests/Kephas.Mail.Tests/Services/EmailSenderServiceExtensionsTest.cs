// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailSenderServiceExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Mail.Tests.Message;
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
            var builder = new MailMessageBuilder();

            var sender = Substitute.For<IEmailSenderService>();
            sender.CreateEmailMessageBuilder(Arg.Any<IContext>()).Returns(builder);
            sender.SendAsync(Arg.Any<IEmailMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(0));

            await sender.SendAsync("me@me.com", "subject", "body").PreserveThreadContext();

            var message = builder.EmailMessage;
            Assert.AreEqual("me@me.com", message.To.Single().Address);
            Assert.AreEqual("subject", message.Subject);
            Assert.AreEqual("body", message.BodyHtml);
        }

        [Test]
        public async Task SendAsync_multiple_addresses()
        {
            var builder = new MailMessageBuilder();

            var sender = Substitute.For<IEmailSenderService>();
            sender.CreateEmailMessageBuilder(Arg.Any<IContext>()).Returns(builder);
            sender.SendAsync(Arg.Any<IEmailMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(0));

            await sender.SendAsync(new [] { "me@me.com", "you@you.com" }, "subject", "body").PreserveThreadContext();

            var message = builder.EmailMessage;
            Assert.AreEqual("me@me.com", message.To.First().Address);
            Assert.AreEqual("you@you.com", message.To.Skip(1).Single().Address);
            Assert.AreEqual("subject", message.Subject);
            Assert.AreEqual("body", message.BodyHtml);
        }
    }
}