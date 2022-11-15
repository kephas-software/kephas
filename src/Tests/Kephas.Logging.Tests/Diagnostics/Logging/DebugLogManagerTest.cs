// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the debug log manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Diagnostics.Logging
{
    using System;
    using System.Text;

    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using NUnit.Framework;

    [TestFixture]
    public class DebugLogManagerTest
    {
        [Test]
        public void Log_with_callback()
        {
            var sb = new StringBuilder();
            var logManager = new DebugLogManager(
                (name, level, message, args, exception) => sb.AppendFormat(
                    "{0}: {1}: {2} ({3}) {4}",
                    name,
                    level,
                    message,
                    string.Join(", ", args),
                    exception));
            var logger = logManager.GetLogger("logger-name");
            logger.Fatal(new ArgumentException("arg-1"), "Bad arg 1");

            var log = sb.ToString();
            Assert.AreEqual("logger-name: Fatal: Bad arg 1 () System.ArgumentException: arg-1", log);
        }

        [Test]
        public void Log_with_callback_and_args()
        {
            var sb = new StringBuilder();
            var logManager = new DebugLogManager(
                (name, level, message, args, exception) => sb.AppendFormat(
                    "{0}: {1}: {2} ({3}) {4}",
                    name,
                    level,
                    message,
                    string.Join(", ", args),
                    exception));
            var logger = logManager.GetLogger("logger-name");
            logger.Fatal(new ArgumentException("arg-1"), "Bad arg 1", "a1", 2);

            var log = sb.ToString();
            Assert.AreEqual("logger-name: Fatal: Bad arg 1 (a1, 2) System.ArgumentException: arg-1", log);
        }

        [Test]
        public void Log_with_stringbuilder()
        {
            var sb = new StringBuilder();
            var logManager = new DebugLogManager(sb);
            var logger = logManager.GetLogger("logger-name");
            logger.Fatal(new ArgumentException("arg-1"), "Bad arg 1");

            var log = sb.ToString();
            Assert.AreEqual("[logger-name] [Fatal] Bad arg 1. System.ArgumentException: arg-1\r\n", log);
        }

        [Test]
        public void Log_with_stringbuilder_and_args()
        {
            var sb = new StringBuilder();
            var logManager = new DebugLogManager(sb);
            var logger = logManager.GetLogger("logger-name");
            logger.Fatal(new ArgumentException("arg-1"), "Bad arg 1", "a1", 2);

            var log = sb.ToString();
            Assert.AreEqual("[logger-name] [Fatal] Bad arg 1 (a1, 2). System.ArgumentException: arg-1\r\n", log);
        }
    }
}