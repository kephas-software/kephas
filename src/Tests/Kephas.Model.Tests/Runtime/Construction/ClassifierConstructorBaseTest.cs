// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructorBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the classifier constructor base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;
    using System.Linq;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    using Telerik.JustMock;

    [TestFixture]
    public class ClassifierConstructorBaseTest
    {
        private INamedElement TryCreateTestModelElement()
        {
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var constructor = new TestClassifierConstructor();
            var runtimeElement = typeof(TestContact).AsDynamicTypeInfo();
            var modelElement = constructor.TryCreateModelElement(context, runtimeElement);

            return modelElement;
        }

        [Test]
        public void TryCreateModelElement_ReturnType()
        {
            var modelElement = this.TryCreateTestModelElement();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<TestClassifier>(modelElement);
        }

        [Test]
        public void TryCreateModelElement_Members()
        {
            var modelElement = (ClassifierBase<IClassifier>)this.TryCreateTestModelElement();

            Assert.AreEqual(3, modelElement.Members.Count());
        }

        [Test]
        public void TryCreateModelElement_Annotations()
        {
            var modelElement = (ClassifierBase<IClassifier>)this.TryCreateTestModelElement();

            Assert.AreEqual(1, modelElement.Annotations.Count());
        }

        [Test]
        public void TryCreateModelElement_Properties()
        {
            var modelElement = (ClassifierBase<IClassifier>)this.TryCreateTestModelElement();

            Assert.AreEqual(2, modelElement.Properties.Count());
        }

        private class TestClassifier : ClassifierBase<IClassifier>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ClassifierBase{TModelContract}" /> class.
            /// </summary>
            /// <param name="constructionContext">Context for the construction.</param>
            /// <param name="name">The name.</param>
            public TestClassifier(IModelConstructionContext constructionContext, string name)
                : base(constructionContext, name)
            {
            }
        }

        private class TestClassifierConstructor : ClassifierConstructorBase<TestClassifier, IClassifier>
        {
            /// <summary>
            /// Core implementation of trying to get the element information.
            /// </summary>
            /// <param name="constructionContext">Context for the construction.</param>
            /// <param name="runtimeElement">The runtime element.</param>
            /// <returns>
            /// A new element information based on the provided runtime element information, or <c>null</c>
            /// if the runtime element information is not supported.
            /// </returns>
            protected override TestClassifier TryCreateModelElementCore(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
            {
                return new TestClassifier(constructionContext, this.TryComputeName(constructionContext, runtimeElement));
            }
        }

        [Serializable]
        private class TestContact
        {
            public string Name { get; set; }

            public DateTime? Birthday { get; }
        }
    }
}