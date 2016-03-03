﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the classifier constructor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    using Telerik.JustMock;

    [TestFixture]
    public class ClassifierConstructorTest
    {
        private INamedElement TryCreateTestModelElement()
        {
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var constructor = new ClassifierConstructor();
            var runtimeElement = typeof(TestContact).AsDynamicTypeInfo();
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