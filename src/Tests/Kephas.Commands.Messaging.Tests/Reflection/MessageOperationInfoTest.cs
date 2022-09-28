// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageOperationInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message operation information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Tests.Reflection
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Commands.Messaging.Reflection;
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageOperationInfoTest
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        public MessageOperationInfoTest()
        {
            this.typeRegistry = new RuntimeTypeRegistry();
        }

        [Test]
        public void CreateMessage_exact_param()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(this.typeRegistry, this.typeRegistry.GetTypeInfo(typeof(NullableParamMessage)), lazyMessageProcessor);
            var dateTime = new DateTime(2020, 04, 19);
            var msg = operationInfo.CreateMessage(new Expando { ["starttime"] = dateTime });

            Assert.IsInstanceOf<NullableParamMessage>(msg);
            var typedMsg = (NullableParamMessage)msg;
            Assert.AreEqual(dateTime, typedMsg.StartTime);
        }

        [Test]
        public void CreateMessage_parse_date()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(this.typeRegistry, this.typeRegistry.GetTypeInfo(typeof(NullableParamMessage)), lazyMessageProcessor);
            var msg = operationInfo.CreateMessage(new Expando { ["starttime"] = "2020-04-19" });

            Assert.IsInstanceOf<NullableParamMessage>(msg);
            var typedMsg = (NullableParamMessage)msg;
            Assert.AreEqual(new DateTime(2020, 04, 19), typedMsg.StartTime);
        }

        [Test]
        public void CreateMessage_positional_param()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(this.typeRegistry, this.typeRegistry.GetTypeInfo(typeof(EnumMessage)), lazyMessageProcessor);
            var dateTime = new DateTime(2020, 04, 19);
            var msg = operationInfo.CreateMessage(new Expando { ["fatal"] = true });

            Assert.IsInstanceOf<EnumMessage>(msg);
            var typedMsg = (EnumMessage)msg;
            Assert.AreEqual(LogLevel.Fatal, typedMsg.LogLevel);
        }

        [Test]
        public void CreateMessage_parse_enum()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(this.typeRegistry, this.typeRegistry.GetTypeInfo(typeof(EnumMessage)), lazyMessageProcessor);
            var msg = operationInfo.CreateMessage(new Expando { ["logLevel"] = "error" });

            Assert.IsInstanceOf<EnumMessage>(msg);
            var typedMsg = (EnumMessage)msg;
            Assert.AreEqual(LogLevel.Error, typedMsg.LogLevel);
        }

        [Test]
        public void CreateMessage_arg_not_found()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(this.typeRegistry, this.typeRegistry.GetTypeInfo(typeof(EnumMessage)), lazyMessageProcessor);
            var dateTime = new DateTime(2020, 04, 19);
            Assert.Throws<ArgumentException>(() => operationInfo.CreateMessage(new Expando { ["nonexisting"] = true }));
        }

        [Test]
        public void CreateMessage_use_short_name()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(this.typeRegistry, this.typeRegistry.GetTypeInfo(typeof(UpdateMessage)), lazyMessageProcessor);
            var msg = operationInfo.CreateMessage(new Expando { ["Pre"] = true });

            Assert.IsInstanceOf<UpdateMessage>(msg);
            var typedMsg = (UpdateMessage)msg;
            Assert.IsTrue(typedMsg.IncludePrerelease);
        }

        [Test]
        public async Task InvokeAsync_success()
        {
            var messageProcessor = Substitute.For<IMessageProcessor>();
            messageProcessor.ProcessAsync(Arg.Any<NullableParamMessage>(), Arg.Any<Action<IMessagingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => new ResponseMessage { Message = $"Start time: {ci.Arg<NullableParamMessage>().StartTime:s}" });
            var operationInfo = new MessageOperationInfo(
                    this.typeRegistry,
                    this.typeRegistry.GetTypeInfo(typeof(NullableParamMessage)),
                    new Lazy<IMessageProcessor>(() => messageProcessor));

            var result = await operationInfo.InvokeAsync(null, new object?[] { new Expando { ["starttime"] = "2020-04-19" } });

            Assert.IsInstanceOf<ResponseMessage>(result);

            var response = (ResponseMessage)result;
            Assert.AreEqual("Start time: 2020-04-19T00:00:00", response.Message);
        }

        [Test]
        public async Task InvokeAsync_missing_handler()
        {
            var messageProcessor = Substitute.For<IMessageProcessor>();
            messageProcessor.ProcessAsync(Arg.Any<NullableParamMessage>(), Arg.Any<Action<IMessagingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => (IMessage)null ?? throw new MissingHandlerException("bad"));
            var operationInfo = new MessageOperationInfo(
                    this.typeRegistry,
                    this.typeRegistry.GetTypeInfo(typeof(NullableParamMessage)),
                    new Lazy<IMessageProcessor>(() => messageProcessor));

            Assert.ThrowsAsync<MissingHandlerException>(() => operationInfo.InvokeAsync(null, new object?[] { new Expando() }));
        }

        public class NullableParamMessage : IMessage
        {
            public DateTime? StartTime { get; set; }
        }

        public class EnumMessage : IMessage
        {
            public LogLevel? LogLevel { get; set; }
        }

        public class UpdateMessage : IMessage
        {
            [Display(ShortName = "pre", Description = "Includes prerelease.")]
            public bool IncludePrerelease { get; set; }
        }
    }
}
