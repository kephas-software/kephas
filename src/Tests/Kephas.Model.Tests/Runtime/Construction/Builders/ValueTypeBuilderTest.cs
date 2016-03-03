// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTypeBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the value type builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.Construction.Builders
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Reflection;

    using NUnit.Framework;

    using Telerik.JustMock;

    [TestFixture]
    public class ValueTypeBuilderTest
    {
        public ValueTypeBuilder CreateBuilder<T>()
        {
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var dynamicType = typeof(T).AsDynamicTypeInfo();
            return new ValueTypeBuilder(context, dynamicType);
        }

        [Test]
        public void Element_success()
        {
            var builder = this.CreateBuilder<ForcedValueType>();

            var element = builder.Element;

            Assert.IsInstanceOf<ValueType>(element);
            Assert.AreEqual("ForcedValueType", element.Name);
        }

        private class ForcedValueType
        {
        }
    }
}