// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A runtime annotation information factory test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction.Annotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.Elements;
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction.Annotations;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// A runtime annotation information factory test.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AnnotationConstructorTest
    {
        [Test]
        public void TryCreateModelElement_success()
        {
            var constructor = new AnnotationConstructor();
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var annotation = constructor.TryCreateModelElement(context, new NotMultipleAttribute());

            Assert.IsInstanceOf<Annotation>(annotation);
            Assert.AreEqual("@NotMultiple", annotation.Name);
            Assert.AreEqual(false, ((Annotation)annotation).AllowMultiple);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        public class NotMultipleAttribute : Attribute { }
    }
}