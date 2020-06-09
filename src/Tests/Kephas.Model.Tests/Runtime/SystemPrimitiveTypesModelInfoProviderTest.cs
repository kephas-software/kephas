// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemPrimitiveTypesModelInfoProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(bool).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(byte).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(sbyte).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(short).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(ushort).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(int).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(uint).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(long).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(ulong).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(decimal).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(double).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(float).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(DateTime).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(DateTimeOffset).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(TimeSpan).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Uri).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(Guid).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));

            Assert.IsTrue(elementInfos.Any(info => info.Parts.First() == typeof(string).AsRuntimeTypeInfo(this.TypeRegistry) && info.IsPrimitive));
        }
    }
}