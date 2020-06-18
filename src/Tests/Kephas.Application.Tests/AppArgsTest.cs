// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppArgsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
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
    }
}