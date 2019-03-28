// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoSetUp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime type information set up class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime.RuntimeTypeInfoFactory
{
    using System;
    using System.Reflection;

    using Kephas.Runtime;

    using NUnit.Framework;

    [SetUpFixture]
    public class RuntimeTypeInfoSetUp
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
}