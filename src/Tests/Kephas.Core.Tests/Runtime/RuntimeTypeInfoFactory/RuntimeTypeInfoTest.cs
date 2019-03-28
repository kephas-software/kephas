// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeTypeInfo" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime.RuntimeTypeInfoFactory
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Runtime;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="RuntimeTypeInfo"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeTypeInfoFactoryTest
    {
        [Test]
        public void CreateRuntimeTypeInfo_default()
        {
            var typeInfo = RuntimeTypeInfo.CreateRuntimeTypeInfo(typeof(int));
            Assert.IsInstanceOf<RuntimeTypeInfo>(typeInfo);
        }

        [Test]
        public void CreateRuntimeTypeInfo_attributed()
        {
            var typeInfo = RuntimeTypeInfo.CreateRuntimeTypeInfo(typeof(HasSpecialRuntimeTypeInfo));
            Assert.IsInstanceOf<SpecialRuntimeTypeInfo>(typeInfo);
        }

        [RuntimeTypeInfoType(typeof(SpecialRuntimeTypeInfo))]
        public class HasSpecialRuntimeTypeInfo { }

        public class SpecialRuntimeTypeInfo : RuntimeTypeInfo
        {
            public SpecialRuntimeTypeInfo(Type type)
                : base(type)
            {
            }
        }
    }
}
