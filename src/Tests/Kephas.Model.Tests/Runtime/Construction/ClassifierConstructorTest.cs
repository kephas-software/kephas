// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the classifier constructor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;

    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class ClassifierConstructorTest : ConstructorTestBase
    {
        private INamedElement TryCreateTestModelElement()
        {
            var constructor = new ClassifierConstructor();
            var context = this.GetConstructionContext();
            var runtimeElement = typeof(TestContact).AsRuntimeTypeInfo();
            var modelElement = constructor.TryCreateModelElement(context, runtimeElement);

            return modelElement;
        }

        [Test]
        public void TryCreateModelElement_ReturnType()
        {
            var modelElement = this.TryCreateTestModelElement();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<Classifier>(modelElement);
        }

        private class TestContact
        {
            public string Name { get; set; }

            public DateTime? Birthday { get; }
        }
    }
}