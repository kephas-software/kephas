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
    using Kephas.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }

        private class TestContext : ContextBase
        {
        }
    }
}
