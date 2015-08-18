namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;

    using Kephas.Model.Runtime.Construction;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimeAnnotationInfoBaseTest
    {
        [Test]
        public void TryCreateElement_Name_do_not_allow_multiple()
        {
            var elementInfo = new DefaultRuntimeAnnotationInfo(new NotMultipleAttribute());
            Assert.IsNotNull(elementInfo);
            Assert.AreEqual("@NotMultiple", elementInfo.Name);
        }

        [Test]
        public void TryCreateElement_Name_do_not_allow_multiple_implicit()
        {
            var elementInfo = new DefaultRuntimeAnnotationInfo(new NotMultipleImplicitAttribute());
            Assert.IsNotNull(elementInfo);
            Assert.AreEqual("@NotMultipleImplicit", elementInfo.Name);
        }

        [Test]
        public void TryCreateElement_Name_allow_multiple()
        {
            var elementInfo = new DefaultRuntimeAnnotationInfo(new MultipleAttribute());
            Assert.IsNotNull(elementInfo);
            Assert.AreEqual("@Multiple_" + elementInfo.RuntimeElement.GetHashCode(), elementInfo.Name);
        }

        [Test]
        public void TryCreateElement_Name_not_cannonical_name()
        {
            var elementInfo = new DefaultRuntimeAnnotationInfo(new NonCannonical());
            Assert.IsNotNull(elementInfo);
            Assert.AreEqual("@NonCannonical", elementInfo.Name);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        public class NotMultipleAttribute : Attribute { }

        public class NotMultipleImplicitAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class MultipleAttribute : Attribute { }

        public class NonCannonical : Attribute { }

        public class DefaultRuntimeAnnotationInfo : RuntimeAnnotationInfoBase<Attribute>
        {
            public DefaultRuntimeAnnotationInfo(Attribute runtimeElement)
                : base(runtimeElement)
            {
            }
        }

        public class RequiredAttribute : Attribute { }

        public class RequiredRuntimeAnnotationInfo : RuntimeAnnotationInfoBase<RequiredAttribute>
        {
            public RequiredRuntimeAnnotationInfo(RequiredAttribute runtimeElement)
                : base(runtimeElement)
            {
            }
        }
    }
}