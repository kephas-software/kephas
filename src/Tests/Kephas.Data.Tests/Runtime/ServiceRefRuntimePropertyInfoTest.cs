// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRefRuntimePropertyInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Runtime
{
    using Kephas.Data.Runtime;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceRefRuntimePropertyInfoTest
    {
        [Test]
        public void ServiceRefType_typed_reference()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var propInfo = new ServiceRefRuntimePropertyInfo(typeRegistry, typeof(IMyEntity).GetProperty(nameof(IMyEntity.StringRef)));
            var stringRefType = propInfo.ServiceRefType;
            Assert.AreSame(typeRegistry.GetTypeInfo(typeof(string)), stringRefType);
        }

        [Test]
        public void ServiceRefType_untyped_reference()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var propInfo = new ServiceRefRuntimePropertyInfo(typeRegistry, typeof(IMyEntity).GetProperty(nameof(IMyEntity.ObjectRef)));
            var objectRefType = propInfo.ServiceRefType;
            Assert.AreSame(typeRegistry.GetTypeInfo(typeof(object)), objectRefType);
        }

        public interface IMyEntity
        {
            public IServiceRef<string> StringRef { get; }

            public IServiceRef ObjectRef { get; }
        }
    }
}