// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.PermissionsModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model.Security.Authorization.Elements;
    using Kephas.Runtime;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.Application;
    using Kephas.Services;
    using Kephas.Testing.Model;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PermissionsTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_permissioninfo_support()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var behavior = new AuthorizationAppLifecycleBehavior(typeRegistry);
            await behavior.BeforeAppInitializeAsync(Substitute.For<IContext>());

            var container = this.CreateContainerForModel(
                new AmbientServices(typeRegistry: typeRegistry),
                typeof(IDoPermission),
                typeof(ISpecialDoPermission));
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var doPermission = (PermissionType)modelSpace.Classifiers.Single(c => c.Name == "Do");
            var specialDoPermission = (PermissionType)modelSpace.Classifiers.Single(c => c.Name == "SpecialDo");

            Assert.AreEqual(1, doPermission.Parts.Count());
            Assert.AreEqual("do", doPermission.TokenName);
            Assert.AreEqual(Scoping.Type | Scoping.Instance, doPermission.Scoping);

            Assert.AreEqual(1, specialDoPermission.Parts.Count());
            Assert.AreEqual("special-do", specialDoPermission.TokenName);
            Assert.AreEqual(Scoping.Type, specialDoPermission.Scoping);
        }
    }
}