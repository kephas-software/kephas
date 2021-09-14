// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionsModelRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Security.Permissions.Runtime.ModelRegistries
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Model.Security.Permissions;
    using Kephas.Model.Security.Permissions.AttributedModel;
    using Kephas.Model.Security.Permissions.Elements;
    using Kephas.Model.Security.Permissions.Runtime.ModelRegistries;
    using Kephas.Reflection;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;
    using Kephas.Testing.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PermissionsModelRegistryTest : CompositionTestBase
    {
        [Test]
        public async Task GetRuntimeElementsAsync()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(Arg.Any<Assembly>())
                .Returns(new[]
                {
                    typeof(IPermissionType),
                    typeof(PermissionType),
                    typeof(string),
                    typeof(SystemPermission),
                    typeof(AppAdminPermission),
                    typeof(ModelAttributedPermission),
                    typeof(AttributedPermission),
                    typeof(IReadPermission),
                });

            var contextFactory = this.CreateContextFactoryMock(() =>
                new ModelRegistryConventions(Substitute.For<IInjector>()));

            var registry = new PermissionsModelRegistry(contextFactory, appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(5, result.Count);
            Assert.AreSame(typeof(SystemPermission), result[0]);
            Assert.AreSame(typeof(AppAdminPermission), result[1]);
            Assert.AreSame(typeof(ModelAttributedPermission), result[2]);
            Assert.AreSame(typeof(AttributedPermission), result[3]);
            Assert.AreSame(typeof(IReadPermission), result[4]);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_ExcludeFromModel()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { this.GetType().Assembly });

            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(Arg.Any<Assembly>()).Returns(new[] { typeof(IPermissionType), typeof(PermissionType), typeof(string), typeof(ExcludedPermission) });

            var contextFactory = this.CreateContextFactoryMock(() =>
                new ModelRegistryConventions(Substitute.For<IInjector>()));

            var registry = new PermissionsModelRegistry(contextFactory, appRuntime, typeLoader);
            var result = (await registry.GetRuntimeElementsAsync()).ToList();
            Assert.AreEqual(0, result.Count);
        }

        [ExcludeFromModel]
        [PermissionInfo]
        public class ExcludedPermission
        {
        }

        [PermissionType("model-attributed")]
        public class ModelAttributedPermission {}

        [PermissionInfo("attributed")]
        public class AttributedPermission {}

        [PermissionInfo]
        public interface IReadPermission {}
    }
}