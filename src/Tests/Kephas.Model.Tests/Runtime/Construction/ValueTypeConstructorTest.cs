// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeConstructorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the value type constructor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction
{
    using System;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    using ValueType = Kephas.Model.Elements.ValueType;

    [TestFixture]
    public class ValueTypeConstructorTest : ConstructorTestBase
    {
        private INamedElement TryCreateTestModelElement<T>()
        {
            var constructor = new ValueTypeConstructor();
            var context = this.GetConstructionContext();
            var runtimeElement = typeof(T).AsRuntimeTypeInfo(this.TypeRegistry);
            var modelElement = constructor.TryCreateModelElement(context, runtimeElement);

            return modelElement;
        }

        [Test]
        public void TryCreateModelElement_ReturnType_null_for_not_value_types()
        {
            var modelElement = this.TryCreateTestModelElement<TestContact>();

            Assert.IsNull(modelElement);
        }

        [Test]
        public void TryCreateModelElement_ReturnType_success_for_attributes_not_value_types()
        {
            var modelElement = this.TryCreateTestModelElement<TestAddress>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<ValueType>(modelElement);
        }

        [Test]
        public void TryCreateModelElement_ReturnType_success_for_value_types()
        {
            var modelElement = this.TryCreateTestModelElement<TestMoney>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<ValueType>(modelElement);
        }

        [Test]
        public void TryCreateModelElement_ReturnType_success_for_enums()
        {
            var modelElement = this.TryCreateTestModelElement<TestState>();

            Assert.IsNotNull(modelElement);
            Assert.IsInstanceOf<ValueType>(modelElement);
        }

        private class TestContact
        {
            public string Name { get; set; }

            public DateTime? Birthday { get; }
        }

        private struct TestMoney
        {
            public decimal Value { get; set; }

            public string Currency { get; set; }
        }

        [ValueType]
        private class TestAddress
        {
        }

        private enum TestState
        {
            None,
            InProgress,
            Completed
        }
    }
}