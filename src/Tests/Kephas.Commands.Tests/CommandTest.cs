// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class CommandTest
    {
        [Test]
        public void Parse()
        {
            var cmd = Command.Parse("do something");

            Assert.AreEqual("do", cmd.Name);
            Assert.AreEqual(1, cmd.Args.ToDictionary().Count);
            Assert.IsTrue(cmd.Args["something"] as bool?);
        }
    }
}