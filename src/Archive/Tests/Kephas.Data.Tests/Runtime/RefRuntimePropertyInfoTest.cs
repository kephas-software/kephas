// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefRuntimePropertyInfoTest.cs" company="Kephas Software SRL">
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
    public class RefRuntimePropertyInfoTest
    {
        [Test]
        public void RefType_typed_reference()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var propInfo = new RefRuntimePropertyInfo(typeRegistry, typeof(IMyEntity).GetProperty(nameof(IMyEntity.StringRef)));
            var stringRefType = propInfo.RefType;
            Assert.AreSame(typeRegistry.GetTypeInfo(typeof(string)), stringRefType);
        }

        [Test]
        public void RefType_untyped_reference()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var propInfo = new RefRuntimePropertyInfo(typeRegistry, typeof(IMyEntity).GetProperty(nameof(IMyEntity.ObjectRef)));
            var objectRefType = propInfo.RefType;
            Assert.AreSame(typeRegistry.GetTypeInfo(typeof(object)), objectRefType);
        }

        public interface IMyEntity
        {
            public IRef<string> StringRef { get; }

            public IRef ObjectRef { get; }
        }
    }
}