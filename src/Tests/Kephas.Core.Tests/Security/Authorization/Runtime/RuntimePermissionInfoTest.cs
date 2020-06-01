// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePermissionInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Security.Authorization.Runtime
{
    using System.Linq;

    using Kephas.Runtime;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimePermissionInfoTest
    {
        [OneTimeSetUp]
        public void OneTImeSetUp()
        {
            RuntimeTypeInfo.RegisterFactory(new AuthorizationTypeInfoFactory());
        }

        [Test]
        public void Constructor_for_system()
        {
            var info = new RuntimePermissionInfo(typeof(SystemPermission));

            Assert.AreEqual("system", info.TokenName);
            Assert.AreEqual(1, info.RequiredPermissions.Count());
            Assert.AreEqual(0, info.GrantedPermissions.Count());
            Assert.AreEqual(info.TokenName, info.RequiredPermissions.First().TokenName);
            Assert.AreEqual(info.Scoping, info.RequiredPermissions.First().Scoping);
        }

        [Test]
        public void Constructor_for_appadmin()
        {
            var info = new RuntimePermissionInfo(typeof(AppAdminPermission));

            Assert.AreEqual("appadmin", info.TokenName);
            Assert.AreEqual(0, info.RequiredPermissions.Count());
            Assert.AreEqual(0, info.GrantedPermissions.Count());
        }

        [Test]
        public void Constructor_compute_token_name()
        {
            var info = new RuntimePermissionInfo(typeof(ITestReadPermission));

            Assert.AreEqual("TestRead", info.TokenName);
            Assert.AreEqual(0, info.RequiredPermissions.Count());
            Assert.AreEqual(0, info.GrantedPermissions.Count());
            Assert.AreEqual(Scoping.Global, info.Scoping);
        }

        [Test]
        public void Constructor_derived_permission()
        {
            var info = new RuntimePermissionInfo(typeof(IDerivedTestPermission));

            Assert.AreEqual("DerivedTest", info.TokenName);
            Assert.AreEqual(0, info.RequiredPermissions.Count());
            Assert.AreEqual(1, info.GrantedPermissions.Count());
            Assert.AreEqual("TestRead", info.GrantedPermissions.First().TokenName);
            Assert.AreEqual(Scoping.Global, info.Scoping);
        }

        public interface ITestReadPermission : IPermission {}
        public interface IDerivedTestPermission : ITestReadPermission {}
    }
}