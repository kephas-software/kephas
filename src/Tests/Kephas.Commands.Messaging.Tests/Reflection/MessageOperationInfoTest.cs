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

    using Kephas.Commands.Messaging.Reflection;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageOperationInfoTest
    {
        [Test]
        public void CreateMessage_exact_param()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(typeof(NullableParamMessage).AsRuntimeTypeInfo(), lazyMessageProcessor);
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
            var operationInfo = new MessageOperationInfo(typeof(NullableParamMessage).AsRuntimeTypeInfo(), lazyMessageProcessor);
            var msg = operationInfo.CreateMessage(new Expando { ["starttime"] = "2020-04-19" });

            Assert.IsInstanceOf<NullableParamMessage>(msg);
            var typedMsg = (NullableParamMessage)msg;
            Assert.AreEqual(new DateTime(2020, 04, 19), typedMsg.StartTime);
        }

        [Test]
        public void CreateMessage_positional_param()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(typeof(EnumMessage).AsRuntimeTypeInfo(), lazyMessageProcessor);
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
            var operationInfo = new MessageOperationInfo(typeof(EnumMessage).AsRuntimeTypeInfo(), lazyMessageProcessor);
            var dateTime = new DateTime(2020, 04, 19);
            var msg = operationInfo.CreateMessage(new Expando { ["logLevel"] = "error" });

            Assert.IsInstanceOf<EnumMessage>(msg);
            var typedMsg = (EnumMessage)msg;
            Assert.AreEqual(LogLevel.Error, typedMsg.LogLevel);
        }

        [Test]
        public void CreateMessage_arg_not_found()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var operationInfo = new MessageOperationInfo(typeof(EnumMessage).AsRuntimeTypeInfo(), lazyMessageProcessor);
            var dateTime = new DateTime(2020, 04, 19);
            Assert.Throws<InsufficientMemoryException>(() => operationInfo.CreateMessage(new Expando { ["nonexisting"] = true }));
        }

        public class NullableParamMessage : IMessage
        {
            public DateTime? StartTime { get; set; }
        }

        public class EnumMessage : IMessage
        {
            public LogLevel? LogLevel { get; set; }
        }
    }
}
