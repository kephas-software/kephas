// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Tests
{
    using System;
    using System.Linq;

    using Kephas.Data.Capabilities;
    using Kephas.Services;

    using NUnit.Framework;

    [TestFixture]
    public class ContextExtensionsTest
    {
        [Test]
        public void GetInitialData_not_set()
        {
            var context = new TestContext();
            var initialData = context.InitialData();
            Assert.IsNull(initialData);
        }

        [Test]
        public void SetInitialData_object_enumeration()
        {
            var context = new TestContext();
            context.WithInitialData(new[] { "ana", "are", "mere" });

            var initialData = context.InitialData();
            Assert.IsNotNull(initialData);
            Assert.AreEqual(3, initialData.Count());

            var initialDataList = initialData.ToList();
            Assert.AreEqual("ana", initialDataList[0].Entity);
            Assert.AreEqual(ChangeState.NotChanged, initialDataList[0].ChangeState);
            Assert.AreEqual("are", initialDataList[1].Entity);
            Assert.AreEqual(ChangeState.NotChanged, initialDataList[1].ChangeState);
            Assert.AreEqual("mere", initialDataList[2].Entity);
            Assert.AreEqual(ChangeState.NotChanged, initialDataList[2].ChangeState);
        }

        [Test]
        public void SetInitialData_tuple_enumeration()
        {
            var context = new TestContext();
            context.WithInitialData(new[]
                                       {
                                           ((object)"ana", ChangeState.Added),
                                           ((object)"are", ChangeState.Changed),
                                           ((object)"mere", ChangeState.NotChanged),
                                       });

            var initialData = context.InitialData();
            Assert.IsNotNull(initialData);
            Assert.AreEqual(3, initialData.Count());

            var initialDataList = initialData.ToList();
            Assert.AreEqual("ana", (string)initialDataList[0].Entity);
            Assert.AreEqual(ChangeState.Added, initialDataList[0].ChangeState);
            Assert.AreEqual("are", (string)initialDataList[1].Entity);
            Assert.AreEqual(ChangeState.Changed, initialDataList[1].ChangeState);
            Assert.AreEqual("mere", (string)initialDataList[2].Entity);
            Assert.AreEqual(ChangeState.NotChanged, initialDataList[2].ChangeState);
        }

        [Test]
        public void SetInitialData_entity_info_enumeration()
        {
            var context = new TestContext();
            context.WithInitialData(new[]
                                       {
                                           new EntityInfo("ana") { ChangeState = ChangeState.Added },
                                           new EntityInfo("are") { ChangeState = ChangeState.Changed },
                                           new EntityInfo("mere") { ChangeState = ChangeState.NotChanged },
                                       });

            var initialData = context.InitialData();
            Assert.IsNotNull(initialData);
            Assert.AreEqual(3, initialData.Count());

            var initialDataList = initialData.ToList();
            Assert.AreEqual("ana", initialDataList[0].Entity);
            Assert.AreEqual(ChangeState.Added, initialDataList[0].ChangeState);
            Assert.AreEqual("are", initialDataList[1].Entity);
            Assert.AreEqual(ChangeState.Changed, initialDataList[1].ChangeState);
            Assert.AreEqual("mere", initialDataList[2].Entity);
            Assert.AreEqual(ChangeState.NotChanged, initialDataList[2].ChangeState);
        }

        public class TestContext : Context
        {
        }
    }
}