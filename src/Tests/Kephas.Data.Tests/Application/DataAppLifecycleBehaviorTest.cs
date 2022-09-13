// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAppLifecycleBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Application
{
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Data.Runtime;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataAppLifecycleBehaviorTest
    {
        [Test]
        public async Task BeforeAppInitializeAsync_RefRuntimePropertyInfoFactory_registered()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            this.RegisterFactories(typeRegistry);
            var typeInfo = typeRegistry.GetTypeInfo(typeof(IMyEntity));

            Assert.IsInstanceOf<RefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyEntity.StringRef)]);
            Assert.IsInstanceOf<RefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyEntity.ObjectRef)]);
        }

        [Test]
        public async Task BeforeAppInitializeAsync_ServiceRefRuntimePropertyInfoFactory_registered()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            this.RegisterFactories(typeRegistry);
            var typeInfo = typeRegistry.GetTypeInfo(typeof(IMyServiceEntity));

            Assert.IsInstanceOf<ServiceRefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyServiceEntity.StringServiceRef)]);
            Assert.IsInstanceOf<ServiceRefRuntimePropertyInfo>(typeInfo.Properties[nameof(IMyServiceEntity.ObjectServiceRef)]);
        }

        private void RegisterFactories(IRuntimeTypeRegistry typeRegistry)
        {
            typeRegistry.RegisterFactory(new RuntimeEntityInfoFactory());
            typeRegistry.RegisterFactory(new RefRuntimePropertyInfoFactory());
            typeRegistry.RegisterFactory(new ServiceRefRuntimePropertyInfoFactory());
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