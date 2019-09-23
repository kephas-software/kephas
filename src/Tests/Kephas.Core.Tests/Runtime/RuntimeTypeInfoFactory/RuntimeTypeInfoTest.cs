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
        private static bool initialized;

        private static object sync = new object();

        [OneTimeSetUp]
        public void RegisterFactory()
        {
            if (!initialized)
            {
                lock (sync)
                {
                    if (!initialized)
                    {
                        RuntimeTypeInfo.RegisterFactory(new AttributedRuntimeTypeInfoFactory());
                    }
                }
            }
        }

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

        public class AttributedRuntimeTypeInfoFactory : IRuntimeTypeInfoFactory
        {
            public IRuntimeTypeInfo TryCreateRuntimeTypeInfo(Type rawType)
            {
                var attr = rawType.GetCustomAttribute<RuntimeTypeInfoTypeAttribute>();
                var typeInfoType = attr?.Type;
                if (typeInfoType == null)
                {
                    return null;
                }

                var typeInfo = (IRuntimeTypeInfo)Activator.CreateInstance(typeInfoType, rawType);
                return typeInfo;
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
        public sealed class RuntimeTypeInfoTypeAttribute : Attribute
        {
            public RuntimeTypeInfoTypeAttribute(Type type)
            {
                this.Type = type;
            }

            public Type Type { get; }
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
