namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Annotations;

    using NUnit.Framework;

    using Telerik.JustMock;

    [TestFixture]
    public class AnnotationConstructorBaseTest
    {
        [Test]
        public void TryCreateModelElement_Name_do_not_allow_multiple()
        {
            var constructor = new TestAnnotationConstructor();
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var annotation = constructor.TryCreateModelElement(context, new NotMultipleAttribute());
            Assert.IsNotNull(annotation);
            Assert.AreEqual("@NotMultiple", annotation.Name);
        }

        [Test]
        public void TryCreateModelElement_Name_do_not_allow_multiple_implicit()
        {
            var constructor = new TestAnnotationConstructor();
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var annotation = constructor.TryCreateModelElement(context, new NotMultipleImplicitAttribute());
            Assert.IsNotNull(annotation);
            Assert.AreEqual("@NotMultipleImplicit", annotation.Name);
        }

        [Test]
        public void TryCreateModelElement_Name_allow_multiple()
        {
            var constructor = new TestAnnotationConstructor();
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var annotation = constructor.TryCreateModelElement(context, new MultipleAttribute());
            Assert.IsNotNull(annotation);
            Assert.AreEqual("@Multiple_" + annotation.Parts.Single().GetHashCode(), annotation.Name);
        }

        [Test]
        public void TryCreateModelElement_Name_not_cannonical_name()
        {
            var constructor = new TestAnnotationConstructor();
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var annotation = constructor.TryCreateModelElement(context, new NonCannonical());
            Assert.IsNotNull(annotation);
            Assert.AreEqual("@NonCannonical", annotation.Name);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        public class NotMultipleAttribute : Attribute { }

        public class NotMultipleImplicitAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class MultipleAttribute : Attribute { }

        public class NonCannonical : Attribute { }

        public class TestAnnotation : Annotation
        {
            public TestAnnotation(IModelConstructionContext constructionContext, string name)
                : base(constructionContext, name)
            {
            }
        }

        public class TestAnnotationConstructor : AnnotationConstructorBase<Annotation, Attribute>
        {
            protected override Annotation TryCreateModelElementCore(IModelConstructionContext constructionContext, Attribute runtimeElement)
            {
                return new TestAnnotation(constructionContext, this.TryComputeNameCore(runtimeElement));
            }
        }
    }
}