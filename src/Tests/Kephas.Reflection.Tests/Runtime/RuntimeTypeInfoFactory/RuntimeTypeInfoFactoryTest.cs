// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeTypeInfo" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Runtime.RuntimeTypeInfoFactory
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Runtime.Factories;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="RuntimeTypeInfo"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeTypeInfoFactoryTest
    {
        [Test]
        public void TryCreateElementInfo_default()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var typeInfo = (typeRegistry as IRuntimeElementInfoFactory).TryCreateElementInfo(typeRegistry, typeof(int));
            Assert.IsInstanceOf<RuntimeTypeInfo>(typeInfo);
        }

        [Test]
        public void TryCreateElementInfo_attributed()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            typeRegistry.RegisterFactory(new AttributedRuntimeTypeInfoFactory());
            var typeInfo = (typeRegistry as IRuntimeElementInfoFactory).TryCreateElementInfo(typeRegistry, typeof(HasSpecialRuntimeTypeInfo));
            Assert.IsInstanceOf<SpecialRuntimeTypeInfo>(typeInfo);
        }

        public class AttributedRuntimeTypeInfoFactory : RuntimeTypeInfoFactoryBase
        {
            public override IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, int position = -1, ILogger? logger = null)
            {
                var rawType = reflectInfo;
                var attr = rawType.GetCustomAttribute<RuntimeTypeInfoAttribute>();
                var typeInfoType = attr?.Type;
                if (typeInfoType == null)
                {
                    return null;
                }

                var typeInfo = (IRuntimeTypeInfo)Activator.CreateInstance(typeInfoType, registry, rawType);
                return typeInfo;
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
        public sealed class RuntimeTypeInfoAttribute : Attribute
        {
            public RuntimeTypeInfoAttribute(Type type)
            {
                this.Type = type;
            }

            public Type Type { get; }
        }

        [RuntimeTypeInfo(typeof(SpecialRuntimeTypeInfo))]
        public class HasSpecialRuntimeTypeInfo { }

        public class SpecialRuntimeTypeInfo : RuntimeTypeInfo
        {
            public SpecialRuntimeTypeInfo(IRuntimeTypeRegistry typeRegistry, Type type)
                : base(typeRegistry, type)
            {
            }
        }
    }
}
