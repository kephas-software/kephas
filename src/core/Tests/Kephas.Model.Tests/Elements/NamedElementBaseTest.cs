// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedElementBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="NamedElementBase{TModelContract}" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Elements
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Test class for <see cref="NamedElementBase{TModelContract}"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class NamedElementBaseTest
    {
        [TestMethod]
        public void Constructor_Success()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var element = new TestNamedElement(modelSpace, "name");

            Assert.AreEqual(modelSpace, element.ModelSpace);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("name", element.QualifiedName);
        }

        [TestMethod]
        public void Constructor_Success_WithDiscriminator()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var element = new TestNamedElementWithDiscriminator(modelSpace, "name");

            Assert.AreEqual(modelSpace, element.ModelSpace);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("##name", element.QualifiedName);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void Constructor_Failure_ModelSpace_not_set()
        {
            var element = new TestNamedElement(null, "name");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void Constructor_Failure_Name_not_set()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var element = new TestNamedElement(modelSpace, null);
        }

        private interface ITestElement
        {
        }

        [MemberNameDiscriminator("##")]
        private interface ITestElementWithDiscriminator
        {
        }

        private class TestNamedElement : NamedElementBase<ITestElement>
        {
            public TestNamedElement(IModelSpace modelSpace, string name)
                : base(modelSpace, name)
            {
            }
        }

        private class TestNamedElementWithDiscriminator : NamedElementBase<ITestElementWithDiscriminator>
        {
            public TestNamedElementWithDiscriminator(IModelSpace modelSpace, string name)
                : base(modelSpace, name)
            {
            }
        }
    }
}