// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Commands
{
    using Kephas.Commands;
    using NUnit.Framework;

    [TestFixture]
    public class CommandInfoTest
    {
        [Test]
        public void Parse()
        {
            var cmd = CommandInfo.Parse("do something");

            Assert.AreEqual("do", cmd.Name);
            Assert.AreEqual(1, cmd.Args.ToDictionary().Count);
            Assert.IsTrue(cmd.Args["something"] as bool?);
        }
    }
}