// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAppLifecycleBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Data.Runtime;

namespace Kephas.Data.Tests.Application
{
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Data.Application;
    using Kephas.Runtime;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataAppLifecycleBehaviorTest
    {
        [Test]
        public async Task BeforeAppInitializeAsync_RefRuntimePropertyInfoFactory_registered()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var behavior = new DataAppLifecycleBehavior(typeRegistry);
            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>());
            var typeInfo = typeRegistry.GetTypeInfo(typeof(IMyEntity));

            Assert.IsInstanceOf<RefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyEntity.StringRef)]);
            Assert.IsInstanceOf<RefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyEntity.ObjectRef)]);
        }

        [Test]
        public async Task BeforeAppInitializeAsync_ServiceRefRuntimePropertyInfoFactory_registered()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var behavior = new DataAppLifecycleBehavior(typeRegistry);
            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>());
            var typeInfo = typeRegistry.GetTypeInfo(typeof(IMyServiceEntity));

            Assert.IsInstanceOf<ServiceRefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyServiceEntity.StringServiceRef)]);
            Assert.IsInstanceOf<ServiceRefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyServiceEntity.ObjectServiceRef)]);
        }

        public interface IMyEntity
        {
            public IRef<string> StringRef { get; }

            public IRef ObjectRef { get; }
        }

        public interface IMyServiceEntity
        {
            public IServiceRef<string> StringServiceRef { get; }

            public IServiceRef ObjectServiceRef { get; }
        }
    }
}