// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeAnnotationInfoFactoryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A runtime annotation information factory test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Factory
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Factory;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// A runtime annotation information factory test.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultRuntimeAnnotationInfoFactoryTest
    {
        [Test]
        public void TryCreateElement_ReturnType()
        {
            var factory = new DefaultRuntimeAnnotationInfoFactory();
            var elementInfo = factory.TryGetElementInfo(Mock.Create<IRuntimeModelInfoFactory>(), new NotMultipleAttribute());
            Assert.IsNotNull(elementInfo);
            Assert.IsInstanceOf<DefaultRuntimeAnnotationInfo>(elementInfo);
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        public class NotMultipleAttribute : Attribute { }
    }
}