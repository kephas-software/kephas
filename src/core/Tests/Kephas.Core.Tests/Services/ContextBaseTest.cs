// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ContextBase" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services
{
    using System.Security.Principal;

    using Kephas.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Test class for <see cref="ContextBase"/>.
    /// </summary>
    [TestClass]
    public class ContextBaseTest
    {
        [TestMethod]
        public void Dynamic_Context()
        {
            dynamic context = new TestContext();
            context.Value = 12;
            Assert.AreEqual(12, context.Value);

            var mockUser = Mock.Create<IIdentity>();
            context.AuthenticatedUser = mockUser;
            Assert.AreEqual(mockUser, context.AuthenticatedUser);

            var contextBase = (ContextBase)context;
            Assert.AreEqual(mockUser, contextBase.AuthenticatedUser);

            Assert.AreEqual(mockUser, contextBase["AuthenticatedUser"]);
            Assert.AreEqual(12, contextBase["Value"]);
        }

        private class TestContext : ContextBase
        {
        }
    }
}
