// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfoFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A runtime property information factory test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// A runtime property information factory test.
    /// </summary>
    [TestFixture]
    public class RuntimePropertyInfoFactoryTest
    {
        [Test]
        public void TryCreateElement_ReturnType()
        {
            var factory = new PropertyConstructor();
            var propertyInfo = typeof(TestModelElement).GetTypeInfo().GetProperty("Age");
            var elementInfo = factory.TryCreateModelElement(Mock.Create<IRuntimeModelElementFactory>(), propertyInfo);
            Assert.IsNotNull(elementInfo);
            Assert.IsInstanceOf<RuntimePropertyInfo>(elementInfo);
        }

        [Test]
        public void TryCreateElement_Name()
        {
            var factory = new PropertyConstructor();
            var propertyInfo = typeof(TestModelElement).GetTypeInfo().GetProperty("Age");
            var elementInfo = (RuntimePropertyInfo)factory.TryCreateModelElement(Mock.Create<IRuntimeModelElementFactory>(), propertyInfo);
            Assert.AreEqual("Age", elementInfo.Name);
        }

        public class TestModelElement
        {
            public int Age { get; set; }
        }
    }
}