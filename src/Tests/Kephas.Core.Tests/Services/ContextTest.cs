// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="Context" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services
{
    using System.Security.Principal;

    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="Context"/>.
    /// </summary>
    [TestFixture]
    public class ContextTest
    {
        [Test]
        public void Dynamic_Context()
        {
            dynamic context = new Context();
            context.Value = 12;
            Assert.AreEqual(12, context.Value);

            var mockUser = Substitute.For<IIdentity>();
            context.Identity = mockUser;
            Assert.AreSame(mockUser, context.Identity);

            var contextBase = (Context)context;
            Assert.AreSame(mockUser, contextBase.Identity);

            Assert.AreSame(mockUser, contextBase["Identity"]);
            Assert.AreEqual(12, contextBase["Value"]);
        }
    }
}
