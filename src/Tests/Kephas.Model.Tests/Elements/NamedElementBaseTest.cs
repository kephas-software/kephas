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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;

    using NUnit.Framework;

    using Telerik.JustMock;

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
            var context = new ModelConstructionContext { ModelSpace = modelSpace };
            var element = new TestNamedElement(context, "name");

            Assert.AreEqual(modelSpace, element.ModelSpace);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("name", element.QualifiedName);
        }

        [Test]
        public void Constructor_Success_WithDiscriminator()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var context = new ModelConstructionContext { ModelSpace = modelSpace };

            var element = new TestNamedElementWithDiscriminator(context, "name");

            Assert.AreEqual(modelSpace, element.ModelSpace);
            Assert.AreEqual("name", element.Name);
            Assert.AreEqual("##name", element.QualifiedName);
        }

        [Test]
        public void Constructor_Failure_ModelSpace_not_set()
        {
            var context = new ModelConstructionContext { ModelSpace = null };
            Assert.That(() => new TestNamedElement(context, "name"), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void Constructor_Failure_Name_not_set()
        {
            var modelSpace = Mock.Create<IModelSpace>();
            var context = new ModelConstructionContext { ModelSpace = modelSpace };
            Assert.That(() => new TestNamedElement(context, null), Throws.InstanceOf<Exception>());
        }

        private interface ITestElement : INamedElement
        {
        }

        [MemberNameDiscriminator("##")]
        private interface ITestElementWithDiscriminator : INamedElement
        {
        }

        private class TestNamedElement : NamedElementBase<ITestElement>
        {
            public TestNamedElement(IModelConstructionContext constructionContext, string name)
                : base(constructionContext, name)
            {
            }

            /// <summary>
            /// Gets the annotations of this model element.
            /// </summary>
            /// <value>
            /// The model element annotations.
            /// </value>
            public override IEnumerable<IAnnotation> Annotations => new List<IAnnotation>();
        }

        private class TestNamedElementWithDiscriminator : NamedElementBase<ITestElementWithDiscriminator>
        {
            public TestNamedElementWithDiscriminator(IModelConstructionContext constructionContext, string name)
                : base(constructionContext, name)
            {
            }

            /// <summary>
            /// Gets the annotations of this model element.
            /// </summary>
            /// <value>
            /// The model element annotations.
            /// </value>
            public override IEnumerable<IAnnotation> Annotations => new List<IAnnotation>();
        }
    }
}