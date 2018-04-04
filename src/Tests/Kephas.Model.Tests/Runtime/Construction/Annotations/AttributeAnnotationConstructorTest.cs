// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeAnnotationConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A runtime annotation information factory test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction.Annotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction.Annotations;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// A runtime annotation information factory test.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AttributeAnnotationConstructorTest
    {
        [Test]
        public void TryCreateModelElement_success()
        {
            var constructor = new AttributeAnnotationConstructor();
            var context = new ModelConstructionContext(Substitute.For<IAmbientServices>()) { ModelSpace = Substitute.For<IModelSpace>() };
            var annotation = constructor.TryCreateModelElement(context, new AnnotationConstructorBaseTest.NotMultipleAttribute());

            Assert.IsInstanceOf<Annotation>(annotation);
            Assert.AreEqual("@NotMultiple", annotation.Name);
            Assert.AreEqual(false, ((Annotation)annotation).AllowMultiple);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        public class NotMultipleAttribute : Attribute { }
    }
}