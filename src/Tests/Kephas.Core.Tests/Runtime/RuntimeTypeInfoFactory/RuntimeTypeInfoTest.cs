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
    using System.Reflection;

    using Kephas.Runtime;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="RuntimeTypeInfo"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeTypeInfoFactoryTest
    {
        [Test]
        public void CreateRuntimeTypeInfo_default()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var typeInfo = typeRegistry.CreateRuntimeTypeInfo(typeof(int));
            Assert.IsInstanceOf<RuntimeTypeInfo>(typeInfo);
        }

        [Test]
        public void CreateRuntimeTypeInfo_attributed()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            typeRegistry.RegisterFactory(new AttributedRuntimeTypeInfoFactory(typeRegistry));
            var typeInfo = typeRegistry.CreateRuntimeTypeInfo(typeof(HasSpecialRuntimeTypeInfo));
            Assert.IsInstanceOf<SpecialRuntimeTypeInfo>(typeInfo);
        }

        public class AttributedRuntimeTypeInfoFactory : IRuntimeTypeInfoFactory
        {
            private readonly IRuntimeTypeRegistry typeRegistry;

            public AttributedRuntimeTypeInfoFactory(IRuntimeTypeRegistry typeRegistry)
            {
                this.typeRegistry = typeRegistry;
            }

            public IRuntimeTypeInfo? TryCreateRuntimeTypeInfo(Type rawType)
            {
                var attr = rawType.GetCustomAttribute<RuntimeTypeInfoAttribute>();
                var typeInfoType = attr?.Type;
                if (typeInfoType == null)
                {
                    return null;
                }

                var typeInfo = (IRuntimeTypeInfo)Activator.CreateInstance(typeInfoType, this.typeRegistry, rawType);
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
