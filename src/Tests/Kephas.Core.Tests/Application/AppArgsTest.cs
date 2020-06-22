// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppArgsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
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
        public void LogMinimumLevel_empty()
        {
            var args = new AppArgs("");
            Assert.IsNull(args.LogMinimumLevel);
        }

        [Test]
        public void LogMinimumLevel_parsed()
        {
            var args = new AppArgs("-loglevel debug");
            Assert.AreEqual(LogLevel.Debug, args.LogMinimumLevel);
        }
    }
}