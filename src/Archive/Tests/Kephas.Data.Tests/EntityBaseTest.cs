// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using Kephas.Data.Capabilities;
    using NUnit.Framework;

    [TestFixture]
    public class EntityBaseTest
    {
        [Test]
        public void This_property_set_will_set_the_state()
        {
            var entity = new MyEntity();
            entity.Name = "gigi";

            Assert.AreEqual("gigi", entity.Name);
            Assert.AreEqual(ChangeState.Changed, (entity as IChangeStateTrackable).ChangeState);
        }

        public class MyEntity : EntityBase
        {
            public string Name
            {
                get => (string?)this[nameof(this.Name)] ?? string.Empty;
                set => this[nameof(this.Name)] = value;
            }
        }
    }
}