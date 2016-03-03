// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreSimpleTypesModelInfoProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="CoreSimpleTypesModelInfoProvider" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// Tests for <see cref="CoreSimpleTypesModelInfoProvider"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CoreSimpleTypesModelInfoProviderTest
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var provider = new CoreSimpleTypesModelInfoProvider(Mock.Create<IRuntimeModelElementFactory>());
            var context = new ModelConstructionContext { ModelSpace = Mock.Create<IModelSpace>() };
            var elementInfos = (await provider.GetElementInfosAsync(context)).Cast<IValueType>().ToList();

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(bool).AsDynamicTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(byte).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(sbyte).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(short).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(ushort).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(int).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(uint).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(long).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(ulong).AsDynamicTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(decimal).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(double).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(float).AsDynamicTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(DateTime).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(DateTimeOffset).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(TimeSpan).AsDynamicTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Uri).AsDynamicTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Guid).AsDynamicTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(string).AsDynamicTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Id).AsDynamicTypeInfo() && info.IsPrimitive));
        }
    }
}