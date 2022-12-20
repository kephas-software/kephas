// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppArgsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Application
{
    using Kephas.Application;
    using Kephas.Logging;
    using NUnit.Framework;

    [TestFixture]
    public class AppArgsTest
    {
        [Test]
        public void AppArgs_empty()
        {
            var args = new AppArgs();
            var dictArgs = args.ToDictionary();
            Assert.Greater(dictArgs.Count, 0);
        }

        [Test]
        public void AppArgs_environment_variable()
        {
            Environment.SetEnvironmentVariable("KEPHASAPP_Hello", "John");
            var args = new AppArgs();
            Assert.AreEqual("John", args["hello"]);
        }

        [Test]
        public void LogLevel_empty()
        {
            var args = new AppArgs("");
            Assert.IsNull(args.LogLevel);
        }

        [Test]
        public void LogLevel_parsed()
        {
            var args = new AppArgs("-loglevel debug");
            Assert.AreEqual(LogLevel.Debug, args.LogLevel);
        }

        [Test]
        public void RunAsRoot_true()
        {
            var args = new AppArgs("-appid test");
            Assert.IsTrue(args.RunAsRoot);
        }

        [Test]
        public void AppId_case_insensitive()
        {
            var args = new AppArgs("-appid test");
            Assert.AreEqual("test", args.AppId);
        }

        [Test]
        public void AppInstanceId_case_insensitive()
        {
            var args = new AppArgs("-appInstanceid test-1");
            Assert.AreEqual("test-1", args.AppInstanceId);
        }

        [Test]
        public void RunAsRoot_false()
        {
            var args = new AppArgs("-root test");
            Assert.IsFalse(args.RunAsRoot);
        }

        [Test]
        public void ToDictionary_string()
        {
            var args = new AppArgs("-Hi=there -Hello=\"World\"");
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(2, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
        }

        [Test]
        public void ToDictionary_string_special_separators()
        {
            var args = new AppArgs("-Hi=there \t-Hello=\"World\" \r\n-coming=True");
            var dictArgs = args.ToDictionary();
            Assert.AreEqual(3, dictArgs.Count);
            Assert.AreEqual("there", args["Hi"]);
            Assert.AreEqual("World", args["Hello"]);
            Assert.AreEqual("True", args["coming"]);
        }
    }
}