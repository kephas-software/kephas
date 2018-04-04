// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttributeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service contract attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services
{
    using System;
    using System.Linq;

    using Kephas.Services;

    using NUnit.Framework;

    /// <summary>
    /// An application service contract attribute test.
    /// </summary>
    [TestFixture]
    public class AppServiceContractAttributeTest
    {
        [Test]
        public void DefaultMetadataAttributeTypes()
        {
            Assert.IsTrue(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(OverridePriorityAttribute)));
            Assert.IsTrue(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(ProcessingPriorityAttribute)));
            Assert.IsTrue(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(OptionalServiceAttribute)));
            Assert.IsTrue(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(ServiceNameAttribute)));
        }

        [Test]
        public void RegisterDefaultMetadataAttributeTypes_success()
        {
            Assert.IsFalse(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(DummyAttribute)));

            AppServiceContractAttribute.RegisterDefaultMetadataAttributeTypes(typeof(DummyAttribute));

            Assert.IsTrue(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(DummyAttribute)));
        }

        [Test]
        public void RegisterDefaultMetadataAttributeTypes_unique_types()
        {
            Assert.IsTrue(AppServiceContractAttribute.DefaultMetadataAttributeTypes.Contains(typeof(ServiceNameAttribute)));

            AppServiceContractAttribute.RegisterDefaultMetadataAttributeTypes(typeof(ServiceNameAttribute));

            Assert.AreEqual(1, AppServiceContractAttribute.DefaultMetadataAttributeTypes.Count(t => t == typeof(ServiceNameAttribute)));
        }

        public class DummyAttribute : Attribute
        {
        }
    }
}