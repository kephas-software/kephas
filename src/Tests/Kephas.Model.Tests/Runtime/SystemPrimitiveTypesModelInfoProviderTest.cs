// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemPrimitiveTypesModelInfoProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="SystemPrimitiveTypesModelInfoProvider" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Tests.Runtime.Construction;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="SystemPrimitiveTypesModelInfoProvider"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class SystemPrimitiveTypesModelInfoProviderTest : ConstructorTestBase
    {
        [Test]
        public async Task GetElementInfosAsync()
        {
            var provider = new SystemPrimitiveTypesModelInfoProvider(Substitute.For<IRuntimeModelElementFactory>());
            var context = this.GetConstructionContext();
            var elementInfos = (await provider.GetElementInfosAsync(context)).Cast<IValueType>().ToList();

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(bool).AsRuntimeTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(byte).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(sbyte).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(short).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(ushort).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(int).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(uint).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(long).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(ulong).AsRuntimeTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(decimal).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(double).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(float).AsRuntimeTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(DateTime).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(DateTimeOffset).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(TimeSpan).AsRuntimeTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Uri).AsRuntimeTypeInfo() && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Guid).AsRuntimeTypeInfo() && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(string).AsRuntimeTypeInfo() && info.IsPrimitive));
        }
    }
}