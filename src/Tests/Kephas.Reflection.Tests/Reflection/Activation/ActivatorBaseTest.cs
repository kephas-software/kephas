// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivatorBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activator base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection.Activation
{
    using System;
    using Kephas.Activation;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ActivatorBase"/>.
    /// </summary>
    [TestFixture]
    public class ActivatorBaseTest
    {
        [Test]
        public void CreateInstance_no_args()
        {
            var activator = new Activator();
            var date = activator.CreateInstance(typeof(DateTime).AsRuntimeTypeInfo());
            Assert.IsInstanceOf<DateTime>(date);
        }

        [Test]
        public void GetImplementationType_overridden_core()
        {
            var activator = new OverriddenActivator(t => typeof(string).AsRuntimeTypeInfo());
            var implementationType = activator.GetImplementationType(typeof(int).AsRuntimeTypeInfo());
            Assert.AreEqual(typeof(string), ((IRuntimeTypeInfo)implementationType).Type);
        }

        public class Activator : ActivatorBase { }

        public class OverriddenActivator : ActivatorBase
        {
            private readonly Func<ITypeInfo, ITypeInfo> converter;

            public OverriddenActivator(Func<ITypeInfo, ITypeInfo> converter)
            {
                this.converter = converter;
            }

            protected override ITypeInfo? ComputeImplementationType(ITypeInfo abstractType, dynamic? activationContext = null, bool throwOnNotFound = true)
            {
                return this.converter(abstractType);
            }
        }
    }
}