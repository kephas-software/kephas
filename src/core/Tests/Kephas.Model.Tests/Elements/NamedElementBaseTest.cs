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
    using Kephas.Model.Elements.Construction;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    /// <summary>
    /// Test class for <see cref="NamedElementBase{TModelContract}"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class NamedElementBaseTest
    {
        [Test]
        public void Constructor_Success()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var elementInfo = Mock.Create<INamedElementInfo>();
            elementInfo.Arrange(e => e.Name).Returns("name");

            var element = new TestNamedElement(elementInfo, modelSpace);

            Assert.AreEqual(modelSpace, element.ModelSpace);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("name", element.QualifiedName);
        }

        [Test]
        public void Constructor_Success_WithDiscriminator()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var elementInfo = Mock.Create<INamedElementInfo>();
            elementInfo.Arrange(e => e.Name).Returns("name");

            var element = new TestNamedElementWithDiscriminator(elementInfo, modelSpace);

            Assert.AreEqual(modelSpace, element.ModelSpace);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("##name", element.QualifiedName);
        }

        [Test]
        [ExpectedException]
        public void Constructor_Failure_ModelSpace_not_set()
        {
            var elementInfo = Mock.Create<INamedElementInfo>();
            var element = new TestNamedElement(elementInfo, null);
        }

        [Test]
        [ExpectedException]
        public void Constructor_Failure_Name_not_set()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var element = new TestNamedElement(null, modelSpace);
        }

        private interface ITestElement
        {
        }

        [MemberNameDiscriminator("##")]
        private interface ITestElementWithDiscriminator
        {
        }

        private class TestNamedElement : NamedElementBase<ITestElement, INamedElementInfo>
        {
            public TestNamedElement(INamedElementInfo elementInfo, IModelSpace modelSpace)
                : base(elementInfo, modelSpace)
            {
            }
        }

        private class TestNamedElementWithDiscriminator : NamedElementBase<ITestElementWithDiscriminator, INamedElementInfo>
        {
            public TestNamedElementWithDiscriminator(INamedElementInfo elementInfo, IModelSpace modelSpace)
                : base(elementInfo, modelSpace)
            {
            }
        }
    }
}